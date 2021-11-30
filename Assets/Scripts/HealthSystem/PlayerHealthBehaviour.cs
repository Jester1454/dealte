using System;
using System.Collections;
using System.Collections.Generic;
using Animations;
using UnityEngine;

namespace Player.Behaviours.HealthSystem
{
	public class PlayerHealthBehaviour : HealthBehaviour
	{
		public override void Damage(float damage, DamageType damageType, Vector3 senderPosition, bool disableAnimation = false)
		{
			if (_isDead || _isInvulnerability) return;
            
			_currentHealth -= Mathf.FloorToInt(damage);
			if (Math.Abs(damage) < Mathf.Epsilon)
			{
				return;
			}
            
			if (_currentHealth <= 0)
			{
				Death();
			}
			else
			{
				Damage(damage, disableAnimation, damageType, senderPosition);
			}
		}

		public override void Heal(float healValue)
		{
			if (Mathf.Approximately(_currentHealth, _maxHealth))
				return;
			
			healValue = Mathf.FloorToInt(healValue);
			if (healValue + _currentHealth > _maxHealth)
			{
				_currentHealth = _maxHealth;
			}
			else
			{
				_currentHealth += healValue;
			}
			OnHeal?.Invoke(healValue);
		}
	}
}