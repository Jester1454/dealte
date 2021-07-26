using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Utilities
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class DebugEnemyGizmos : MonoBehaviour
	{
		[SerializeField] private bool _showVelocity;
		[SerializeField] private bool _showDesiredVelocity;
		[SerializeField] private bool _showPath;
		
		private NavMeshAgent _agent;
		
		private void Awake()
		{
			_agent = GetComponent<NavMeshAgent>();
		}

		private void OnDrawGizmos()
		{
			if (_agent == null) return;
			
			if (_showVelocity)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine(transform.position, transform.position + _agent.velocity);
			}

			if (_showDesiredVelocity)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawLine(transform.position, transform.position + _agent.desiredVelocity);
			}

			if (_showPath)
			{
				Gizmos.color = Color.black;

				var prevPathCorner = transform.position;
				foreach (var pathCorner in _agent.path.corners)
				{
					Gizmos.DrawLine(prevPathCorner, pathCorner);
					Gizmos.DrawSphere(pathCorner, 0.1f);
					prevPathCorner = pathCorner;
				}
			}
		}
	}
}