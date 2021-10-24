using System.Collections;
using System.Collections.Generic;
using Enemy.EnemyBehaviours.SensorySystems;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using Player.Behaviours.AttackSystem;
using Player.Behaviours.HealthSystem;
using RPGCharacterAnimsFREE;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.EnemyStates
{
	public class MeleeAttack : ActionTask<Transform>
	{
		[RequiredField] public BBParameter<Animator> _animator;
		[RequiredField] public BBParameter<List<AttackCollider>> _attackColliders;
		[RequiredField] public BBParameter<SensorySystem> _sensorySystem;
		[RequiredField] public BBParameter<NavMeshAgent> _agent;
		[RequiredField] public BBParameter<AnimatorEvents> _animatorEvents;
		[RequiredField] public BBParameter<string> _attackAnimatorKey;
		public BBParameter<float> _damage;
		public BBParameter<float> _rotationSpeed;
		public BBParameter<float> _delayBetweenAttack;
		public BBParameter<float> _beforeBetweenAttack;
		public BBParameter<Vector3> _offsetRotation;

		private void OnDamage(IGettingDamage gettingDamage)
		{
			gettingDamage.Damage(_damage.value, DamageType.Melee);
		}

		private void SetActiveAttackCollider(bool value)
		{
			foreach (var attackCollider in _attackColliders.value)
			{
				attackCollider.SetActive(value);
			}
		}

		protected override void OnExecute()
		{
			StartCoroutine(AttackBeforeDelay());
		}

		private IEnumerator AttackBeforeDelay()
		{
			yield return new WaitForSeconds(_beforeBetweenAttack.value);
			Attack();
		}

		private void StopCurrentAttack()
		{
			_animator.value.ResetTrigger(_attackAnimatorKey.value);

			foreach (var attackCollider in _attackColliders.value)
			{
				attackCollider.OnDamage -= OnDamage;
			}

			SetActiveAttackCollider(false);
			_animatorEvents.value.OnFinishAttack -= OnFinishAttack;
		}

		private void OnFinishAttack()
		{
			StopCurrentAttack();
			StartCoroutine(AttackEndDelay());
		}

		private IEnumerator AttackEndDelay()
		{
			yield return new WaitForSeconds(_delayBetweenAttack.value);
			EndAction(true);
		}

		protected override void OnUpdate()
		{
			RotateTowards(_sensorySystem.value.VisibleTarget.transform);
		}

		private void RotateTowards(Transform target)
		{
			var direction = (target.position - agent.position).normalized;
			var lookRotation = Quaternion.LookRotation(direction);
			agent.rotation = Quaternion.Slerp(agent.rotation, Quaternion.Euler(lookRotation.eulerAngles + _offsetRotation.value),
				Time.deltaTime * _rotationSpeed.value);
		}

		private void Attack()
		{
			_animatorEvents.value.OnFinishAttack += OnFinishAttack;
			foreach (var attackCollider in _attackColliders.value)
			{
				attackCollider.OnDamage += OnDamage;
			}

			if (_agent.value.isOnNavMesh)
			{
				_agent.value.isStopped = true;
			}
			SetActiveAttackCollider(true);

			_animator.value.SetTrigger(_attackAnimatorKey.value);
		}

		protected override void OnStop()
		{
			StopCurrentAttack();
		}
	}
}