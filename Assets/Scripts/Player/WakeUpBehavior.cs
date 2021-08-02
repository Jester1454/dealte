using System;
using RPGCharacterAnimsFREE;
using UnityEngine;

namespace Player
{
	public class WakeUpBehavior : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		[SerializeField] private AnimatorEvents _animatorEvents;
		
		public Action OnFinishWakeUp;
		private bool _isDisable = false;
		
		private static readonly int _wakeUp = Animator.StringToHash("WakeUp");
		private static readonly int _haveWeapon = Animator.StringToHash("HaveWeapon");

		public void WakeUp()
		{
			if (_isDisable) return;
			_animatorEvents.OnWakeUpFinish += OnWakeUpFinish;
			_animator.SetTrigger(_wakeUp);
			_animator.SetBool(_haveWeapon, false);
		}

		private void OnWakeUpFinish()
		{
			OnFinishWakeUp?.Invoke();
		}

		public void Disable()
		{
			_isDisable = true;
		}

		public void Enable()
		{
			_isDisable = false;
		}
	}
}