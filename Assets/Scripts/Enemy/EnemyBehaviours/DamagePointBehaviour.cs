using System.Collections;
using Animations;
using Player.Behaviours.HealthSystem;
using UnityEngine;

namespace Enemy
{
	public class DamagePointBehaviour : MonoBehaviour
	{
		[SerializeField] private HealthBehaviour _healthBehaviour;
		[SerializeField] private string _savePointTag;
		[SerializeField] private float _starDamageDelay = 1f;
		[SerializeField] private float _damageRate = 0.5f;
		[SerializeField] private float _damage = 0.1f;
		[SerializeField] private DissolveAnimation _dissolveAnimation;

		private bool _isTakingDamage;
		private Coroutine _damageCoroutine;

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag(_savePointTag))
			{
				StartDamage();
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (other.CompareTag(_savePointTag))
			{
				StartDamage();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag(_savePointTag))
			{
				StopDamage();
			}
		}

		private void StartDamage()
		{
			if (_isTakingDamage)
				return;
			
			_isTakingDamage = true;
			_damageCoroutine = StartCoroutine(Damage());
		}

		private void StopDamage()
		{
			_isTakingDamage = false;
			if (_damageCoroutine != null)
			{
				StopCoroutine(_damageCoroutine);
			}
			
			if (_dissolveAnimation != null)
			{
				_dissolveAnimation.FinishAnimation();
			}
		}
		
		private IEnumerator Damage()
		{
			yield return new WaitForSeconds(_starDamageDelay);
			
			if (_dissolveAnimation != null)
			{
				_dissolveAnimation.StartAnimation();
			}
			
			var rate = new WaitForSeconds(_damageRate);
			while (_isTakingDamage)
			{
				_healthBehaviour.Damage(_damage, DamageType.Light, true);
				yield return rate;
			}
		}
	}
}