using System;
using Player.Movement;
using UnityEngine;

namespace Player.Behaviours.AttackSystem
{
	public class AimBehaviour : MonoBehaviour
	{
		[SerializeField] private float _maxDistance;
		[SerializeField] private float _characterRadius;
		[SerializeField] private ThrowBehaviour _throwBehaviour;
		[SerializeField] private GameObject _target;
		[SerializeField] private float _heightOffset;
		[SerializeField] private LayerMask _obstaclesLayerMask;
		[SerializeField] private float _offset = 0.5f;
		[SerializeField] private float _targetSpeed;
		[SerializeField] private CharacterBehaviour _characterBehaviour;
		[SerializeField] private Animator _animator;
		[SerializeField] private float _rotationSpeed;
		[SerializeField] private CharacterMovementAnimator _movementAnimator;

		private static readonly int _aimingAnimationKey = Animator.StringToHash("IsAiming");
		public Action OnFinishThrowing; 
		private bool _isAiming;
		private bool _isEnable;
		private Transform _currentTarget;
		private GameObject _targetParent;
		private Vector3 _forward;
		private Vector3 _right;
		private Vector3 _lastAimInput;

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

		private void Awake()
		{
			_forward = Camera.main.transform.forward;
			_forward.y = _heightOffset;
			_forward = _forward.normalized;
			_right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;
		}

		private void FinishTrowing()
		{
			OnFinishThrowing?.Invoke();
		}

		public void StartAiming()
		{
			if (!_isEnable)
				return;
			_animator.SetBool(_aimingAnimationKey, true);
			_isAiming = true;
			_targetParent = new GameObject("target");
			_currentTarget = Instantiate(_target, _targetParent.transform).transform;
			_targetParent.transform.position = transform.position + transform.forward * _characterRadius;
			_currentTarget.position = _targetParent.transform.position;
		}

		public void FinishAiming(bool isCancel)
		{
			_isAiming = false;
			Destroy(_targetParent);
			_animator.SetBool(_aimingAnimationKey, false);
			if (!isCancel)
			{
				_throwBehaviour.Throw(_currentTarget.position);
			}
		}
		
		private void UpdateTargetPosition(Vector2 aimInput)
		{
			aimInput.Normalize();
			var targetPosition = _forward.normalized * aimInput.y + _right.normalized * aimInput.x + _currentTarget.position;
			targetPosition.y = _heightOffset + transform.position.y;
			if (Vector3.Distance(transform.position, targetPosition) < _maxDistance)
			{
				_currentTarget.position = Vector3.Lerp(_currentTarget.position, CheckObstacles(targetPosition),
					Time.deltaTime * _targetSpeed);
			}
		}

		private Vector3 CheckObstacles(Vector3 position)
		{
			var origin = transform.position + transform.forward * _characterRadius;
			origin.y = _heightOffset + transform.position.y;
			Debug.DrawLine(origin, position, Color.cyan);
			
			if (Physics.Raycast(origin, transform.forward, out var hitInfo, Vector3.Distance(origin, position) + _offset, _obstaclesLayerMask))
			{
				if (hitInfo.transform.CompareTag(tag))
					return position;
				
				var newPosition = transform.position + transform.forward.normalized * ((hitInfo.point - origin).magnitude);
				return new Vector3(newPosition.x, transform.position.y + _heightOffset, newPosition.z);
			}

			return position;
		}

		private void AttachTargetToCharacter()
		{
			_targetParent.transform.position = Vector3.Lerp(transform.position, _targetParent.transform.position, _targetSpeed * Time.deltaTime);
		}

		private void UpdatePCharacterRotation(Vector3 targetPosition)
		{
			if (!_isAiming)
				return;

			if (!_isEnable)
				return;
			
			var direction = targetPosition - transform.position;
			direction.y = 0;
			direction.Normalize();
			
			if (Mathf.Approximately(direction.magnitude, 0))
				return;

			var targetRotation = Quaternion.LookRotation(direction, Vector3.up);
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
			
			if (_lastAimInput == direction)
				return;
			
			_movementAnimator.UpdateAnimatorState(direction);
			_lastAimInput = direction;
		}
		
		private void Update()
		{
			if (!_isAiming)
				return;

			AttachTargetToCharacter();
			UpdateTargetPosition(_characterBehaviour.PlayerControls.Gameplay.Aiming.ReadValue<Vector2>());
			UpdatePCharacterRotation(_currentTarget.position);
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