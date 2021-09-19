using System;
using System.Collections;
using Player.Behaviours.HealthSystem;
using RPGCharacterAnimsFREE;
using UnityEngine;

namespace Player.Behaviours.AttackSystem
{
	public class AttackBehaviour : MonoBehaviour
	{
		[SerializeField] private float _damage;
		[SerializeField] private AttackCollider[] _attackCollider;
		[SerializeField] private Animator _animator;
		[SerializeField] private AnimatorEvents _animatorEvents;
		
		[Header("VFX params")]
		[SerializeField] private GameObject _vfxObject;
		[SerializeField] private Vector3 _vfxPositionOffset;
		[SerializeField] private Vector3 _vfxRotationOffset;
		
		[Header("Camera Shaker params")] 
		[SerializeField] protected float _shakeDuration;
		[SerializeField] protected float _amplitude;
		[SerializeField] protected float _frequency;
		
		public Action OnFinish;
		
		private static readonly int _attack = Animator.StringToHash("Attack");
		private static readonly int _attackTye = Animator.StringToHash("AttackType");
		
		private bool _isAttack = false;
		private bool _isEnable = false;
		
		private IGettingDamage _thisGettingDamage;
		private int _currentAttackType = 1;
		
		private void OnEnable()
		{
			_animatorEvents.OnHit += OnHit;
			_thisGettingDamage = GetComponent<IGettingDamage>();
			foreach (var attackCollider in _attackCollider)
			{
				attackCollider.OnDamage += OnDamage;
			}
		}

		private void OnHit()
		{
			SetActiveAttack(false);
			
			_currentAttackType++;
			if (_currentAttackType > 3)
			{
				_currentAttackType = 1;
			}
			
			OnFinish?.Invoke();
		}

		private void OnDamage(IGettingDamage gettingDamage)
		{
			if (_isAttack && gettingDamage != _thisGettingDamage)
			{
				gettingDamage.Damage(_damage);
			}
		}

		private void SetActiveAttack(bool value)
		{
			foreach (var attackCollider in _attackCollider)
			{
				attackCollider.SetActive(value);
			}
			_isAttack = value;
		}

		private IEnumerator PlayVFX()
		{
			var vfxObject = Instantiate(_vfxObject, _vfxPositionOffset, Quaternion.Euler(_vfxRotationOffset), transform);
			var duration = vfxObject.GetComponentInChildren<ParticleSystem>().main.duration;

			while (duration > 0)
			{
				duration -= Time.deltaTime;
				vfxObject.transform.position = transform.position + _vfxPositionOffset;
				vfxObject.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + _vfxRotationOffset);
				yield return null;
			}
			
			Destroy(vfxObject);
		}

		public void Attack()
		{
			if (_isAttack || !_isEnable)
			{
				return;
			}
			_animator.SetInteger(_attackTye, _currentAttackType);
			_animator.SetTrigger(_attack);
			StartCoroutine(PlayVFX());
			CinemachineCameraShaker.Instance.ShakeCamera(_shakeDuration, _amplitude, _frequency);
			SetActiveAttack(true);
		}

		private void OnDisable()
		{
			_animatorEvents.OnHit -= OnHit;

			foreach (var attackCollider in _attackCollider)
			{
				attackCollider.OnDamage -= OnDamage;
			}
		}

		public void Enable()
		{
			_isEnable = true;
		}

		public void Disable()
		{
			_isEnable = false;
		}

		public bool IsEnable => _isEnable;
	}
}