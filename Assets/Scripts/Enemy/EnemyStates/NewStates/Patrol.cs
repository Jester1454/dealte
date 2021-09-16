using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.EnemyStates
{
	public class Patrol : ActionTask<NavMeshAgent>
	{
		[RequiredField] public BBParameter<Animator> _animator;
		[RequiredField] public BBParameter<List<Transform>> _wayPoints;
		public BBParameter<Vector2> _waitingTimeRange;
		public BBParameter<float> _patrolSpeed = 4;
		
		private float _previousSpeed;
		private int _destPointIndex = 0;
		private static readonly int _speed = Animator.StringToHash("Speed");
		private bool _isWaiting;
		private float _onEnterSpeed;
		
		protected override void OnExecute()
		{
			if (agent.isStopped)
			{
				agent.isStopped = false;
			}
			
			if (_wayPoints.value.Count == 0) 
			{
				EndAction(false);
				return;
			}


			_onEnterSpeed = agent.speed;
			agent.speed = _patrolSpeed.value;
			GotoNextPoint();
		}

		private void GotoNextPoint() 
		{
			agent.isStopped = false;
			var nextPoint = _wayPoints.value[_destPointIndex].position;
			
			if (NavMesh.Raycast(agent.transform.position, nextPoint, out var hit, NavMesh.AllAreas))
			{
				NavMesh.FindClosestEdge(nextPoint, out hit, NavMesh.AllAreas);
			}
			
			var path = new NavMeshPath();
			NavMesh.CalculatePath(agent.transform.position, hit.position, NavMesh.AllAreas, path);
			agent.path = path;
			
			_destPointIndex = (_destPointIndex + 1) % _wayPoints.value.Count;
		}

		private IEnumerator Waiting()
		{
			agent.isStopped = true;
			_isWaiting = true;
			yield return new WaitForSeconds(Random.Range(_waitingTimeRange.value.x, _waitingTimeRange.value.y));
			_isWaiting = false;
			GotoNextPoint();
		}

		protected override void OnUpdate()
		{
			if (!agent.pathPending && agent.remainingDistance < 0.5f && !_isWaiting)
			{
				StartCoroutine(Waiting());
			}

			_animator.value.SetFloat(_speed, Mathf.Lerp(_previousSpeed, agent.velocity.magnitude, Time.deltaTime));
			_previousSpeed = agent.velocity.magnitude;
		}

		protected override void OnStop()
		{
			agent.speed = _onEnterSpeed;
		}
	}
}