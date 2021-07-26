using UnityEngine;

namespace Enemy.EnemyBehaviours
{
	public interface IEatableObject
	{
		void StartEat();
		void EndEat();
		Vector3 EatAnimationScale { get; }
	}
	
	public class EatableObject : MonoBehaviour, IEatableObject
	{
		[SerializeField] private Collider _collider;
		[SerializeField] private Rigidbody _rigidBody;
		[SerializeField] private Vector3 _eatAnimationScale = new Vector3(0.5f, 0.5f, 0.5f);
		public Vector3 EatAnimationScale => _eatAnimationScale;

		public void StartEat()
		{
			_collider.enabled = false;
			_rigidBody.useGravity = false;
			_rigidBody.detectCollisions = false;
		}

		public void EndEat()
		{
			Destroy(gameObject);
		}
	}
}
