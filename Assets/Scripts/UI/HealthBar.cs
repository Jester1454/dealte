using DG.Tweening;
using Player.Behaviours.HealthSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _healthBar;
        [SerializeField] private Image _damageBar;
        [SerializeField] private float _animationDuration = 0.3f;
        
        private PlayerHealthBehaviour _healthBehaviour;

        private void Awake()
        {
            _healthBar.fillAmount = 0f;
            _damageBar.fillAmount = 0f;
        }

        public void Init(PlayerHealthBehaviour healthBehaviour)
        {
            _healthBehaviour = healthBehaviour;
            _healthBar.fillAmount = GetFillAmount();
            _damageBar.fillAmount = GetFillAmount();
            _healthBehaviour = healthBehaviour;
            _healthBehaviour.OnTakeDamage += OnTakeDamage;
            _healthBehaviour.OnHeal += OnHeal;
        }

        private void OnHeal()
        {
            _damageBar.fillAmount = GetFillAmount();

            _healthBar.DOFillAmount(GetFillAmount(), _animationDuration);
        }

        private void OnTakeDamage(DamageType damageType)
        {
            _healthBar.fillAmount = GetFillAmount();
            _damageBar.DOFade(0, _animationDuration).onComplete += () =>
            {
                _damageBar.fillAmount = GetFillAmount();
                var color = _damageBar.color;
                _damageBar.color = new Color(color.r, color.g, color.b, 1f);;
            };
        }

        private float GetFillAmount()
        {
            return _healthBehaviour.CurrentHealth / _healthBehaviour.MaxHealth;
        }

        private void OnDisable()
        {
            _healthBehaviour.OnTakeDamage -= OnTakeDamage;
            _healthBehaviour.OnHeal -= OnHeal;
        }
    }
}
