using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthSegment : MonoBehaviour
    {
        [SerializeField] private Image _healthBar;
        [SerializeField] private Image _damageBar;
        [SerializeField] private Image _healBar;
        [SerializeField] private float _animationDuration = 0.3f;
        private bool _isFull = false;
        public bool IsFull => _isFull;
        
        private void Awake()
        {
            _healthBar.fillAmount = 0f;
            _damageBar.fillAmount = 0f;
            _healBar.fillAmount = 1f;
        }

        public void Heal()
        {
            _isFull = true;
            
            _damageBar.fillAmount = 0f;
            _healBar.fillAmount = 1f;
            _healthBar.DOFillAmount(1f, _animationDuration).onComplete += () =>
            {
                _healBar.fillAmount = 0;
            };
        }

        public void UpdateFill(float value)
        {
            _damageBar.fillAmount = 1f;
            _healthBar.fillAmount = value;
        }

        public void Damage()
        {
            if (!_isFull) return;
            
            _isFull = false;
            _healthBar.fillAmount = 0f;
            _healBar.fillAmount = 0f;
            _damageBar.fillAmount = 1f;
            _damageBar.DOFillAmount(0, _animationDuration);
        }
    }
}