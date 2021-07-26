using Enemy.EnemyBehaviours;
using Enemy.EnemyBehaviours.SensorySystems;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
	public class EnemyContextData
	{
		public Animator Animator { get; private set; }
		public NavMeshAgent NavMeshAgent { get; private set; }
		public Transform Transform { get; private set; }
		public SensorySystem SensorySystem { get; private set; }
		public Rigidbody Rigidbody { get; private set; }
		public Collider Collider { get; private set; }
		public EnemyContextData(Animator animator, NavMeshAgent navMeshAgent, Transform transform, SensorySystem sensorySystem, Rigidbody rigidbody, Collider collider)
		{
			Animator = animator;
			NavMeshAgent = navMeshAgent;
			Transform = transform;
			SensorySystem = sensorySystem;
			Rigidbody = rigidbody;
			Collider = collider;
		}
	}
}