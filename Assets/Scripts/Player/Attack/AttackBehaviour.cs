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
		[SerializeField] private Animator _animator;
		[SerializeField] private AnimatorEvents _animatorEvents;
		[SerializeField] private List<AttackData> _attacksData;
		
		[Header("Camera Shaker params")] 
		[SerializeField] protected float _shakeDuration;
		[SerializeField] protected float _amplitude;
		[SerializeField] protected float _frequency;
		
		public Action OnFinish;
		
		private bool _isAttack = false;
		private bool _isEnable = false;
		private bool _hitBoxEnable = false;
		private IGettingDamage _thisGettingDamage;
		private readonly List<IGettingDamage> _filterObject = new List<IGettingDamage>();
		private int _currentAttack = 0;
		private AttackData _currentAttackData;
		private bool _playNextAttack = false;
		
		private void OnEnable()
		{
			_animatorEvents.OnHit += OnHit;
			_animatorEvents.OnStartAttack += OnEventStartAttack;
			_thisGettingDamage = GetComponent<IGettingDamage>();
		}

		private void OnHit()
		{
			if (_playNextAttack && _currentAttack + 1 < _attacksData.Count)
			{
				UpdateCurrentAttack();
				StarAttack();
			}
			else
			{
				_playNextAttack = false;
				_isAttack = false;
				_hitBoxEnable = false;
				OnFinish?.Invoke();	
			}
		}

		private void Damage(GameObject hitObject)
		{
			var gettingDamage = hitObject.GetComponent<IGettingDamage>();
			
			if (gettingDamage != null && _isAttack && gettingDamage != _thisGettingDamage && !_filterObject.Contains(gettingDamage))
			{
				gettingDamage.Damage(_currentAttackData.Damage);
				_filterObject.Add(gettingDamage);
			}
		}

		private void FixedUpdate()
		{
			if (!_hitBoxEnable) return;

			var inHitBox = _currentAttackData.HitBox.GetHits(transform.position, transform.forward);

			foreach (var hit in inHitBox)
			{
				Damage(hit);
			}
		}

		private IEnumerator PlayVFX()
		{
			if (_currentAttackData.VfxObject == null) yield break;

			var vfxObject = Instantiate(_currentAttackData.VfxObject, _currentAttackData.VfxPositionOffset, Quaternion.Euler(_currentAttackData.VfxRotationOffset), transform);
			var duration = vfxObject.GetComponentInChildren<ParticleSystem>().main.duration;

			while (duration > 0)
			{
				duration -= Time.deltaTime;
				vfxObject.transform.position = transform.position + _currentAttackData.VfxPositionOffset;
				vfxObject.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + _currentAttackData.VfxRotationOffset);
				yield return null;
			}
			
			Destroy(vfxObject);
		}

		public void Attack()
		{
			if (!_isEnable)
			{
				return;
			}
			
			if (_isAttack)
			{
				_playNextAttack = true;
				return;
			}

			_playNextAttack = false;
			_currentAttack = 0;
			_currentAttackData = _attacksData[_currentAttack];
			StarAttack();
		}

		private void StarAttack()
		{
			_filterObject.Clear();
			_animator.SetTrigger(_currentAttackData.AnimatorKey);
			StartCoroutine(PlayVFX());
			CinemachineCameraShaker.Instance.ShakeCamera(_shakeDuration, _amplitude, _frequency);
			_isAttack = true;
		}

		private void UpdateCurrentAttack()
		{
			_currentAttack++;
			_currentAttackData = _attacksData[_currentAttack];
			if (_currentAttack >= _attacksData.Count)
			{
				_currentAttack = 0;
			}
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
		
		// private void OnValidate()
		// {
		// 	_visionDebugMesh = _hitBox.CreateSegmentMesh();
		// }
		//
		// private void OnDrawGizmos()
		// {
		// 	if (_hitBox != null)
		// 	{
		// 		Gizmos.color = Color.red;
		// 		Gizmos.DrawWireMesh(_visionDebugMesh, transform.position, transform.rotation);
		// 	}
		// }
	}
	
	[Serializable]
	public struct AttackData
	{
		[SerializeField] private float _damage;
		[SerializeField] private SegmentHitBox _hitBox;
		[SerializeField] private string _animatorKey;
		
		[Header("VFX params")]
		[SerializeField] private GameObject _vfxObject;
		[SerializeField] private Vector3 _vfxPositionOffset;
		[SerializeField] private Vector3 _vfxRotationOffset;

		public Vector3 VfxRotationOffset => _vfxRotationOffset;
		public Vector3 VfxPositionOffset => _vfxPositionOffset;
		public GameObject VfxObject => _vfxObject;
		public string AnimatorKey => _animatorKey;
		public SegmentHitBox HitBox => _hitBox;
		public float Damage => _damage;
	}
}