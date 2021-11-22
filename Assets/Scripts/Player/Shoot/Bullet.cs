using System;
using Player.Behaviours.HealthSystem;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Player
{
	public class Bullet : MonoBehaviour
	{
		[SerializeField] private float _colliderRadius;
		[SerializeField] private BulletEffectAnimator _animator;
		
		private Vector3 _velocity = Vector3.zero;
		private float _damage;
		private Collider[] _hits = new Collider[50];
		
		public void StartShot(Vector3 velocity, float damage)
		{
			_velocity = velocity;
			_damage = damage;
			transform.rotation = Quaternion.LookRotation(new Vector3(velocity.x, 0, velocity.z), Vector3.up);
			
			if (_animator != null)
			{
				_animator.PlayMuzzleEffect();
			}
		}

		private void FixedUpdate()
		{
			var currentPosition = transform.position;
			var newPosition = currentPosition + _velocity * Time.deltaTime;
			var length = (currentPosition - newPosition).magnitude;
			Debug.DrawRay(currentPosition, transform.forward * length, Color.magenta);
			
			if (Physics.SphereCast(transform.position, _colliderRadius, transform.forward, out var hitInfo, length))
			{
				if (hitInfo.collider == null) return;
				if (hitInfo.collider.isTrigger) return;
		
				var other = hitInfo.transform;
				var takeDamage = other.gameObject.GetComponent<IGettingDamage>();
				takeDamage?.Damage(_damage, DamageType.Melee, transform.position);
				Destroy(gameObject);
				
				if (_animator != null)
				{
					_animator.PlayHitAnimation(hitInfo.normal, hitInfo.point);
				}
			}
			
			transform.position = newPosition;
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, _colliderRadius);
		}
	}
}