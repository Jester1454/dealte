using System.Collections.Generic;
using UnityEngine;

namespace Player
{
	public class BulletEffectAnimator : MonoBehaviour
	{
		[SerializeField] private GameObject _muzzlePrefab;
		[SerializeField] private GameObject _hitPrefab;
		[SerializeField] private List<GameObject> _trails;

		public void PlayMuzzleEffect()
		{
			if (_muzzlePrefab == null) return;
			
			var muzzleVfx = Instantiate(_muzzlePrefab, transform.position, Quaternion.identity);
			muzzleVfx.transform.forward = gameObject.transform.forward;
			
			var ps = muzzleVfx.GetComponent<ParticleSystem>();
			if (ps != null)
			{
				Destroy(muzzleVfx, ps.main.duration);
			}
			else 
			{
				var psChild = muzzleVfx.transform.GetChild(0).GetComponent<ParticleSystem>();
				Destroy(muzzleVfx, psChild.main.duration);
			}
		}

		public void PlayHitAnimation(Vector3 normal, Vector3 point)
		{
			if (_trails.Count > 0)
			{
				foreach (var trail in _trails)
				{
					trail.transform.parent = null;
					var ps = trail.GetComponent<ParticleSystem>();
					if (ps != null)
					{
						ps.Stop();
						Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
					}
				}
			}
			
			var rot = Quaternion.FromToRotation(Vector3.up, normal);
			var pos = point;

			if (_hitPrefab != null)
			{
				var hitVFX = Instantiate(_hitPrefab, pos, rot);

				var ps = hitVFX.GetComponent<ParticleSystem>();
				if (ps == null)
				{
					var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
					Destroy(hitVFX, psChild.main.duration);
				}
				else
					Destroy(hitVFX, ps.main.duration);
			}
		}
	}
}