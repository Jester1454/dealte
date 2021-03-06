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
		[SerializeField] private AimCursor _aimCursor;
		
		private readonly List<AttackBehaviour> _attackBehaviours = new List<AttackBehaviour>();
		private bool _isEnable = false;
		private IGettingDamage _thisGettingDamage;
		private int _currentAttackBehaviourIndex = -1;
		private AttackBehaviour CurrentAttackBehaviour => _attackBehaviours[_currentAttackBehaviourIndex];

		public Action OnFinish;
		public Action<int> OnEnemyDamage;
		private bool _transactionToNextAttack = false;
		private bool _isFinish = true;
		
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
			
			var hitCount = CurrentAttackBehaviour.OnUpdate(transform);
			if (hitCount > 0)
			{
				OnEnemyDamage?.Invoke(hitCount);
			}
		}

		private void OnAttackFinish()
		{
			CurrentAttackBehaviour.OnFinish -= OnAttackFinish;
			if (_transactionToNextAttack)
			{
				TransactionToNextAttack();
			}
			else
			{
				OnCurrentAttacksFinish();
			}
		}

		private void TransactionToNextAttack()
		{
			_currentAttackBehaviourIndex++;
			
			StartCoroutine(FastRotateToCursorDirection());
			CurrentAttackBehaviour.Start(transform);
			CurrentAttackBehaviour.OnFinish += OnAttackFinish;

			_transactionToNextAttack = false;
		}

		private void OnCurrentAttacksFinish()
		{
			_isFinish = true;
			_transactionToNextAttack = false;
			_animator.applyRootMotion = false;
			OnFinish?.Invoke();
			StartCoroutine(IdleTimer(CurrentAttackBehaviour.AttackData.IdleDuration));
			_currentAttackBehaviourIndex = -1;
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
			_animator.applyRootMotion = true;

			if (_isFinish)
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
			
			StartCoroutine(FastRotateToCursorDirection());
			CurrentAttackBehaviour.Start(transform);
			CurrentAttackBehaviour.OnFinish += OnAttackFinish;

			_isFinish = false;
		}
		
		
		private IEnumerator FastRotateToCursorDirection()
		{
			if (_aimCursor != null && _aimCursor.ShowAimCursor)
			{
				var dir = _aimCursor.CursorPosition - transform.position;
				_animator.applyRootMotion = false;
				transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z), Vector3.up);
				yield return new WaitForEndOfFrame();
				_animator.applyRootMotion = true;
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