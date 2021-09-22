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
			Color color = Color.red;
			color.a = 0.6f;
			Gizmos.color = color;
			if (!Application.isPlaying && _collider && !_collider.enabled) _collider.enabled = true;
			if (_collider && _collider.enabled)
			{
				if (_collider as BoxCollider)
				{
					BoxCollider box = _collider as BoxCollider;

					var sizeX = transform.lossyScale.x * box.size.x;
					var sizeY = transform.lossyScale.y * box.size.y;
					var sizeZ = transform.lossyScale.z * box.size.z;
					Matrix4x4 rotationMatrix = Matrix4x4.TRS(box.bounds.center, transform.rotation, new Vector3(sizeX, sizeY, sizeZ));
					Gizmos.matrix = rotationMatrix;
					Gizmos.DrawCube(Vector3.zero, Vector3.one);
				}
			}
		}
	}
}