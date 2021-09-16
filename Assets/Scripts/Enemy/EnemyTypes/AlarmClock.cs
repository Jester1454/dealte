using System.Linq;
using Enemy.EnemyBehaviours.SensorySystems;
using Enemy.EnemyStates;
using FSM;
using Player.Behaviours.HealthSystem;
using RPGCharacterAnimsFREE;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
	public class AlarmClock : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		[SerializeField] private PatrolStateData _patrolStateData;
		[SerializeField] private HealthBehaviour _healthBehaviour;
		[SerializeField] private Rigidbody _rigidbody;
		[SerializeField] private Collider _collider;
		
		private StateMachine _stateMachine;
		private NavMeshAgent _navMeshAgent;

		private void Awake()
		{
			_navMeshAgent = GetComponent<NavMeshAgent>();

			var enemyContextData = new EnemyContextData(_animator, _navMeshAgent, transform, null, _rigidbody, _collider, null);
			
			_stateMachine = new StateMachine(this);	
			_stateMachine.AddState("Patrol", new PatrolState1(false, enemyContextData, _patrolStateData));
			_stateMachine.AddState("Die", new DeathState1(false, enemyContextData));
			
			_stateMachine.AddTransitionFromAny(new Transition("Any", "Die", transition => _healthBehaviour.CurrentHealth <= 0, true));
		}

		private void Start()
		{
			_stateMachine.Init();
		}

		private void Update()
		{
			_stateMachine.OnLogic();
		}

		private void OnDrawGizmos()
		{
			var previousWayPoint = _patrolStateData.WayPoints.FirstOrDefault();
			foreach (var wayPoint in _patrolStateData.WayPoints)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(wayPoint.position, 0.3f);
				Gizmos.color = Color.green;
				Gizmos.DrawLine(previousWayPoint.position, wayPoint.position);

				previousWayPoint = wayPoint;
			}
		}
	}
}