using System.Collections.Generic;
using FSM;
using Player.Behaviours.AttackSystem;
using UnityEngine;
using System;
using System.Collections;
using Enemy.EnemyBehaviours.SensorySystems;
using Player.Behaviours.HealthSystem;
using RPGCharacterAnimsFREE;
using UnityEngine.AI;

namespace Enemy.EnemyStates
{
	[Serializable]
	public struct MeleeAttackStateData
	{
		[SerializeField] private float _range;
		[SerializeField] private List<AttackCollider> _attackColliders;
		[SerializeField] private float _damage;
		[SerializeField] private float _rotationSpeed;
		[SerializeField] private float _delayBetweenAttack;
		[SerializeField] private Vector3 _offsetRotation;
		
		public float Range => _range;
		public List<AttackCollider> AttackColliders => _attackColliders;
		public float Damage => _damage;
		public float RotationSpeed => _rotationSpeed;
		public float DelayBetweenAttack => _delayBetweenAttack;
		public Vector3 OffsetRotation => _offsetRotation;
	}
	
	public class MeleeAttackState : StateBase
	{
		private readonly Animator _animator;
		private List<AttackCollider> _attackColliders;
		private readonly float _damage;
		private readonly float _rotationSpeed;
		private readonly Transform _transform;
		private readonly SensorySystem _sensorySystem;
		private readonly NavMeshAgent _agent;
		private readonly AnimatorEvents _animatorEvents;
		private bool _isAttack = false;
		private readonly float _delayBetweenAttack;
		private bool _requestExit;
		private Vector3 _offsetRotation;
		
		private static readonly int _attack = Animator.StringToHash("IsAttack");

		public MeleeAttackState(bool needsExitTime, MeleeAttackStateData meleeData, EnemyContextData data) : base(needsExitTime)
		{
			_animator = data.Animator;
			_attackColliders = meleeData.AttackColliders;
			_damage = meleeData.Damage;
			_rotationSpeed = meleeData.RotationSpeed;
			_sensorySystem = data.SensorySystem;
			_transform = data.Transform;
			_agent = data.NavMeshAgent;
			_animatorEvents = data.AnimatorEvents;
			_delayBetweenAttack = meleeData.DelayBetweenAttack;
			_offsetRotation = meleeData.OffsetRotation;
		}

		private void OnDamage(IGettingDamage gettingDamage)
		{
			gettingDamage.Damage(_damage);
		}

		private void SetActiveAttack(bool value)
		{
			foreach (var attackCollider in _attackColliders)
			{
				attackCollider.SetActive(value);
			}

			_isAttack = value;
		}
		
		public override void OnEnter()
		{
			base.OnEnter();
			Attack();
		}

		private void OnFinishAttack()
		{
			_animator.SetBool(_attack, false);
			
			foreach (var attackCollider in _attackColliders)
			{
				attackCollider.OnDamage -= OnDamage;
			}
			
			SetActiveAttack(false);
			_animatorEvents.OnFinishAttack -= OnFinishAttack;

			if (!_requestExit)
			{
				mono.StartCoroutine(AttackDelay());
			}
		}

		private IEnumerator AttackDelay()
		{
			yield return new WaitForSeconds(_delayBetweenAttack);
			Attack();
		}

		public override void OnLogic()
		{
			base.OnLogic();
			RotateTowards(_sensorySystem.VisibleTarget.transform);
		}
		
		private void RotateTowards(Transform target) 
		{
			var direction = (target.position - mono.transform.position).normalized;
			var lookRotation = Quaternion.LookRotation(direction);
			_transform.rotation = Quaternion.Slerp(_transform.rotation, Quaternion.Euler(lookRotation.eulerAngles + _offsetRotation), Time.deltaTime * _rotationSpeed);
		}

		private void Attack()
		{
			_animatorEvents.OnFinishAttack += OnFinishAttack;
			foreach (var attackCollider in _attackColliders)
			{
				attackCollider.OnDamage += OnDamage;
			}
			
			_agent.isStopped = true;
			
			_animator.SetBool(_attack, true);
			SetActiveAttack(true);
		}

		public override void OnExit()
		{
			base.OnExit();
			SetActiveAttack(false);
			_requestExit = false;
		}

		public override void RequestExit()
		{
			_requestExit = true;
			if (!_isAttack)
			{
				fsm.StateCanExit();
			}
		}
	}
}