using System;
using System.Collections;
using System.Collections.Generic;
using Player.Behaviours.HealthSystem;
using RPGCharacterAnimsFREE;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Player.Behaviours.AttackSystem
{
	public class AttackBehaviour : MonoBehaviour
	{
		[SerializeField] private Transform _attachSword;
		[SerializeField] private float _damage;
		[SerializeField] private Animator _animator;
		[SerializeField] private AnimatorEvents _animatorEvents;
		[SerializeField] private SegmentHitBox _hitBox;
		
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
		private static readonly int _attackType = Animator.StringToHash("AttackType");
		
		private bool _isAttack = false;
		private bool _isEnable = false;
		private bool _hitBoxEnable = false;
		private IGettingDamage _thisGettingDamage;
		private int _currentAttackType = 1;
		private Mesh _visionDebugMesh;
		private List<IGettingDamage> _filterObject = new List<IGettingDamage>();
	
		private void OnEnable()
		{
			_animatorEvents.OnHit += OnHit;
			_animatorEvents.OnStartAttack += OnEventStartAttack;
			_thisGettingDamage = GetComponent<IGettingDamage>();
		}

		private void OnHit()
		{
			_isAttack = false;
			_hitBoxEnable = false;
			
			_currentAttackType++;
			if (_currentAttackType > 2)
			{
				_currentAttackType = 1;
			}
			
			OnFinish?.Invoke();
		}

		private void Damage(GameObject hitObject)
		{
			var gettingDamage = hitObject.GetComponent<IGettingDamage>();
			
			if (gettingDamage != null && _isAttack && gettingDamage != _thisGettingDamage && !_filterObject.Contains(gettingDamage))
			{
				gettingDamage.Damage(_damage);
				_filterObject.Add(gettingDamage);
			}
		}

		private void FixedUpdate()
		{
			if (!_hitBoxEnable) return;

			var inHitBox = _hitBox.GetHits(transform.position, transform.forward);

			foreach (var hit in inHitBox)
			{
				Damage(hit);
			}
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
			
			_filterObject.Clear();
			_animator.SetInteger(_attackType, _currentAttackType);
			_animator.SetTrigger(_attack);
			StartCoroutine(PlayVFX());
			CinemachineCameraShaker.Instance.ShakeCamera(_shakeDuration, _amplitude, _frequency);
			_isAttack = true;
		}

		private void OnEventStartAttack()
		{
			_hitBoxEnable = true;
		}

		private void OnDisable()
		{
			_animatorEvents.OnHit -= OnHit;
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
		
		private void OnValidate()
		{
			_visionDebugMesh = _hitBox.CreateSegmentMesh();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(_attachSword.TransformPoint(Vector3.zero), 0.5f);
			Gizmos.DrawLine(transform.position, _attachSword.TransformPoint(Vector3.zero));

			if (_hitBox != null)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireMesh(_visionDebugMesh, transform.position, transform.rotation);
			}
		}
	}
}