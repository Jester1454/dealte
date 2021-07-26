using System.Collections.Generic;
using FSM;
using Player.Behaviours.AttackSystem;
using UnityEngine;
using System;
using Enemy.EnemyBehaviours.SensorySystems;
using Player.Behaviours.HealthSystem;

namespace Enemy.EnemyStates
{
	[Serializable]
	public struct MeleeAttackStateData
	{
		[SerializeField] private float _range;
		[SerializeField] private List<AttackCollider> _attackColliders;
		[SerializeField] private float _damage;
		[SerializeField] private float _rotationSpeed;
		
		public float Range => _range;
		public List<AttackCollider> AttackColliders => _attackColliders;
		public float Damage => _damage;
		public float RotationSpeed => _rotationSpeed;
	}
	
	public class MeleeAttackState : StateBase
	{
		private readonly Animator _animator;
		private List<AttackCollider> _attackColliders;
		private readonly float _damage;
		private readonly float _rotationSpeed;
		private readonly Transform _transform;
		private readonly SensorySystem _sensorySystem;
		
		private static readonly int _attack = Animator.StringToHash("IsAttack");

		public MeleeAttackState(bool needsExitTime, MeleeAttackStateData meleeData, EnemyContextData data) : base(needsExitTime)
		{
			_animator = data.Animator;
			_attackColliders = meleeData.AttackColliders;
			_damage = meleeData.Damage;
			_rotationSpeed = meleeData.RotationSpeed;
			_sensorySystem = data.SensorySystem;
			_transform = data.Transform;
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
		}
		
		public override void OnEnter()
		{
			base.OnEnter();
			Attack();
						
			foreach (var attackCollider in _attackColliders)
			{
				attackCollider.OnDamage += OnDamage;
			}
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
			_transform.rotation = Quaternion.Slerp(_transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
		}

		private void Attack()
		{
			_animator.SetBool(_attack, true);
			SetActiveAttack(true);
		}

		public override void OnExit()
		{
			base.OnExit();

			_animator.SetBool(_attack, false);
			
			foreach (var attackCollider in _attackColliders)
			{
				attackCollider.OnDamage -= OnDamage;
			}
			
			SetActiveAttack(false);
		}
	}
}