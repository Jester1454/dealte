using Enemy.EnemyBehaviours.SensorySystems;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.EnemyStates
{
	public class ChaseVisibleTarget : ActionTask<NavMeshAgent>
	{
		[RequiredField] public BBParameter<Animator> _animator;
		[RequiredField] public BBParameter<SensorySystem> _sensorySystem;
		public BBParameter<float> _speed;
		public BBParameter<float> _stoppingDistance;
	
		private float _onEnterSpeed;
		private float _onEnterStoppingDistance;
		private static readonly int _speedAnimatorKey = Animator.StringToHash("Speed");
	
		protected override void OnExecute()
		{
			if (agent.isStopped)
			{
				agent.isStopped = false;
			}
		
			_onEnterSpeed = agent.speed;
			agent.speed = _speed.value;
		
			_onEnterStoppingDistance = agent.stoppingDistance;
			agent.stoppingDistance = _stoppingDistance.value;
		}

		protected override void OnUpdate()
		{
			agent.destination = _sensorySystem.value.VisibleTarget.transform.position;
			_animator.value.SetFloat(_speedAnimatorKey, agent.velocity.magnitude);
		}

		protected override void OnResume()
		{
			if (agent.isStopped)
			{
				agent.isStopped = false;
			}
		}

		protected override void OnStop()
		{
			if (agent == null)
				return;
			
			agent.isStopped = true;
			agent.ResetPath();
			agent.speed = _onEnterSpeed;
			agent.stoppingDistance = _onEnterStoppingDistance;
			_animator.value.SetFloat(_speedAnimatorKey, 0);
		}
	}
}