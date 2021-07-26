using System;
using FSM;
using Player.Behaviours.HealthSystem;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.EnemyStates
{
	[Serializable]
	public struct ChaseTargetStateData
	{
		[SerializeField] private float _speed;
		[SerializeField] private float _stoppingDistance; 

		public float Speed => _speed;
		public float StoppingDistance => _stoppingDistance;
	}
	
	public class ChaseTargetState : StateBase
	{
		private readonly Animator _animator;
		private readonly float _speed;
		private readonly float _stoppingDistance;
		private readonly NavMeshAgent _agent;
		private readonly EnemyContextData _data;
		
		private float _onEnterSpeed;
		private float _onEnterStoppingDistance;
		private static readonly int _speedAnimatorKey = Animator.StringToHash("Speed");

		public ChaseTargetState(bool needsExitTime, EnemyContextData data, ChaseTargetStateData state) : base(needsExitTime)
		{
			_speed = state.Speed;
			_stoppingDistance = state.StoppingDistance;
			_animator = data.Animator;
			_agent = data.NavMeshAgent;
			_data = data;
		}
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			_onEnterSpeed = _agent.speed;
			_agent.speed = _speed;
			
			_onEnterStoppingDistance = _agent.stoppingDistance;
			_agent.stoppingDistance = _stoppingDistance;
		}

		public override void OnLogic()
		{
			base.OnLogic();

			_agent.destination = _data.SensorySystem.VisibleTarget.transform.position;
			_animator.SetFloat(_speedAnimatorKey, _agent.velocity.magnitude);
		}

		public override void OnExit()
		{
			base.OnExit();
			_agent.speed = _onEnterSpeed;
			_agent.stoppingDistance = _onEnterStoppingDistance;
			_animator.SetFloat(_speedAnimatorKey, 0);
		}
	}
}