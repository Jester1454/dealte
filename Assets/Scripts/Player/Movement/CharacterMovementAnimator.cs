using UnityEngine;

namespace Player.Movement
{
	public class CharacterMovementAnimator : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		[SerializeField] private CharacterController _characterController;
		[SerializeField] private float _lerpMultiplayer;
		private float _velocityX;
		private float _velocityZ;
		
		private static readonly int _movementXAnimatorKey = Animator.StringToHash("XMovement");
		private static readonly int _movementZAnimatorKey = Animator.StringToHash("ZMovement");
		private static readonly int _speedAnimatorKey = Animator.StringToHash("Speed");
		private static readonly int _isRunAniamtorKey = Animator.StringToHash("IsRun");

		private void Awake()
		{
			_characterController = GetComponent<CharacterController>();
		}
		
		public void UpdateAnimatorState()
		{
			var newVelocityX = Vector3.Dot(_characterController.velocity.normalized, transform.right);
			var newVelocityZ = Vector3.Dot(_characterController.velocity.normalized, transform.forward);

			_velocityX = Mathf.Lerp(_velocityX, newVelocityX, Time.deltaTime * _lerpMultiplayer);
			_velocityZ = Mathf.Lerp(_velocityZ, newVelocityZ, Time.deltaTime * _lerpMultiplayer);
			
			_animator.SetFloat(_movementXAnimatorKey, _velocityX);
			_animator.SetFloat(_movementZAnimatorKey, _velocityZ);
			_animator.SetFloat(_speedAnimatorKey, _characterController.velocity.magnitude);
		}

		public void SetActiveStrafeMovement(bool value)
		{
			_animator.SetLayerWeight(_animator.GetLayerIndex("StrafeLayer"), value ? 1 : 0);
		}

		public void Disable()
		{
			_animator.SetLayerWeight(0, 0f);
		}

		public void Enable()
		{
			_animator.SetLayerWeight(0, 1f);
		}
	}
}