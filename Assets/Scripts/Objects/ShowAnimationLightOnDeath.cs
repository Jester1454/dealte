using Player.Behaviours.HealthSystem;
using UnityEngine;
using Utils;

namespace Objects
{
	public class ShowAnimationLightOnDeath : MonoBehaviour
	{
		[SerializeField] private HealthBehaviour _healthBehaviour;
		[SerializeField] private SavePointShowAnimation _savePointShowAnimation;
		[SerializeField] private float _animationDuration;
		
		private bool _isShowing = false;

		private void OnEnable()
		{
			_healthBehaviour.OnDeath += OnDeath;
		}

		private void OnDeath()
		{
			EnableLight();
		}

		private void EnableLight()
		{
			if (_isShowing) return;
			
			_isShowing = true;
			StartCoroutine(_savePointShowAnimation.Show(_animationDuration));
		}
	}
}