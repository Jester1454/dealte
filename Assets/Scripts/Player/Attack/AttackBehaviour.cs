using System;
using System.Collections;
using System.Collections.Generic;
using Player.Behaviours.HealthSystem;
using RPGCharacterAnimsFREE;
using UnityEngine;

namespace Player.Behaviours.AttackSystem
{
	public class AttackBehaviour
	{
		private readonly MonoBehaviour _mono;
		private readonly AttackData _attackData;
		private readonly List<IGettingDamage> _filterObject = new List<IGettingDamage>();
		private bool _hitBoxEnable;
		private bool _isProcessAttack;
		private bool _isFinishAttack;
		private readonly IGettingDamage _thisGettingDamage;
		private readonly AnimatorEvents _animatorEvents;
		public AttackData AttackData => _attackData;
		public bool IsProcessAttack => _isProcessAttack;
		public bool IsFinishAttack => _isFinishAttack;
		public Action OnFinish;
		
		public AttackBehaviour(MonoBehaviour mono, AttackData attackData, IGettingDamage thisGettingDamage, AnimatorEvents animatorEvents)
		{
			_mono = mono;
			_attackData = attackData;
			_thisGettingDamage = thisGettingDamage;
			_animatorEvents = animatorEvents;
			_animatorEvents.OnFinishAttack += OnFinishAttack;
		}
		
		public void Start(Transform transform)
		{
			_isProcessAttack = true;
			_isFinishAttack = false;
			_animatorEvents.OnHit += OnHit;
			_animatorEvents.OnStartAttack += OnEventStartAttack;
			_filterObject.Clear();
			
			_mono.StopCoroutine(PlayVFX(transform));
			if (_attackData.NeedVfx)
			{
				_mono.StartCoroutine(PlayVFX(transform));
			}
		
			if (_attackData.NeedShake)
			{
				CinemachineCameraShaker.Instance.ShakeCamera(_attackData.ShakeDuration, _attackData.Amplitude, _attackData.Frequency);
			}
		}

		private void OnFinishAttack()
		{
			_isFinishAttack = true;
		}

		private void OnHit()
		{
			_isProcessAttack = false;
			_hitBoxEnable = false;
			_animatorEvents.OnHit -= OnHit;
			_animatorEvents.OnStartAttack -= OnEventStartAttack;
			OnFinish?.Invoke();
		}

		private void OnEventStartAttack()
		{
			_hitBoxEnable = true;
		}

		public void Stop()
		{
			_isProcessAttack = false;
			_hitBoxEnable = false;
		}

		public int OnUpdate(Transform transform)
		{
			if (!_hitBoxEnable) return 0;

			var inHitBox = _attackData.HitBox.GetHits(transform.position, transform.forward);

			var hintCount = 0;
			foreach (var hit in inHitBox)
			{
				var isHit = Damage(hit);
				if (isHit) hintCount++;
			}

			return hintCount;
		}
		
		private bool Damage(GameObject hitObject)
		{
			var gettingDamage = hitObject.GetComponent<IGettingDamage>();
			
			if (gettingDamage != null && gettingDamage != _thisGettingDamage && !_filterObject.Contains(gettingDamage) && gettingDamage.CurrentHealth > 0
			    && gettingDamage.Side != _thisGettingDamage.Side)
			{
				gettingDamage.Damage(_attackData.Damage, DamageType.Melee, _mono.transform.position);
				_filterObject.Add(gettingDamage);
				return true;
			}

			return false;
		}

		private IEnumerator PlayVFX(Transform transform)
		{
			if (_attackData.VfxObject == null) yield break;

			yield return new WaitForSeconds(_attackData.VfxPlayDelay);

			var vfxObject = GameObject.Instantiate(_attackData.VfxObject, _attackData.VfxPositionOffset, Quaternion.Euler(transform.rotation.eulerAngles + _attackData.VfxRotationOffset), transform);
			var particleSystem = vfxObject.GetComponentInChildren<ParticleSystem>();
			
			var duration = particleSystem != null ? particleSystem.main.duration : _attackData.VfxDuration;

			while (duration > 0)
			{
				duration -= Time.deltaTime;
				vfxObject.transform.position = transform.position + _attackData.VfxPositionOffset;
				vfxObject.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + _attackData.VfxRotationOffset);
				yield return null;
			}
			
			GameObject.Destroy(vfxObject);
		}
	}
}