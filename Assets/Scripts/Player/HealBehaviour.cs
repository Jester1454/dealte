using System;
using Player.Animations;
using Player.Behaviours.HealthSystem;
using Player.PickUp;
using RPGCharacterAnimsFREE;
using UnityEngine;

namespace Player
{
	public class HealBehaviour : MonoBehaviour
	{
		[SerializeField] private PickUpMedKitBehaviour _pickUpMedKitBehaviour;
		[SerializeField] private HealthBehaviour _healthBehaviour;
		[SerializeField] private Animator _animator;
		[SerializeField] private IKHands _ikHands;
		[SerializeField] private AnimatorEvents _animatorEvents;
		
		private static readonly int _healAnimatorKey = Animator.StringToHash("IsHeal");
		public event Action OnHealFinish;
		
		private bool _isEnabled;
		private bool _isProcessing;
		public bool IsEnabled => _isEnabled;

		public bool CanHeal => !(_isProcessing || !_isEnabled ||
		                         Math.Abs(_healthBehaviour.MaxHealth - _healthBehaviour.CurrentHealth) < Mathf.Epsilon);
		
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
			if (!CanHeal) return;
			
			_isProcessing = true;
			
			_pickUpMedKitBehaviour.UseHealMedKit();
			_healthBehaviour.Heal(_healthBehaviour.MaxHealth);
			_animator.SetTrigger(_healAnimatorKey);
			_ikHands.SetIKOff();
			_animatorEvents.OnHealFinished += AnimatorEventsOnOnHealFinished;
		}

		private void AnimatorEventsOnOnHealFinished()
		{
			_ikHands.SetIKOn();
			_animatorEvents.OnHealFinished -= AnimatorEventsOnOnHealFinished;
			OnHealFinish?.Invoke();
			_isProcessing = false;
		}
	}
}