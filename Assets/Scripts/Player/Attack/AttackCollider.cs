using System;
using Player.Behaviours.HealthSystem;
using UnityEngine;

namespace Player.Behaviours.AttackSystem
{
	public class AttackCollider : MonoBehaviour
	{
		[SerializeField] private Collider _collider;
		
		public Action<IGettingDamage> OnDamage;

		private void OnEnable()
		{
			_collider.enabled = false;
		}

		public void SetActive(bool value)
		{
			_collider.enabled = value;
		}
		
		private void OnTriggerEnter(Collider other)
		{
			var takingDamage = other.GetComponent<IGettingDamage>();

			if (takingDamage != null)
			{
				OnDamage?.Invoke(takingDamage);
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(transform.position, -transform.right * 5f);
		}
	}
}