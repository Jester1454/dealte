﻿using System;
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
		private bool _isDisable;
		private static readonly int _dodgeRoll = Animator.StringToHash("DodgeRoll");

		private void OnEnable()
		{
			_animatorEvents.OnDodgeRollFinish += DodgeRollFinish;
		}

		private void DodgeRollFinish()
		{
			_hands.SetIKOn();
			_animator.applyRootMotion = false;
			OnDodgeRollFinish?.Invoke();
		}

		public void MakeDodgeRoll()
		{
			if (_isDisable)
				return;
			_hands.SetIKOff();
			_animator.SetTrigger(_dodgeRoll);
			_animator.applyRootMotion = true;
		}

		private void OnDisable()
		{
			_animatorEvents.OnDodgeRollFinish -= DodgeRollFinish;
		}

		public void Disable()
		{
			_isDisable = true;
		}
	}
}