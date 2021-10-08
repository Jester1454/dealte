using System;
using System.Collections;
using System.Collections.Generic;
using Player.Behaviours.HealthSystem;
using RPGCharacterAnimsFREE;
using UnityEngine;

namespace Player.Behaviours.AttackSystem
{
	public class ComboAttackBehaviour : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		[SerializeField] private AnimatorEvents _animatorEvents;
		[SerializeField] private List<AttackData> _attacksData;

		private readonly List<AttackBehaviour> _attackBehaviours = new List<AttackBehaviour>();
		private bool _isEnable = false;
		private IGettingDamage _thisGettingDamage;
		private int _currentAttackBehaviourIndex = -1;
		private AttackBehaviour CurrentAttackBehaviour => _attackBehaviours[_currentAttackBehaviourIndex];

		public Action OnFinish;
		private bool _transactionToNextAttack = false;
		private bool _isFinish = false;
		
		private void OnEnable()
		{
			_thisGettingDamage = GetComponent<IGettingDamage>();
			CreateAttackBehaviours();
		}

		private void CreateAttackBehaviours()
		{
			foreach (var attackData in _attacksData)
			{
				_attackBehaviours.Add(new AttackBehaviour(this, attackData, _thisGettingDamage, _animatorEvents));				
			}
		}

		private void FixedUpdate()
		{
			if (!_isEnable) return;
			if (_currentAttackBehaviourIndex == -1) return;
			
			CurrentAttackBehaviour.OnUpdate(transform);
		}

		private void Update()
		{
			if (!_isEnable) return;
			if (_isFinish) return;
			if (_currentAttackBehaviourIndex == -1) return;

			if (!CurrentAttackBehaviour.IsProcessAttack)
			{
				if (_transactionToNextAttack)
				{
					TransactionToNextAttack();
				}
				else
				{
					OnAttacksFinish();
				}
			}
		}

		private void TransactionToNextAttack()
		{
			_currentAttackBehaviourIndex++;
			CurrentAttackBehaviour.Start(transform);
			_transactionToNextAttack = false;
		}

		private void OnAttacksFinish()
		{
			_isFinish = true;
			OnFinish?.Invoke();
			StartCoroutine(IdleTimer(CurrentAttackBehaviour.AttackData.IdleDuration));
			_currentAttackBehaviourIndex = -1;
			_transactionToNextAttack = false;
		}

		private IEnumerator IdleTimer(float duration)
		{
			if (duration <= 0) yield break;

			_isEnable = false;
			yield return new WaitForSeconds(duration);
			_isEnable = true;
		}

		public void Attack()
		{
			if (!_isEnable) return;
			
			if (_currentAttackBehaviourIndex == -1)
			{
				StarNewAttackChain();
				return;
			}
			
			if (CurrentAttackBehaviour.IsProcessAttack)
			{
				var newIndex = _currentAttackBehaviourIndex + 1;
				if (newIndex < _attackBehaviours.Count)
				{
					_animator.SetTrigger(_attackBehaviours[newIndex].AttackData.AnimatorKey);
					_transactionToNextAttack = true;
				}
			}
			else
			{
				StarNewAttackChain();
			}
		}

		private void StarNewAttackChain()
		{
			_currentAttackBehaviourIndex = 0;
			_animator.SetTrigger(CurrentAttackBehaviour.AttackData.AnimatorKey);
			CurrentAttackBehaviour.Start(transform);
			_isFinish = false;
		}

		public void Enable()
		{
			_isEnable = true;
		}

		public void Disable()
		{
			_isEnable = false;
		}

		public bool IsEnable => _isEnable;

		private void OnDrawGizmos()
		{
			foreach (var attackData in _attacksData)
			{
				if (attackData.HitBox != null)
				{
					Gizmos.color = attackData.DebugColor;
					Gizmos.DrawWireMesh(attackData.HitBox.CreateSegmentMesh(), transform.position, transform.rotation);
				}
			}
		}
	}
}