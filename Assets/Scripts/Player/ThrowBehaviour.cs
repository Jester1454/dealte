using System;
using System.Collections.Generic;
using System.Linq;
using Player.PickUp;
using RPGCharacterAnimsFREE;
using UnityEngine;

namespace Player.Behaviours.AttackSystem
{
	public class ThrowBehaviour : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		[SerializeField] private List<Transform> _throwingObjects;
		[SerializeField] private AnimatorEvents _animatorEvents;
		[SerializeField] private float _force;
		
		private static readonly int _throw = Animator.StringToHash("Throw");
		private bool _isDisabled;
		public Action OnFinishThrowing; 
		
		private void OnEnable()
		{
			_animatorEvents.OnFinishThrowing += FinishThrowing;
			_animatorEvents.OnStartThrowing += OnStartThrowing;
		}

		private void FinishThrowing()
		{
			OnFinishThrowing?.Invoke();
		}

		private void OnStartThrowing()
		{
			var throwingItem = _throwingObjects.FirstOrDefault();

			_throwingObjects.Remove(throwingItem);

			if (throwingItem != null)
			{
				throwingItem.GetComponent<IThrowingObject>().Throw(_force, transform.forward);
			}
		}

		public void AddThrowingObject(IPickableObject pickableObject)
		{
			_throwingObjects.Add(pickableObject.Transform);
		}

		public bool CanThrow()
		{
			return _throwingObjects.Count > 0;
		}
		
		public void Throw()
		{
			if (_isDisabled || _throwingObjects.Count == 0)
				return;
			
			_animator.SetTrigger(_throw);	
		}

		public void Disable()
		{
			_isDisabled = true;
		}

		private void OnDisable()
		{
			_animatorEvents.OnFinishThrowing -= OnFinishThrowing;
			_animatorEvents.OnStartThrowing -= OnStartThrowing;
		}
	}
}