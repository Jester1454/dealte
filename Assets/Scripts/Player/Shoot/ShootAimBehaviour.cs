using Player.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class ShootAimBehaviour : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		[SerializeField] private float _rotationSpeed;
		[SerializeField] private GameObject _rotationIndication;
		[SerializeField] private Vector3 _rotationOffset;
		[SerializeField] private AimCursor _aimCursor;
		[SerializeField] private CharacterMovementAnimator _movementAnimator;

		private Vector2 _lastAimInput;
		
		private bool _isEnable = false;
		private bool _isAiming = false;
		private static readonly int _aimingAnimationKey = Animator.StringToHash("IsAiming");
		private GameObject _currentVfx;
		public bool IsEnable => _isEnable;

		public void Enable()
		{
			_isEnable = true;
		}

		public void Disable()
		{
			_isEnable = false;
		}

		public void StartAiming()
		{
			if (!_isEnable)
				return;
			
			_isAiming = true;
			_animator.SetBool(_aimingAnimationKey, _isAiming);
			_currentVfx = Instantiate(_rotationIndication, transform.position, Quaternion.identity);
		}

		public void EndAiming()
		{
			if (!_isEnable)
				return;
			_isAiming = false;
			_animator.SetBool(_aimingAnimationKey, _isAiming);
			Destroy(_currentVfx);
		}

		private void UpdateAimingProcess()
		{
			var aimInput = _lastAimInput;

			if (_aimCursor.ShowAimCursor)
			{
				var cursorPosition = _aimCursor.CursorPosition;
				aimInput = new Vector2(cursorPosition.x - transform.position.x, cursorPosition.z - transform.position.z);
				aimInput.Normalize();
			}
			else 
			{
				aimInput = Gamepad.current.leftStick.ReadValue();
			}
			
			UpdateAimingProcess(aimInput);
		}

		private void UpdateAimingProcess(Vector2 aimInput)
		{
			if (!_isEnable)
				return;
			
			if (!_isAiming)
				return;

			if (Mathf.Approximately(aimInput.magnitude, 0))
			{
				_currentVfx.transform.position = transform.position;
				_currentVfx.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + _rotationOffset);
				return;
			}

			var targetRotation = Quaternion.LookRotation(new Vector3(aimInput.x, 0, aimInput.y), Vector3.up);

			var newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
			transform.rotation = newRotation;
			_currentVfx.transform.position = transform.position;
			_currentVfx.transform.rotation = Quaternion.Euler(newRotation.eulerAngles + _rotationOffset);
			
			if (_lastAimInput == aimInput)
				return;
			
			_movementAnimator.UpdateAnimatorState(aimInput);
			_lastAimInput = aimInput;
		}

		private void Update()
		{
			if (!_isAiming)
				return;
			UpdateAimingProcess();
		}
	}
}