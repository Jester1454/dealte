using System;
using System.Collections;
using System.Collections.Generic;
using Animations;
using UnityEngine;

namespace Player.Behaviours.HealthSystem
{
    public interface IGettingDamage
    {
        float CurrentHealth { get; }
        void Damage(float damage, DamageType damageType, Vector3 senderPosition, bool disableAnimation = false);
    }

    public class HealthBehaviour : MonoBehaviour, IGettingDamage
    {
        [SerializeField] protected float _maxHealth;
        [SerializeField] protected float _maxLightArmor;
        [SerializeField] protected bool _lightDamageOnlyforLightArmor;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected string _dieAnimationKey = "Die";
        [SerializeField] protected string _takeDamageAnimationKey = "TakeDamage";
        [SerializeField] protected bool _invulnerability;
        [SerializeField] protected float _invulnerabilityDuration;
        [SerializeField] protected List<OnDamageAnimation> _damageAnimations;
        
        public Action<float, DamageType> OnTakeDamage;
        public Action OnDeath;
        public Action<float> OnHeal;
        
        protected bool _isDead = false;
        protected float _currentHealth;
        protected float _currentLightArmor;
        protected bool _isInvulnerability = false;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;

        private void Awake()
        {
            _currentHealth = _maxHealth;
            _currentLightArmor = _maxLightArmor;
        }

        public virtual void Damage(float damage, DamageType damageType, Vector3 senderPosition, bool disableAnimation = false)
        {
            if (_isDead || _isInvulnerability) return;
            
            if (Math.Abs(damage) < Mathf.Epsilon)
            {
                return;
            }

            if (_currentLightArmor > 0)
            {
                if (damageType == DamageType.Light)
                {
                    _currentLightArmor -= damage;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (damageType == DamageType.Light && _lightDamageOnlyforLightArmor) return;
                
                _currentHealth -= damage;
            }

            if (_currentHealth <= 0 && _currentLightArmor <= 0)
            {
                Death();
            }
            else
            {
                Damage(damage, disableAnimation, damageType, senderPosition);
            }
        }

        protected void Death()
        {
            if (!string.IsNullOrEmpty(_dieAnimationKey) && _animator != null)
            {
                _animator.SetTrigger(Animator.StringToHash(_dieAnimationKey));
            }

            OnDeath?.Invoke();
            _isDead = true;
        }

        protected void Damage(float damage, bool disableAnimation, DamageType damageType, Vector3 senderPosition)
        {
            if (!string.IsNullOrEmpty(_takeDamageAnimationKey) && !disableAnimation && _animator != null)
            {
                _animator.SetTrigger(Animator.StringToHash(_takeDamageAnimationKey));
            }
            OnTakeDamage?.Invoke(damage, damageType);
            PlayOnDamageAnimations(damageType, senderPosition);
            
            if (_invulnerability)
            {
                StartCoroutine(StartInvulnerability());
            }
        }

        private void PlayOnDamageAnimations(DamageType damageType, Vector3 senderPosition)
        {
            if (_damageAnimations == null) return;
            
            foreach (var damageAnimation in _damageAnimations)
            {
                damageAnimation.PlayAnimation(damageType, senderPosition);
            }
        }

        public virtual void Heal(float healValue)
        {
            if (Mathf.Approximately(_currentHealth, _maxHealth))
                return;
            
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

        private IEnumerator StartInvulnerability()
        {
            _isInvulnerability = true;
            yield return new WaitForSeconds(_invulnerabilityDuration);
            _isInvulnerability = false;
        }
    }

    public enum DamageType
    {
        Melee,
        Light
    }
}
