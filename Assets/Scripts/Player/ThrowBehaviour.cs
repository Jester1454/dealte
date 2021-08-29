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
		
		[Header("Camera Shaker params")] 
		[SerializeField] protected float _shakeDuration;
		[SerializeField] protected float _amplitude;
		[SerializeField] protected float _frequency;

		private static readonly int _throw = Animator.StringToHash("Throw");
		private bool _isEnable;
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
			CinemachineCameraShaker.Instance.ShakeCamera(_shakeDuration, _amplitude, _frequency);
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
			if (!_isEnable || _throwingObjects.Count == 0)
				return;
			
			_animator.SetTrigger(_throw);	
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

		private void OnDisable()
		{
			_animatorEvents.OnFinishThrowing -= OnFinishThrowing;
			_animatorEvents.OnStartThrowing -= OnStartThrowing;
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(transform.position, transform.forward);
			Gizmos.DrawSphere(transform.forward, 1f);
		}
	}
}