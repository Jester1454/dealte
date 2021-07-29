using System.Collections.Generic;
using Player.Behaviours.HealthSystem;
using UnityEngine;

namespace Animations
{
	public class FlashOnDamageAnimation : MonoBehaviour
	{
		[SerializeField] private List<FlashLightAnimation> _animations;
		[SerializeField] private HealthBehaviour _healthBehaviour;
		[SerializeField] private float _duration;
        
		private void OnEnable()
		{
			_healthBehaviour.OnTakeDamage += OnTakeDamage;
		}

		private void OnTakeDamage()
		{
			foreach (var lightAnimation in _animations)
			{
				StartCoroutine(lightAnimation.Flash(_duration));
			}
		}

		private void OnDisable()
		{
			_healthBehaviour.OnTakeDamage -= OnTakeDamage;
		}
	}
}