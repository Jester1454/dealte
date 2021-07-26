using System;
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
        public Action OnTakeDamage;
        public Action OnDeath;

        private bool _isDead = false;
        private float _currentHealth;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        public void Damage(float damage)
        {
            if (_isDead) return;
            
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
            }
        }
    }
}
