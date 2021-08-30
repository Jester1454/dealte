﻿using System;
using UnityEngine;

namespace Player.Behaviours.AttackSystem
{
	public class ChargeThrowBehaviour : MonoBehaviour
	{
		[SerializeField] private Vector2 _distanceLimit;
		[SerializeField] private ThrowBehaviour _throwBehaviour;
		[SerializeField] private GameObject _target;
		[SerializeField] private float _heightOffset;
		[SerializeField] private float _chargeSpeed;
		
		public Action OnFinishThrowing; 
		private float _chargeTime;
		private float _currentOffset;
		private bool _isCharge;
		private bool _isEnable;
		private GameObject _currentTarget;
		
		public bool CanThrow => _throwBehaviour.CanThrow();
		public bool IsEnable => _isEnable;

		private void OnEnable()
		{
			_throwBehaviour.OnFinishThrowing += FinishTrowing;
		}
		private void OnDisable()
		{
			_throwBehaviour.OnFinishThrowing -= FinishTrowing;
		}

		private void FinishTrowing()
		{
			OnFinishThrowing?.Invoke();
		}

		public void StartThrowCharge()
		{
			if (!_isEnable)
				return;
			
			_isCharge = true;
			_currentTarget = Instantiate(_target);
			_currentOffset = _distanceLimit.x;
		}

		public void FinishCharge(bool isCancel)
		{
			_isCharge = false;
			Destroy(_currentTarget);

			if (!isCancel)
			{
				_throwBehaviour.Throw(_currentTarget.transform.position);
			}
		}

		private void UpdateChargeProcess()
		{
			var newtOffset = _currentOffset + _chargeSpeed * Time.deltaTime;
			var targetPosition = transform.position + transform.forward * newtOffset;
			targetPosition.y = _heightOffset;
			
			if (Vector3.Distance(transform.position, targetPosition) < _distanceLimit.y &&
			    Vector3.Distance(transform.position, targetPosition) > _distanceLimit.x)
			{
				_currentOffset = newtOffset;
				_currentTarget.transform.position = targetPosition;
			}
			else
			{
				targetPosition = transform.position + transform.forward * _currentOffset;
				targetPosition.y = _heightOffset;
				_currentTarget.transform.position = targetPosition;
			}
		}
		
		private void Update()
		{
			if (!_isCharge)
				return;
			
			UpdateChargeProcess();
		}
		
		public void Enable()
		{
			_isEnable = true;
		}

		public void Disable()
		{
			_isEnable = false;
		}
	}
}