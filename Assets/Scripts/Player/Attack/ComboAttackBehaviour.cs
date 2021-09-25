using System;
using System.Collections;
using System.Collections.Generic;
using Player.Behaviours.HealthSystem;
using RPGCharacterAnimsFREE;
using UnityEngine;
using Random = UnityEngine.Random;

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
		private int _currentAttackBehaviourIndex;
		private AttackBehaviour CurrentAttackBehaviour => _attackBehaviours[_currentAttackBehaviourIndex];

		public Action OnFinish;
		private bool _canTransitionToNextAttack = false;
		private bool _transactionToNextAttack = false;
		private bool _isFinish = false;
		
		private AnimatorStateInfo _currentAttackState;
		
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
			
			CurrentAttackBehaviour.OnUpdate(transform);
		}

		private void Update()
		{
			if (!_isEnable) return;
			if (_isFinish) return;
			
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
			_canTransitionToNextAttack = true;
			_transactionToNextAttack = false;
		}

		private void OnAttacksFinish()
		{
			_isFinish = true;
			OnFinish?.Invoke();
			StartCoroutine(IdleTimer(CurrentAttackBehaviour.AttackData.IdleDuration));
			_currentAttackBehaviourIndex = 0;
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
				_currentAttackBehaviourIndex = 0;
				_animator.SetTrigger(CurrentAttackBehaviour.AttackData.AnimatorKey);
				CurrentAttackBehaviour.Start(transform);
				_currentAttackState = _animator.GetCurrentAnimatorStateInfo(0);
				_canTransitionToNextAttack = true;
				_isFinish = false;
			}
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

		// private void OnDrawGizmos()
		// {
		// 	foreach (var attackData in _attacksData)
		// 	{
		// 		if (attackData.HitBox != null)
		// 		{
		// 			Gizmos.color = Random.ColorHSV();
		// 			Gizmos.DrawWireMesh(attackData.HitBox.CreateSegmentMesh(), transform.position, transform.rotation);
		// 		}
		// 	}
		// }
	}
}