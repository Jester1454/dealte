using DG.Tweening;
using NodeCanvas.BehaviourTrees;
using UnityEngine;
using UnityEngine.AI;

namespace Objects
{
	public class CollectableKeyCap : MonoBehaviour, ICollectaleItem
	{
		[SerializeField] private BehaviourTreeOwner _behaviourTreeOwner;
		[SerializeField] private Animator _animator;
		[SerializeField] private int _index;
		[SerializeField] private NavMeshAgent _navMeshAgent;
		[SerializeField] private Collider _collider;
		[SerializeField] private Rigidbody _rigidbody;
		[SerializeField] private Vector3 _finalAngle;
		[SerializeField] private Vector3 _finalScale;
		[SerializeField] private float _animationDuration;
		
		public int Index => _index;
        
		public void Collect(Transform target)
		{
			_behaviourTreeOwner.StopBehaviour();
			
			_animator.Rebind();
			_animator.Update(0f);
			_animator.enabled = false;
			_navMeshAgent.enabled = false;
			_collider.enabled = false;
			_rigidbody.detectCollisions = false;
		
			transform.DOMove(target.position, _animationDuration);
			transform.DORotate(_finalAngle, _animationDuration);
			transform.DOScale(_finalScale, _animationDuration);
			
			// transform.SetParent(target);
		}
	}
}