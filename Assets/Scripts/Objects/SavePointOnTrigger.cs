using UnityEngine;
using Utils;

namespace Objects
{
    public class SavePointOnTrigger : MonoBehaviour
    {
        [SerializeField] private Trigger _trigger;
        [SerializeField] private SavePointShowAnimation _savePointShowAnimation;
        [SerializeField] private float _animationDuration;
        [SerializeField] private bool _showOnAwake;

        private void Awake()
        {
            if (_showOnAwake)
            {
                StartCoroutine(_savePointShowAnimation.Show(_animationDuration));
            }
        }

        private bool _isShowing = false;

        private void OnEnable()
        {
            if(_trigger == null) return;
            
            _trigger.OnTrigger += OnTrigger;
        }

        private void OnDisable()
        {
            if(_trigger == null) return;
            
            _trigger.OnTrigger -= OnTrigger;  
        }

        private void OnTrigger(int id)
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