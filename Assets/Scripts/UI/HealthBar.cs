﻿using System.Collections.Generic;
using System.Linq;
using Player;
using Player.Behaviours.HealthSystem;
using UnityEngine;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private HealthSegment _segment;
        [SerializeField] private Transform _parent;
        private readonly List<HealthSegment> _healthSegments = new List<HealthSegment>();
        private PlayerHealthBehaviour _healthBehaviour;
        private SavePointBehaviour _savePointBehaviour;

        public void Init(PlayerHealthBehaviour healthBehaviour, SavePointBehaviour savePointBehaviour)
        {
            _healthBehaviour = healthBehaviour;
            _savePointBehaviour = savePointBehaviour;
            _healthBehaviour.OnTakeDamage += OnDamage;
            _healthBehaviour.OnHeal += OnHeal;

            for (int i = 0; i < _healthBehaviour.MaxHealth; i++)
            {
                var segment = Instantiate(_segment, _parent);
                segment.Heal();
                _healthSegments.Add(segment);
            }
        }

        private void Update()
        {
            if (_savePointBehaviour.CurrentTimeToDamage <= _savePointBehaviour.TimeToDamage)
            {
               var segment = _healthSegments.LastOrDefault(x => x.IsFull);
               if (segment != null)
               {
                   segment.UpdateFill(_savePointBehaviour.CurrentTimeToDamage / _savePointBehaviour.TimeToDamage);
               }
            }
        }

        private void OnHeal(float healValue)
        {
            var segmentCount = Mathf.CeilToInt(healValue);
            foreach (var segment in _healthSegments)
            {
                if (segment.IsFull) continue;
            
                segment.Heal();
                segmentCount--;
                if(segmentCount <= 0) return;
            }
        }

        private void OnDamage(float damage, DamageType type)
        {
            var segmentCount = Mathf.CeilToInt(damage);
            for (var i = _healthSegments.Count - 1; i >= 0; i--)
            {
                var segment = _healthSegments[i];
                if (!segment.IsFull) continue;

                segment.Damage();
                segmentCount--;
                if (segmentCount <= 0) return;
            }
        }

        private void OnDisable()
        {
            _healthBehaviour.OnTakeDamage -= OnDamage;
            _healthBehaviour.OnHeal -= OnHeal;
        }
    }
}
