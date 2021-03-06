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
		[SerializeField] private int _loopsCount = 1;
		
		private void OnEnable()
		{
			_healthBehaviour.OnTakeDamage += OnTakeDamage;
			_healthBehaviour.OnDeath += OnDeath;
		}

		[ContextMenu("Play")]
		private void PlayAnimation()
		{
			foreach (var lightAnimation in _animations)
			{
				StartCoroutine(lightAnimation.Flash(_duration, _loopsCount));
			}
		}

		private void OnTakeDamage(float damage, DamageType damageType)
		{
			if (damageType == DamageType.Melee)
			{
				PlayAnimation();
			}
		}

		private void OnDeath()
		{
			PlayAnimation();
		}

		private void OnDisable()
		{
			_healthBehaviour.OnTakeDamage -= OnTakeDamage;
			_healthBehaviour.OnDeath -= OnDeath;
		}
	}
}