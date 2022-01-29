using System.Collections;
using Enemy.EnemyBehaviours.SensorySystems;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.EnemyStates
{
	public class RangeAttack : ActionTask<Transform>
	{
		[RequiredField] public BBParameter<Animator> _animator;
		[RequiredField] public BBParameter<SensorySystem> _sensorySystem;
		[RequiredField] public BBParameter<NavMeshAgent> _agent;
		[RequiredField] public BBParameter<ShootBehavior> _shootBehaviour;
		public BBParameter<float> _aimTime;
		public BBParameter<float> _rotationSpeed;
		public BBParameter<float> _delayBetweenAttack;
		public BBParameter<float> _delayBeforeAttack;
		public BBParameter<Vector3> _offsetRotation;
		private static readonly int _isAimingAnimatorKey = Animator.StringToHash("StartAim");
		private static readonly int _attackAnimatorKey = Animator.StringToHash("Attack");
		private bool _isStop = false;
		protected override void OnExecute()
		{
			_shootBehaviour.value.Enable();
			StartCoroutine(Aiming());
		}

		private IEnumerator Aiming()
		{
			yield return StartCoroutine(AttackBeforeDelay());
			if (!isRunning) yield break; // maybe its working for animation stuck

			var currentAimTime = _aimTime.value;
			_animator.value.SetTrigger(_isAimingAnimatorKey);
			
			while (currentAimTime > 0)
			{
				RotateTowards(_sensorySystem.value.VisibleTarget.transform);
				currentAimTime -= Time.deltaTime;
				yield return null;
			}
			
			if (!isRunning) yield break;
			Attack();
		}

		private IEnumerator AttackBeforeDelay()
		{
			yield return new WaitForSeconds(_delayBeforeAttack.value);
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
			StartCoroutine(_shootBehaviour.value.Shoot(_sensorySystem.value.VisibleTarget.transform.position - agent.position, false, true));
			_animator.value.SetTrigger(_attackAnimatorKey);
			if (_agent.value.isOnNavMesh)
			{
				_agent.value.isStopped = true;
			}
			
			StartCoroutine(AttackEndDelay());
		}
		
		private IEnumerator AttackEndDelay()
		{
			yield return new WaitForSeconds(_delayBetweenAttack.value);
			EndAction(true);
		}

		protected override void OnStop()
		{
			base.OnStop();
			_isStop = true;
		}

		public override void OnDrawGizmosSelected()
		{
			// Gizmos.color = _attackData.value.DebugColor;
			// Gizmos.DrawWireMesh(_attackData.value.HitBox.CreateSegmentMesh(), agent.position, agent.rotation);
		}
	}
}