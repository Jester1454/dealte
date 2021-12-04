using Animations;
using DG.Tweening;
using Player.Behaviours.HealthSystem;
using UnityEngine;

namespace Objects
{
	public class OnDeathAnimation : MonoBehaviour
	{
		[SerializeField] private HealthBehaviour _healthBehaviour;
		[SerializeField] private MaterialPropertyChangeAnimation _propertyChangeAnimation;
		[SerializeField] private Vector3 _scaleTargetAnimation;
		[SerializeField] private float _animationDuration;
		
		private void OnEnable()
		{
			_healthBehaviour.OnDeath += OnDeath;
		}

		private void OnDeath()
		{
			_propertyChangeAnimation.PlayAnimation();
			transform.DOScale(_scaleTargetAnimation, _animationDuration);
		}

		private void OnDisable()
		{
			_healthBehaviour.OnDeath -= OnDeath;
		}
	}
}