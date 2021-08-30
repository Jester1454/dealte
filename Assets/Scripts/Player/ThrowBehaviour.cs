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
		[SerializeField] private float _speed;
		
		[Header("Camera Shaker params")] 
		[SerializeField] protected float _shakeDuration;
		[SerializeField] protected float _amplitude;
		[SerializeField] protected float _frequency;

		private static readonly int _throw = Animator.StringToHash("Throw");
		public Action OnFinishThrowing;
		private Vector3 _target;
		
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
				throwingItem.GetComponent<IThrowingObject>().Throw(_target, _speed);
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
		
		public void Throw(Vector3 target)
		{
			if (_throwingObjects.Count == 0)
				return;
			_target = target;
			_animator.SetTrigger(_throw);	
		}

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