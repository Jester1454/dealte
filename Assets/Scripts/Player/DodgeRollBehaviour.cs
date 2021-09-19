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
		
		public Action OnDodgeRollFinish;
		private bool _isEnable;
		private static readonly int _dodgeRoll = Animator.StringToHash("DodgeRoll");

		private void OnEnable()
		{
			_animatorEvents.OnDodgeRollFinish += DodgeRollFinish;
		}

		private void DodgeRollFinish()
		{
			_hands.SetIKOn();
			OnDodgeRollFinish?.Invoke();
		}

		public void MakeDodgeRoll()
		{
			if (!_isEnable)
				return;
			
			_hands.SetIKOff();
			_animator.SetTrigger(_dodgeRoll);
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