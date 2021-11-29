using System;
using Player.Behaviours.HealthSystem;
using UnityEngine;

namespace Objects
{
    public class Trigger : MonoBehaviour
    {
        private enum TriggerType
        {
            Damage,
            Light
        }

        [SerializeField] private TriggerType _type;
        [SerializeField] private int _id;
        [SerializeField] private HealthBehaviour _healthBehaviour;
        public event Action<int> OnTrigger;
        public int Id => _id;
        private bool _isTriggered;

        private void OnEnable()
        {
            if (_type == TriggerType.Damage)
            {
                _healthBehaviour.OnDeath += OnDeath;
            }
        }

        private void OnDeath()
        {
            EnableLight();
        }

        private void EnableLight()
        {
            if (_isTriggered) return;
            
			OnTrigger?.Invoke(_id);
            _isTriggered = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_type == TriggerType.Light)
            {
                OnLightTrigger(other);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (_type == TriggerType.Light)
            {
                OnLightTrigger(other);
            }
        }

        private void OnLightTrigger(Collider other)
        {
            if (_isTriggered) return;
            
            if (other.GetComponent<ISavePoint>() != null)
            {
                OnTrigger?.Invoke(_id);
                _isTriggered = true;
            }
        }

        private void OnDisable()
        {
            if (_type == TriggerType.Damage)
            {
                _healthBehaviour.OnDeath -= OnDeath;
            }
        }
    }
}