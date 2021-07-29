using System;
using System.Collections;
using FSM;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemy.EnemyStates
{
	[Serializable]
	public struct PatrolStateData
	{
		[SerializeField] private Vector2 _waitingTimeRange;
		[SerializeField] private float _patrolSpeed;
		[SerializeField] public Transform[] _wayPoints; 

		public Vector2 WaitingTimeRange => _waitingTimeRange;
		public float PatrolSpeed => _patrolSpeed;
		public Transform[] WayPoints => _wayPoints;
	}
	
	public class PatrolState : StateBase
	{
		private readonly Animator _animator;
		private readonly Vector2 _waitingTimeRange;
		private readonly float _patrolSpeed;
		private readonly Transform[] _wayPoints;
		private readonly NavMeshAgent _agent;

		private int _destPointIndex = 0;
		private static readonly int _speed = Animator.StringToHash("Speed");
		private bool _isWaiting;
		private float _onEnterSpeed;
		
		public PatrolState(bool needsExitTime, EnemyContextData data, PatrolStateData patrolStateData) : base(needsExitTime)
		{
			_animator = data.Animator;
			_waitingTimeRange = patrolStateData.WaitingTimeRange;
			_patrolSpeed = patrolStateData.PatrolSpeed;
			_wayPoints = patrolStateData.WayPoints;
			_agent = data.NavMeshAgent;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			
			if (_agent.isStopped)
			{
				_agent.isStopped = false;
			}

			_onEnterSpeed = _agent.speed;
			_agent.speed = _patrolSpeed;
			GotoNextPoint();
		}

		private void GotoNextPoint() 
		{
			if (_wayPoints.Length == 0)
				return;

			_agent.destination = _wayPoints[_destPointIndex].position;
			_destPointIndex = (_destPointIndex + 1) % _wayPoints.Length;
		}

		private IEnumerator Waiting()
		{
			//TODO turn to new direction
			// Vector3 direction = _wayPoints[_destPointIndex].position - transform.position;
			// var angle = Vector3.Angle(direction, transform.forward);

			_isWaiting = true;
			yield return new WaitForSeconds(Random.Range(_waitingTimeRange.x, _waitingTimeRange.y));
			_isWaiting = false;
			GotoNextPoint();
		}

		public override void OnLogic()
		{
			base.OnLogic();

			if (!_agent.pathPending && _agent.remainingDistance < 0.5f && !_isWaiting)
			{
				mono.StartCoroutine(Waiting());
			}
			
			_animator.SetFloat(_speed, _agent.velocity.magnitude);
		}

		public override void OnExit()
		{
			base.OnExit();
			_agent.speed = _onEnterSpeed;
		}
	}
}