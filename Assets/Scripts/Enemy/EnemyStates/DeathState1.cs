using FSM;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.EnemyStates
{
	public class DeathState1 : StateBase
	{
		private readonly Animator _animator;
		private readonly Rigidbody _rigidbody;
		private readonly Collider _collider;
		private readonly NavMeshAgent _agent;
		
		private static readonly int _die = Animator.StringToHash("Die");

		public DeathState1(bool needsExitTime, EnemyContextData data) : base(needsExitTime)
		{
			_animator = data.Animator;
			_rigidbody = data.Rigidbody;
			_collider = data.Collider;
			_agent = data.NavMeshAgent;
		}
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			_rigidbody.detectCollisions = false;
			_collider.enabled = false;
			_agent.isStopped = true;
			_agent.enabled = false;
			
			_animator.SetTrigger(_die);
			mono.StopAllCoroutines();
		}
	}
}