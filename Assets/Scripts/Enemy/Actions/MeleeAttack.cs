using System.Collections;
using System.Collections.Generic;
using Enemy.EnemyBehaviours.SensorySystems;
using NodeCanvas.Framework;
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
		public BBParameter<float> _damage;
		public BBParameter<float> _rotationSpeed;
		public BBParameter<float> _delayBetweenAttack;
		public BBParameter<Vector3> _offsetRotation;

		private static readonly int _attack = Animator.StringToHash("IsAttack");

		private void OnDamage(IGettingDamage gettingDamage)
		{
			gettingDamage.Damage(_damage.value);
		}

		private void SetActiveAttack(bool value)
		{
			foreach (var attackCollider in _attackColliders.value)
			{
				attackCollider.SetActive(value);
			}
		}

		protected override void OnExecute()
		{
			Attack();
		}

		private void OnFinishAttack()
		{
			_animator.value.SetBool(_attack, false);

			foreach (var attackCollider in _attackColliders.value)
			{
				attackCollider.OnDamage -= OnDamage;
			}

			SetActiveAttack(false);
			_animatorEvents.value.OnFinishAttack -= OnFinishAttack;
		
			StartCoroutine(AttackDelay());
		}

		private IEnumerator AttackDelay()
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

			_agent.value.isStopped = true;

			_animator.value.SetBool(_attack, true);
			SetActiveAttack(true);
		}

		protected override void OnStop()
		{
			SetActiveAttack(false);
		}
	}
}