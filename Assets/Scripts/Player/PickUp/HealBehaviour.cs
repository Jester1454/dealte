using System;
using Player.Behaviours.HealthSystem;
using UnityEngine;

namespace Player.PickUp
{
	public class HealBehaviour : MonoBehaviour
	{
		[SerializeField] private PickUpMedKitBehaviour _pickUpMedKitBehaviour;
		[SerializeField] private HealthBehaviour _healthBehaviour;

		private bool _isEnabled;
		public bool IsEnabled => _isEnabled;
		
		public void Enable()
		{
			_isEnabled = true;
		}

		public void Disable()
		{
			_isEnabled = false;
		}
		
		public void Heal()
		{
			if (!_isEnabled) return;
			if (Math.Abs(_healthBehaviour.MaxHealth - _healthBehaviour.CurrentHealth) < Mathf.Epsilon) return;
			
			_pickUpMedKitBehaviour.UseHealMedKit();
			_healthBehaviour.Heal(_healthBehaviour.MaxHealth);
			//animation??? or VFX
		}
	}
}