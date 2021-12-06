using System;
using System.Collections;
using Animations;
using Objects;
using Player.Behaviours.HealthSystem;
using UnityEngine;

namespace Player
{
	public class SavePointBehaviour : MonoBehaviour
	{
		[SerializeField] private HealthBehaviour _healthBehaviour;
		[SerializeField] private float _starDamageDelay = 1f;
		[SerializeField] private float _timeToDamage = 0.5f;
		[SerializeField] private float _damage = 1f;
		[SerializeField] private bool _canHeal;
		[SerializeField] private float _healRate = 0.5f;
		[SerializeField] private float _heal = 0.1f;
		[SerializeField] private float _starHealDelay = 1f;
		[SerializeField] private DissolveAnimation _dissolveAnimation;
		
		private bool _isDisable = true;
		private bool _isTakingDamage;
		private bool _isHeal;
		private Coroutine _damageCoroutine;
		private Coroutine _healCoroutine;
		private float _currentTimeToDamage;

		public float TimeToDamage => _timeToDamage;
		public float CurrentTimeToDamage => _currentTimeToDamage;

		private void Awake()
		{
			_currentTimeToDamage = _timeToDamage;
			_healthBehaviour.OnHeal += OnHeal;
		}

		private void OnHeal(float value)
		{
			_currentTimeToDamage = _timeToDamage;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (_isDisable)
				return;

			if (other.GetComponent<ISavePoint>() != null)
			{
				StopDamage();
				StartHeal();
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (_isDisable)
				return;

			if (other.GetComponent<ISavePoint>() != null)
			{
				StopDamage();
				StartHeal();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (_isDisable)
				return;
			
			if (other.GetComponent<ISavePoint>() != null)
			{
				StopHeal();
				StartDamage();
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
			_currentTimeToDamage = _timeToDamage;

			if (_damageCoroutine != null)
			{
				StopCoroutine(_damageCoroutine);
			}

			if (_dissolveAnimation != null)
			{
				_dissolveAnimation.FinishAnimation();
			}
		}
		
		private void StopHeal()
		{
			if (_healCoroutine != null)
			{
				_isHeal = false;
				StopCoroutine(_healCoroutine);
			}
		}

		private void StartHeal()
		{
			if (!_canHeal) return;
			if (_isHeal) return;
			
			_isHeal = true;
			_healCoroutine = StartCoroutine(Heal());
		}

		private IEnumerator Damage()
		{
			yield return new WaitForSeconds(_starDamageDelay);
			_currentTimeToDamage = _timeToDamage;

			if (_dissolveAnimation != null)
			{
				_dissolveAnimation.StartAnimation();
			}
			
			while (_isTakingDamage && _currentTimeToDamage > 0)
			{
				_currentTimeToDamage -= Time.deltaTime;

				if (_currentTimeToDamage <= 0 && _isTakingDamage)
				{
					_healthBehaviour.Damage(_damage, DamageType.Light, transform.position, true);
					_currentTimeToDamage = _timeToDamage;
				}
				
				yield return null;
			}
		}
		
		private IEnumerator Heal()
		{
			yield return new WaitForSeconds(_starHealDelay);
			var rate = new WaitForSeconds(_healRate);
			
			while (!_isTakingDamage)
			{
				_healthBehaviour.Heal(_heal);
				yield return rate;
			}
		}
	
		public void Disable()
		{
			_isDisable = true;
		}
		
		public void Enable()
		{
			_isDisable = false;
			CheckCurrentState();
		}

		private void CheckCurrentState()
		{
			var colliders = Physics.OverlapSphere(transform.position, 2f);

			if (colliders == null) return;
			foreach (var otherCollider in colliders)
			{
				if (otherCollider == null) continue;
				var savePoints = otherCollider.GetComponents<ISavePoint>();

				if (savePoints == null) continue;
				StartDamage();
				return;
			}
		}


		private void OnDisable()
		{
			_healthBehaviour.OnHeal -= OnHeal;
		}
	}
}
