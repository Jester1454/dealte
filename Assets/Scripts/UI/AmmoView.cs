using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AmmoView : MonoBehaviour
    {
        [SerializeField] private BulletView _bulletViewPrefab;
        [SerializeField] private Transform _bulletParent;
        
        private ShootBehavior _shootBehavior;
        private readonly List<BulletView> _bulletsView = new List<BulletView>();
        
        public void Init(ShootBehavior shootBehavior)
        {
            _shootBehavior = shootBehavior;
            _shootBehavior.CurrentAmmoChanged = CurrentAmmoChanged;

            for (int i = 0; i < _shootBehavior.MaxAmmo; i++)
            {
                var bullet = Instantiate(_bulletViewPrefab, _bulletParent);
                _bulletsView.Add(bullet);
            }
        }

        private void CurrentAmmoChanged()
        {
            int index = _shootBehavior.CurrentAmmo;

            foreach (var bullet in _bulletsView)
            {
                bullet.SetActive(index > 0);
                index--;
            }
        }

        private void OnDisable()
        {
            _shootBehavior.CurrentAmmoChanged -= CurrentAmmoChanged;
        }
    }
}
