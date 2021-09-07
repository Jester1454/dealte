using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.EnemyStates
{
	public class Death : ActionTask
	{
		[RequiredField] public BBParameter<Animator> _animator;
		[RequiredField] public BBParameter<Rigidbody> _rigidbody;
		[RequiredField] public BBParameter<Collider> _collider;
		[RequiredField] public BBParameter<NavMeshAgent> _agent;
		
		private static readonly int _die = Animator.StringToHash("Die");

		protected override void OnExecute()
		{
			_rigidbody.value.detectCollisions = false;
			_collider.value.enabled = false;
			_agent.value.isStopped = true;
			_agent.value.enabled = false;
			_animator.value.SetTrigger(_die);
		}
	}
}