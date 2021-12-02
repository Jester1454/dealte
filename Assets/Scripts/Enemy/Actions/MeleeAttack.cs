using System.Collections;
using Enemy.EnemyBehaviours.SensorySystems;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
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
		[RequiredField] public BBParameter<SensorySystem> _sensorySystem;
		[RequiredField] public BBParameter<NavMeshAgent> _agent;
		[RequiredField] public BBParameter<AnimatorEvents> _animatorEvents;
		[RequiredField] public BBParameter<AttackData> _attackData;
		[RequiredField] public BBParameter<HealthBehaviour> _healthBehaviour;
		public BBParameter<float> _rotationSpeed;
		public BBParameter<float> _delayBetweenAttack;
		public BBParameter<float> _delayBeforeAttack;
		public BBParameter<Vector3> _offsetRotation;
		private AttackBehaviour _attackBehaviour;
		protected override void OnExecute()
		{
			StartCoroutine(AttackBeforeDelay());
		}

		private IEnumerator AttackBeforeDelay()
		{
			yield return new WaitForSeconds(_delayBeforeAttack.value);
			Attack();
		}

		private void StopCurrentAttack()
		{
			_animator.value.ResetTrigger(_attackData.value.AnimatorKey);
			_attackBehaviour.OnFinish -= OnFinishAttack;
			_attackBehaviour.Stop();
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
			_attackBehaviour?.OnUpdate(agent);
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
			_attackBehaviour = new AttackBehaviour(MonoManager.current, _attackData.value, _healthBehaviour.value, _animatorEvents.value);

			if (_agent.value.isOnNavMesh)
			{
				_agent.value.isStopped = true;
			}
			_attackBehaviour.Start(agent);
			_attackBehaviour.OnFinish += OnFinishAttack;
			_animator.value.SetTrigger(_attackData.value.AnimatorKey);
		}

		protected override void OnStop()
		{
			StopCurrentAttack();
		}

		public override void OnDrawGizmosSelected()
		{
			Gizmos.color = _attackData.value.DebugColor;
			Gizmos.DrawWireMesh(_attackData.value.HitBox.CreateSegmentMesh(), agent.position, agent.rotation);
		}
	}
}