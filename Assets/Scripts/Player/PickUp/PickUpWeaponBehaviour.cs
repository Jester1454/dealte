﻿using System;
using System.Collections;
using Player.Animations;
using Player.Movement;
using RPGCharacterAnimsFREE;
using UnityEngine;

namespace Player.PickUp
{
	public class PickUpWeaponBehaviour : MonoBehaviour
	{
		[SerializeField] private GameObject _weapon;
		[SerializeField] private bool _hasWeapon;
		[SerializeField] private Animator _animator;
		[SerializeField] private string _weaponTag;
		[SerializeField] private IKHands _ikHands;
		[SerializeField] private AnimatorEvents _animatorEvents;
		[SerializeField] private CharacterMovement _characterMovement;
		[SerializeField] private float _moveSpeed;
		
		private static readonly int _hasWeaponAnimatorKey = Animator.StringToHash("HasWeapon");
		private static readonly int _upWeaponAnimationKey = Animator.StringToHash("PickUpWeapon");

		private GameObject _pickUpWeapon;
		private bool _isEnabled;

		public bool IsEnabled => _isEnabled;
		public Action OnFinishPickUpWeapon;

		private void Awake()
		{
			UpdateHasWeaponStatus();
			_animatorEvents.OnPickUpWeaponFinish += FinishPickUp;
		}
		
		private void OnTriggerEnter(Collider other)
		{
			if (!_isEnabled)
				return;
			
			if (other != null && other.CompareTag(_weaponTag))
			{
				_pickUpWeapon = other.gameObject;
			}
		}
    
		private void OnTriggerExit(Collider other)
		{
			if (!_isEnabled)
				return;
			
			if (ReferenceEquals(_pickUpWeapon, other.gameObject))
			{
				_pickUpWeapon = null;
			}
		}

		public void PickUpWeapon()
		{
			if (_pickUpWeapon == null)
				return;
			
			if (!_isEnabled)
				return;
			StartCoroutine(WalkToPickUpTarget());
		}

		private IEnumerator WalkToPickUpTarget()
		{
			var target = _pickUpWeapon.transform.GetChild(0);
			var distance = 5f;
			
			while (distance > 1.5f)
			{
				distance = Vector2.Distance(new Vector2(_characterMovement.transform.position.x, _characterMovement.transform.position.z),
					new Vector2(target.position.x, target.position.z));

				var direction = (target.position - _characterMovement.transform.position).normalized * _moveSpeed;
				_characterMovement.UpdateMovementInput(direction);
				yield return null;
			}
			
			_characterMovement.UpdateMovementInput((_pickUpWeapon.transform.position - _characterMovement.transform.position).normalized * _moveSpeed);

			_animator.SetTrigger(_upWeaponAnimationKey);
		}

		private void FinishPickUp()
		{
			_hasWeapon = true;
			_pickUpWeapon.SetActive(false);

			UpdateHasWeaponStatus();
			OnFinishPickUpWeapon?.Invoke();
			_animatorEvents.OnPickUpWeaponFinish -= FinishPickUp;
		}

		private void UpdateHasWeaponStatus()
		{
			_weapon.SetActive(_hasWeapon);

			_animator.SetBool(_hasWeaponAnimatorKey, _hasWeapon);
			_isEnabled = !_hasWeapon;

			if (_hasWeapon)
			{
				_ikHands.SetIKOn();
			}
			else
			{
				_ikHands.SetIKOff();
			}
		}
		
		public void Disable()
		{
			_isEnabled = false;
		}
		
		public void Enable()
		{
			_isEnabled = true;
		}
	}
}