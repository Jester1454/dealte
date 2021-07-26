using System;
using System.Collections;
using System.Linq;
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
		[SerializeField] private GameObject _vfxObject;
		[SerializeField] private Vector3 _vfxPositionOffset;
		[SerializeField] private Vector3 _vfxRotationOffset;
		public Action OnFinish;
		
		private static readonly int _attack = Animator.StringToHash("Attack");
		private static readonly int _attackTye = Animator.StringToHash("AttackType");
		
		private bool _isAttack = false;
		private bool _isDisable = false;
		
		private IGettingDamage _thisGettingDamage;
		private int _currentAttackType = 1;
		
		private void OnEnable()
		{
			_animatorEvents.OnFinishAttack += OnFinishAttack;
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
		}

		private void OnDamage(IGettingDamage gettingDamage)
		{
			if (_isAttack && gettingDamage != _thisGettingDamage)
			{
				gettingDamage.Damage(_damage);
			}
		}

		private void OnFinishAttack()
		{
			_animator.applyRootMotion = false;
			
			_currentAttackType++;
			if (_currentAttackType > 3)
			{
				_currentAttackType = 1;
			}
			
			OnFinish?.Invoke();
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
			if (_isAttack || _isDisable)
			{
				return;
			}
			_animator.SetInteger(_attackTye, _currentAttackType);
			_animator.SetTrigger(_attack);
			_animator.applyRootMotion = true;
			StartCoroutine(PlayVFX());

			SetActiveAttack(true);
		}

		private void OnDisable()
		{
			_animatorEvents.OnFinishAttack -= OnFinishAttack;
			_animatorEvents.OnHit -= OnHit;

			foreach (var attackCollider in _attackCollider)
			{
				attackCollider.OnDamage -= OnDamage;
			}
		}

		public void Disable()
		{
			_isDisable = true;
		}
	}
}