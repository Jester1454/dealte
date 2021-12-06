using System;
using RPGCharacterAnimsFREE;
using UnityEngine;

namespace Player.PickUp
{
	public class PickUpMedKitBehaviour : MonoBehaviour
	{
		[SerializeField] private int _maxKitCount;
		[SerializeField] private Animator _animator;
		[SerializeField] private AnimatorEvents _animatorEvents;
		
		private int _currentKitCount;
		private IPickableMedKit _currentPickableMedKit;
		private Vector3 _currentMedKitPosition;
		private static readonly int _pickUpAnimatorKey = Animator.StringToHash("PickUp");

		public int CurrentKitCount => _currentKitCount;
		public int MaxKitCount => _maxKitCount;
		public event Action OnMedKitCountChanged;
		public event Action OnPickUpFinish;
		
		private bool _isEnabled;
		public bool IsEnabled => _isEnabled;
		private bool _isProcessing;

		public bool CanPickUp => !(_isProcessing || !_isEnabled || _currentKitCount >= _maxKitCount || _currentPickableMedKit == null);
		
		public void Enable()
		{
			_isEnabled = true;
		}

		public void Disable()
		{
			_isEnabled = false;
		}
		
		private void OnTriggerEnter(Collider other)
		{
			if (_isProcessing) return;

			var pickableObject = other.GetComponent<IPickableMedKit>();

			if (pickableObject != null)
			{
				_currentPickableMedKit = pickableObject;
				_currentMedKitPosition = other.transform.position;
			}
		}
    
		private void OnTriggerExit(Collider other)
		{
			if (_isProcessing) return;
			
			var pickableObject = other.GetComponent<IPickableMedKit>();

			if (_currentPickableMedKit == pickableObject)
			{
				_currentPickableMedKit = null;
			}
		}

		public void PickUp()
		{
			if (!CanPickUp) return;
			
			_isProcessing = true;
			_animator.SetTrigger(_pickUpAnimatorKey);
			transform.rotation = Quaternion.LookRotation(_currentMedKitPosition, Vector3.up);
			_animatorEvents.OnFinishMedKitPickUp += AnimatorEventsOnOnFinishMedKitPickUp;
			_currentPickableMedKit.PickUp();
		}

		private void AnimatorEventsOnOnFinishMedKitPickUp()
		{
			_currentPickableMedKit = null;
			_currentKitCount++;
			OnMedKitCountChanged?.Invoke();
			OnPickUpFinish?.Invoke();
			_animatorEvents.OnFinishMedKitPickUp -= AnimatorEventsOnOnFinishMedKitPickUp;
			_isProcessing = false;
		}

		public void UseHealMedKit()
		{
			if (_currentKitCount <= 0) return;
			_currentKitCount--;
			OnMedKitCountChanged?.Invoke();
		}
	}
}