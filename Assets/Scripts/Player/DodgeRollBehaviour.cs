using System;
using Player.Animations;
using RPGCharacterAnimsFREE;
using UnityEngine;

namespace Player
{
	public class DodgeRollBehaviour : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		[SerializeField] private AnimatorEvents _animatorEvents;
		[SerializeField] private IKHands _hands;
		[SerializeField] private CharacterBehaviour _characterBehaviour;

		public Action OnDodgeRollFinish;
		private bool _isEnable;
		private static readonly int _dodgeRoll = Animator.StringToHash("DodgeRoll");
		private Vector3 _forward;
		private Vector3 _right;
		
		private void OnEnable()
		{
			_animatorEvents.OnDodgeRollFinish += DodgeRollFinish;
		}
		
		private void Awake()
		{
			_forward = Camera.main.transform.forward;
			_forward.y = 0;
			_forward = _forward.normalized;
			_right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;
		}

		private void DodgeRollFinish()
		{
			_hands.SetIKOn();
			_animator.applyRootMotion = false;
			OnDodgeRollFinish?.Invoke();
		}

		public void MakeDodgeRoll()
		{
			if (!_isEnable)
				return;

			UpdateDodgeDirection();
			_hands.SetIKOff();
			_animator.SetTrigger(_dodgeRoll);
			_animator.applyRootMotion = true;
		}

		private void UpdateDodgeDirection()
		{
			var movementInput = _characterBehaviour.PlayerControls.Gameplay.Movement.ReadValue<Vector2>();
			var hasMovementInput = movementInput.sqrMagnitude > 0.0f;
			if (!hasMovementInput) return;
			
			var dir = _forward * movementInput.y + _right * movementInput.x;
			if (dir.sqrMagnitude > 1f)
			{
				dir.Normalize();
			}
			
			transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z), Vector3.up);
		}

		private void OnDisable()
		{
			_animatorEvents.OnDodgeRollFinish -= DodgeRollFinish;
		}

		public void Enable()
		{
			_isEnable = true;
		}

		public void Disable()
		{
			_isEnable = false;
		}

		public bool IsEnable => _isEnable;
	}
}