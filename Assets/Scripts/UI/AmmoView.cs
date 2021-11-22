using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AmmoView : MonoBehaviour
    {
        [SerializeField] private Image _bulletViewPrefab;
        [SerializeField] private Transform _bulletParent;
        
        private ShootBehavior _shootBehavior;
        private readonly List<GameObject> _bulletsView = new List<GameObject>();
        
        public void Init(ShootBehavior shootBehavior)
        {
            _shootBehavior = shootBehavior;
            _shootBehavior.CurrentAmmoChanged = CurrentAmmoChanged;

            for (int i = 0; i < _shootBehavior.MaxAmmo; i++)
            {
                var bullet = Instantiate(_bulletViewPrefab, _bulletParent);
                _bulletsView.Add(bullet.gameObject);
            }
        }

        private void CurrentAmmoChanged()
        {
            int index = _shootBehavior.CurrentAmmo;

            foreach (var bullet in _bulletsView)
            {
                bullet.gameObject.SetActive(index > 0);
                index--;
            }
        }

        private void OnDisable()
        {
            _shootBehavior.CurrentAmmoChanged -= CurrentAmmoChanged;
        }
    }
}
