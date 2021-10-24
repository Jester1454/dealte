using System;
using System.Collections;
using UnityEngine;

namespace Player.Behaviours.HealthSystem
{
    public interface IGettingDamage
    {
        void Damage(float damage, DamageType damageType, bool disableAnimation = false);
    }

    public class HealthBehaviour : MonoBehaviour, IGettingDamage
    {
        [SerializeField] private float _maxHealth;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _dieAnimationKey = "Die";
        [SerializeField] private string _takeDamageAnimationKey = "TakeDamage";
        [SerializeField] private bool _invulnerability;
        [SerializeField] private float _invulnerabilityDuration;
        
        public Action<DamageType> OnTakeDamage;
        public Action OnDeath;
        public Action OnHeal;
        
        private bool _isDead = false;
        private float _currentHealth;
        private bool _isInvulnerability = false;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        public void Damage(float damage, DamageType damageType, bool disableAnimation = false)
        {
            if (_isDead || _isInvulnerability) return;
            
            _currentHealth -= damage;
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
                Damage(disableAnimation, damageType);
            }
        }

        private void Death()
        {
            if (!string.IsNullOrEmpty(_dieAnimationKey))
            {
                _animator.SetTrigger(Animator.StringToHash(_dieAnimationKey));
            }

            OnDeath?.Invoke();
            _isDead = true;
        }

        private void Damage(bool disableAnimation, DamageType damageType)
        {
            if (!string.IsNullOrEmpty(_takeDamageAnimationKey) && !disableAnimation)
            {
                _animator.SetTrigger(Animator.StringToHash(_takeDamageAnimationKey));
            }
                
            OnTakeDamage?.Invoke(damageType);

            if (_invulnerability)
            {
                StartCoroutine(StartInvulnerability());
            }
        }

        public void Heal(float healValue)
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
            OnHeal?.Invoke();
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
