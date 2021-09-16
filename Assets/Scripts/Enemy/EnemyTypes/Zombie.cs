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
	public class Zombie : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		[SerializeField] private PatrolStateData _patrolStateData;
		[SerializeField] private ChaseTargetStateData _chasePlayerData;
		[SerializeField] private MeleeAttackStateData _meleeAttackStateData;
		[SerializeField] private SensorySystem _sensorySystem;
		[SerializeField] private HealthBehaviour _healthBehaviour;
		[SerializeField] private Rigidbody _rigidbody;
		[SerializeField] private Collider _collider;
		[SerializeField] private AnimatorEvents _animatorEvents;
		
		private StateMachine _stateMachine;
		private NavMeshAgent _navMeshAgent;

		private void Awake()
		{
			_navMeshAgent = GetComponent<NavMeshAgent>();

			var enemyContextData = new EnemyContextData(_animator, _navMeshAgent, transform, _sensorySystem, _rigidbody, _collider, _animatorEvents);
			
			_stateMachine = new StateMachine(this);	
			_stateMachine.AddState("Patrol", new PatrolState1(false, enemyContextData, _patrolStateData));
			_stateMachine.AddState("ChaseTarget", new ChaseTargetState1(false, enemyContextData, _chasePlayerData));
			_stateMachine.AddState("Die", new DeathState1(false, enemyContextData));
			_stateMachine.AddState("Attack", new MeleeAttackState1(true, _meleeAttackStateData, enemyContextData));
			
			_stateMachine.AddTransition(new Transition("Patrol", "ChaseTarget", transition => _sensorySystem.VisibleTarget));
			_stateMachine.AddTransition(new Transition("ChaseTarget", "Patrol", transition => !_sensorySystem.VisibleTarget));
			_stateMachine.AddTransitionFromAny(new Transition("Any", "Die", transition => _healthBehaviour.CurrentHealth <= 0, true));

			_stateMachine.AddTransition(new Transition("Patrol", "Attack", AttackCondition));
			_stateMachine.AddTransition(new Transition("ChaseTarget", "Attack", AttackCondition));
			_stateMachine.AddTransition(new Transition("Attack", "ChaseTarget", transition => !AttackCondition(transition)));
		}

		private bool AttackCondition(Transition arg)
		{
			return _sensorySystem.VisibleTarget != null && Vector3.Distance(transform.position, _sensorySystem.VisibleTarget.transform.position) < _meleeAttackStateData.Range;
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

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, _meleeAttackStateData.Range);
		}
	}
}