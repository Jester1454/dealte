using System;
using Player;
using Player.Behaviours.HealthSystem;
using UnityEngine;

namespace Objects
{
    public class Trigger : MonoBehaviour
    {
        public enum TriggerType
        {
            Damage = 0,
            Light = 1,
            Enter = 2
        }

        [SerializeField] public TriggerType _type;
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
            switch (_type)
            {
                case TriggerType.Light:
                    OnLightTrigger(other);
                    break;
                case TriggerType.Enter:
                    OnEnterTrigger(other);
                    break;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            switch (_type)
            {
                case TriggerType.Light:
                    OnLightTrigger(other);
                    break;
                case TriggerType.Enter:
                    OnEnterTrigger(other);
                    break;
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

        private void OnEnterTrigger(Collider other)
        {
            if (_isTriggered) return;
            
            if (other.GetComponent<CharacterBehaviour>() != null)
            {
                OnTrigger?.Invoke(_id);
                UnityEngine.Debug.LogError("ti pidor");
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