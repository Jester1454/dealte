using System;
using System.Collections;
using Player.Animations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
	public class ShootBehavior : MonoBehaviour
	{
		[SerializeField] private Animator _animator;
		[SerializeField] private IKHands _ikHands;

		[Header("Bullet")]
		[SerializeField] private Bullet _bulletPrefab;
		[SerializeField] private float _bulletSpeed;
		[SerializeField] private float _bulletRange;
		[SerializeField] private float _timeBetweenShot;
		[SerializeField] private float _startDelay;

		[Header("Weapon")]
		[SerializeField] private Transform _barrel;
		[SerializeField] private float _damage;
		[SerializeField] private Vector2 _scatterX;
		[SerializeField] private Vector2 _scatterY;
		
		[Header("Ammo")]
		[SerializeField] private int _maxAmmo;

		[Header("Camera Shaker for impact")] 
		[SerializeField] private float _shakeDuration;
		[SerializeField] private float _amplitude;
		[SerializeField] private float _frequency;
		
		private int _currentAmmo;
		private bool _canShoot = true;
		private bool _isEnable = false;
		private static readonly int _shootAnimationKey = Animator.StringToHash("IsCast");

		public Action CurrentAmmoChanged { get; set; }
		public Vector3 BarrelPosition => _barrel.position;

		public int CurrentAmmo
		{
			get => _currentAmmo;
			set
			{
				_currentAmmo = value > _maxAmmo ? _maxAmmo : value;
				CurrentAmmoChanged?.Invoke();
			}
		}
		public int MaxAmmo => _maxAmmo;
		public bool IsEnable => _isEnable;

		public bool CanShoot => _canShoot;

		public void Enable()
		{
			_isEnable = true;
		}

		public void Disable()
		{
			_isEnable = false;
		}
		
		private void Start()
		{
			CurrentAmmo = _maxAmmo;
		}

		public IEnumerator Shoot(Vector3 aimPosition, bool isPlayer = false, bool heightConsider = false, Vector3? endPosition = null)
		{
			if (!_isEnable) yield break;
			
			if (!CheckCanShoot(isPlayer))
				yield break;

			if (isPlayer)
			{
				_ikHands.SetIKOff();
				_animator.SetTrigger(_shootAnimationKey);
			}
			
			yield return new WaitForSeconds(_startDelay);
			
			_canShoot = false;
			transform.rotation = Quaternion.LookRotation(new Vector3(aimPosition.x, 0, aimPosition.z), Vector3.up);

			if (endPosition.HasValue)
			{
				aimPosition = endPosition.Value - _barrel.position;
			}

			var bullet = Instantiate(_bulletPrefab, _barrel.position, Quaternion.identity);
			
			if (isPlayer)
			{
				CurrentAmmo--;
			}
			
			aimPosition += new Vector3(Random.Range(_scatterX.x, _scatterX.y), 0, Random.Range(_scatterY.x, _scatterY.y));

			if (!heightConsider)
			{
				aimPosition.y = 0;
			}
			bullet.StartShot(aimPosition.normalized * _bulletSpeed, _damage);
			Destroy(bullet.gameObject, _bulletRange / (aimPosition.normalized.magnitude * _bulletSpeed));
			
			if (isPlayer)
			{
				ImpactShake();
			}
			
			yield return StartCoroutine(ShotCooldown());
		}

		private IEnumerator ShotCooldown()
		{
			yield return new WaitForSeconds(_timeBetweenShot);
			_canShoot = true;

			if (_ikHands != null)
			{
				_ikHands.SetIKOn();
			}
		}

		private bool CheckCanShoot(bool isPlayer)
		{
			if (!_canShoot)
				return false;
			if (isPlayer)
			{
				if (CurrentAmmo <= 0)
					return false;
			}

			return true;
		}

		private void ImpactShake()
		{
			CinemachineCameraShaker.Instance.ShakeCamera(_shakeDuration, _amplitude, _frequency);
		}
		
		private void OnDrawGizmos()
		{
			if (_barrel == null) return;
			
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(_barrel.transform.position, _bulletRange);
		}
	}
}