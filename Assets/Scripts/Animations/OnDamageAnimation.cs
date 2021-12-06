using System.Collections;
using Player.Behaviours.HealthSystem;
using UnityEngine;
using UnityEngine.VFX;

namespace Animations
{
	public class OnDamageAnimation : MonoBehaviour
	{
		[SerializeField] private Collider _collider;
		[SerializeField] private VisualEffect _visualEffect;
		[SerializeField] private float _duration;
		[SerializeField] private Vector3 _vfxMinVelocityPower;
		[SerializeField] private Vector3 _vfxMaxVelocityPower;
		[SerializeField] private float _minimumVelocity;

		public void PlayAnimation(DamageType damageType, Vector3 senderPosition)
		{
			if (damageType == DamageType.Melee)
			{
				StartCoroutine(SpawnAnimation(_collider.ClosestPointOnBounds(transform.position), (senderPosition - transform.position).normalized));
			}
		}

		private IEnumerator SpawnAnimation(Vector3 spawnPosition, Vector3 direction)
		{
			var currentAnimation = Instantiate(_visualEffect, spawnPosition, Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y), Vector3.up));
			currentAnimation.SetVector3("Velocity", GetVelocity(direction));
			
			yield return new WaitForSeconds(_duration);
			Destroy(currentAnimation.gameObject);
		}


		private Vector3 GetVelocity(Vector3 direction)
		{
			return new Vector3(Mathf.Max(_minimumVelocity, direction.x * Random.Range(_vfxMinVelocityPower.x, _vfxMaxVelocityPower.x)),
				Mathf.Max(_minimumVelocity, Random.Range(_vfxMinVelocityPower.y, _vfxMaxVelocityPower.y)),
				Mathf.Max(_minimumVelocity, Random.Range(_vfxMinVelocityPower.z, _vfxMaxVelocityPower.z)));
		}
	}
}