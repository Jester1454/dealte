using Movement;
using Player.Movement.Settings;
using UnityEngine;

namespace Player.Movement
{
	public class CharacterMovement : MonoBehaviour
	{
		[SerializeField] private MovementSettings _movementSettings;
		[SerializeField] private MovementSettings _strafeMovementSettings;
		[SerializeField] private GravitySettings _gravitySettings;
		[SerializeField] private RotationSettings _rotationSettings;
		[SerializeField] private CameraFollowTarget _cameraFollowTarget;
		[SerializeField] private float _controlRotationSensitivity = 3.0f;
		[SerializeField] private CharacterMovementAnimator _animator;
		
		private CharacterController _characterController;
		private MovementSettings _currentMovementSettings;
		
		private float _targetHorizontalSpeed;
		private Vector2 _currentSpeed;
		private Vector2 _previousSpeed;
		
		private Vector3 _movementInput;
		private Vector3 _lastMovementInput;
		private bool _hasMovementInput;
		private bool _isGrounded;
		private Vector2 _controlRotation;
		private bool _isStop = false;
		
		private Vector3 _forward;
		private Vector3 _right;
		private float _currentMaxSpeed;
		
		private void Awake()
		{
			_characterController = GetComponent<CharacterController>();
			_currentMovementSettings = _movementSettings;

			_currentMaxSpeed = _movementSettings.MaxHorizontalSpeed;
			_forward = Camera.main.transform.forward;
			_forward.y = 0;
			_forward = _forward.normalized;
			_right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;
		}

		public void SetActiveStrafeMovement(bool value)
		{
			_animator.SetActiveStrafeMovement(value);
			_currentMovementSettings = value ? _strafeMovementSettings : _movementSettings;
			_rotationSettings.UseControlRotation = value;
		}

		public void SetActiveWalk(bool value)
		{
			_currentMaxSpeed = value ? _movementSettings.MaxWalkSpeed : _movementSettings.MaxHorizontalSpeed;
		}

		public void Stop()
		{
			_isStop = true;
			_characterController.Move(Vector3.zero);
			_previousSpeed = _currentSpeed;
		}

		public void Continue(bool saveSpeed)
		{
			_isStop = false;

			if (saveSpeed)
			{
				_currentSpeed = _previousSpeed;
			}
		}
		
		public void UpdateCameraInput(Vector2 cameraInput)
		{
			UpdateControlRotation(cameraInput);
		}

		public void UpdateMovementInput(Vector2 movementInput)
		{
			SetMovementInput(movementInput);
		}
		
		private void FixedUpdate()
		{
			UpdateMovementState();
			_cameraFollowTarget.SetPosition(transform.position);
			_cameraFollowTarget.SetControlRotation(_controlRotation);
		}

		private void UpdateMovementState()
		{
			UpdateHorizontalSpeed();
			UpdateVerticalSpeed();
			UpdateMovement();
			
			_isGrounded = _characterController.isGrounded;
			_animator.UpdateAnimatorState();
		}

		private void UpdateMovement()
		{
			Vector3 movement = _currentSpeed.x * GetMovementDirection() + _currentSpeed.y * Vector3.up;
			_characterController.Move(movement * Time.deltaTime);
			OrientToTargetRotation(new Vector3(movement.x, 0, movement.z));
		}

		private void SetMovementInput(Vector2 movementInput)
		{
			var newMovementInput = _movementInput;
			
			if (_movementSettings.IsometricMovement)
			{
				newMovementInput = (_forward * movementInput.y + _right * movementInput.x);
			}
			else
			{
				// Calculate the move direction relative to the character's yaw rotation
				var yawRotation = Quaternion.Euler(0.0f, _controlRotation.y, 0.0f);
				var forward = yawRotation * Vector3.forward;
				var right = yawRotation * Vector3.right;
				newMovementInput = (forward * movementInput.y + right * movementInput.x);
			}

			if (newMovementInput.sqrMagnitude > 1f)
			{
				newMovementInput.Normalize();
			}
			
			bool hasMovementInput = newMovementInput.sqrMagnitude > 0.0f;

			if (_hasMovementInput && !hasMovementInput)
			{
				_lastMovementInput = _movementInput;
			}

			_movementInput = newMovementInput;
			_hasMovementInput = hasMovementInput;
		}

		private void UpdateControlRotation(Vector2 cameraInput)
		{
			Vector2 camInput = cameraInput;

			// Adjust the pitch angle (X Rotation)
			float pitchAngle = _controlRotation.x;
			pitchAngle -= camInput.y * _controlRotationSensitivity;

			// Adjust the yaw angle (Y Rotation)
			float yawAngle = _controlRotation.y;
			yawAngle += camInput.x * _controlRotationSensitivity;

			_controlRotation = new Vector2(pitchAngle, yawAngle);
			
			// Adjust the pitch angle (X Rotation)
			pitchAngle %= 360.0f;
			pitchAngle = Mathf.Clamp(pitchAngle, _rotationSettings.MinPitchAngle, _rotationSettings.MaxPitchAngle);

			yawAngle %= 360.0f;

			_controlRotation = new Vector2(pitchAngle, yawAngle);
		}

		private void UpdateHorizontalSpeed()
		{
			Vector3 movementInput = _movementInput;
			if (movementInput.sqrMagnitude > 1.0f)
			{
				movementInput.Normalize();
			}

			_targetHorizontalSpeed = _isStop ? 0f : movementInput.magnitude * _currentMaxSpeed;
			float acceleration = _hasMovementInput ? _currentMovementSettings.Acceleration : _currentMovementSettings.Decceleration;

			_currentSpeed.x = Mathf.MoveTowards(_currentSpeed.x, _targetHorizontalSpeed, acceleration * Time.deltaTime);
		}

		private void UpdateVerticalSpeed()
		{
			if (_isGrounded)
			{
				_currentSpeed.y = -_gravitySettings.GroundedGravity;
			}
			else
			{
				_currentSpeed.y = Mathf.MoveTowards(_currentSpeed.y, -_gravitySettings.MaxFallSpeed, _gravitySettings.Gravity * Time.deltaTime);
			}
		}

		private Vector3 GetMovementDirection()
		{
			Vector3 moveDir = _hasMovementInput ? _movementInput : _lastMovementInput;
			if (moveDir.sqrMagnitude > 1f)
			{
				moveDir.Normalize();
			}

			return moveDir;
		}

		public void OrientToTargetRotation(Vector3 horizontalMovement)
		{
			if (_rotationSettings.OrientRotationToMovement && horizontalMovement.sqrMagnitude > 0.0f)
			{
				float rotationSpeed = Mathf.Lerp(
					_rotationSettings.MaxRotationSpeed, _rotationSettings.MinRotationSpeed, _currentSpeed.x / _targetHorizontalSpeed);

				Quaternion targetRotation = Quaternion.LookRotation(horizontalMovement, Vector3.up);

				transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
			}
			else if (_rotationSettings.UseControlRotation)
			{
				Quaternion targetRotation = Quaternion.Euler(0.0f, _controlRotation.y, 0.0f);
				transform.rotation = targetRotation;
			}
		}
		
		public void Enable()
		{
			_isStop = false;
			_animator.Enable();
		}

		public void Disable()
		{
			_isStop = true;
			_animator.Disable();
		}
	}
}
