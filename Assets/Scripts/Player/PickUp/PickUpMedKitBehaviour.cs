using System;
using UnityEngine;

namespace Player.PickUp
{
	public class PickUpMedKitBehaviour : MonoBehaviour
	{
		[SerializeField] private int _maxKitCount;
		[SerializeField] private Animator _animator;
		
		private int _currentKitCount;
		private IPickableMedKit _currentPickableMedKit;
		private Vector3 _currentMedKitPosition;
			private static readonly int _pickUpAnimatorKey = Animator.StringToHash("PickUp");

		public int CurrentKitCount => _currentKitCount;
		public int MaxKitCount => _maxKitCount;
		public event Action OnMedKitCountChanged;
		
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
		
		private void OnTriggerEnter(Collider other)
		{
			var pickableObject = other.GetComponent<IPickableMedKit>();

			if (pickableObject != null)
			{
				_currentPickableMedKit = pickableObject;
				_currentMedKitPosition = other.transform.position;
			}
		}
    
		private void OnTriggerExit(Collider other)
		{
			var pickableObject = other.GetComponent<IPickableMedKit>();

			if (_currentPickableMedKit == pickableObject)
			{
				_currentPickableMedKit = null;
			}
		}

		public void PickUp()
		{
			if (!_isEnabled) return;
			if (_currentKitCount >= _maxKitCount) return;
			if (_currentPickableMedKit == null) return;
			
			_currentPickableMedKit.PickUp();
			_currentPickableMedKit = null;
			_currentKitCount++;
			_animator.SetTrigger(_pickUpAnimatorKey);
			transform.rotation = Quaternion.LookRotation(_currentMedKitPosition, Vector3.up);
			OnMedKitCountChanged?.Invoke();
		}

		public void UseHealMedKit()
		{
			if (_currentKitCount <= 0) return;
			_currentKitCount--;
			OnMedKitCountChanged?.Invoke();
		}
	}
}