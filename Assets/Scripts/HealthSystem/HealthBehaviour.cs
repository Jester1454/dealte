using System;
using System.Collections;
using UnityEngine;

namespace Player.Behaviours.HealthSystem
{
    public interface IGettingDamage
    {
        void Damage(float damage);
    }

    public class HealthBehaviour : MonoBehaviour, IGettingDamage
    {
        [SerializeField] private float _maxHealth;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _dieAnimationKey = "Die";
        [SerializeField] private string _takeDamageAnimationKey = "TakeDamage";
        [SerializeField] private bool _invulnerability;
        [SerializeField] private float _invulnerabilityDuration;
        
        public Action OnTakeDamage;
        public Action OnDeath;

        private bool _isDead = false;
        private float _currentHealth;
        private bool _isInvulnerability = false;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        public void Damage(float damage)
        {
            if (_isDead || _isInvulnerability) return;
            
            _currentHealth -= damage;
            if (Math.Abs(damage) < Mathf.Epsilon)
            {
                return;
            }
            
            if (_currentHealth <= 0)
            {
                if (!string.IsNullOrEmpty(_dieAnimationKey))
                {
                    _animator.SetTrigger(Animator.StringToHash(_dieAnimationKey));
                }

                OnDeath?.Invoke();
                _isDead = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(_takeDamageAnimationKey))
                {
                    _animator.SetTrigger(Animator.StringToHash(_takeDamageAnimationKey));
                }
                
                OnTakeDamage?.Invoke();


                if (_invulnerability)
                {
                    StartCoroutine(StartInvulnerability());
                }
            }
        }

        private IEnumerator StartInvulnerability()
        {
            _isInvulnerability = true;
            yield return new WaitForSeconds(_invulnerabilityDuration);
            _isInvulnerability = false;
        }
    }
}
