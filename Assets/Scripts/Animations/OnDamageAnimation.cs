using System.Collections;
using Player.Behaviours.HealthSystem;
using UnityEngine;
using UnityEngine.VFX;

namespace Animations
{
	public class OnDamageAnimation : MonoBehaviour
	{
		[SerializeField] private VisualEffect _visualEffect;
		[SerializeField] private float _duration;
		[SerializeField] private Vector3 _vfxVelocityPower;
		[SerializeField] private float _minimumVelocity;
		[SerializeField] private HealthBehaviour _healthBehaviour;

		private void OnEnable() 
		{
			_healthBehaviour.OnTakeDamage += OnTakeDamage;
		}
		
		private void OnDisable()
		{
			_healthBehaviour.OnTakeDamage -= OnTakeDamage;
		}

		private void OnTakeDamage(DamageType damageType)
		{
			if (damageType == DamageType.Melee)
			{
				// StartCoroutine(SpawnAnimation(other.ClosestPointOnBounds(transform.position), (other.ClosestPointOnBounds(transform.position)).normalized));
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
			return new Vector3(Mathf.Max(_minimumVelocity, direction.x * _vfxVelocityPower.x),
				Mathf.Max(_minimumVelocity, direction.y * _vfxVelocityPower.y),
				Mathf.Max(_minimumVelocity, direction.z * _vfxVelocityPower.z));
		}
	}
}