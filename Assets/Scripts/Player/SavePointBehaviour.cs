﻿using System.Collections;
using Player.Behaviours.HealthSystem;
using UnityEngine;

namespace Player
{
	public class SavePointBehaviour : MonoBehaviour
	{
		[SerializeField] private HealthBehaviour _healthBehaviour;
		[SerializeField] private string _savePointTag;
		[SerializeField] private float _starDamageDelay = 1f;
		[SerializeField] private float _damageRate = 0.5f;
		[SerializeField] private float _damage = 0.1f;
		[SerializeField] private float _healRate = 0.5f;
		[SerializeField] private float _heal = 0.1f;
		[SerializeField] private float _starHealDelay = 1f;
		
		private bool _isDisable = true;
		private bool _isTakingDamage;
		private bool _isHeal;
		private Coroutine _damageCoroutine;
		private Coroutine _healCoroutine;

		private void OnTriggerEnter(Collider other)
		{
			if (_isDisable)
				return;
		
			if (other.CompareTag(_savePointTag))
			{
				StopDamage();
				StartHeal();
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (_isDisable)
				return;
		
			if (other.CompareTag(_savePointTag))
			{
				StopDamage();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (_isDisable)
				return;
			
			if (other.CompareTag(_savePointTag))
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
			if (_damageCoroutine != null)
			{
				StopCoroutine(_damageCoroutine);
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
			if (_isHeal)
				return;
			
			_isHeal = true;
			_healCoroutine = StartCoroutine(Heal());
		}

		private IEnumerator Damage()
		{
			yield return new WaitForSeconds(_starDamageDelay);
			var rate = new WaitForSeconds(_damageRate);

			while (_isTakingDamage)
			{
				_healthBehaviour.Damage(_damage, true);
				yield return rate;
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
			StartDamage();
		}
	}
}
