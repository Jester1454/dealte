using System;
using UnityEngine;

namespace Player.Behaviours.AttackSystem
{
	[Serializable]
	public struct AttackData
	{
		[SerializeField] public float _damage;
		[SerializeField] public SegmentHitBox _hitBox;
		[SerializeField] public string _animatorKey;
		[SerializeField] public float _idleDuration;

		[Header("VFX params")] 
		[SerializeField] public bool _needVfx;
		[SerializeField] public GameObject _vfxObject;
		[SerializeField] public Vector3 _vfxPositionOffset;
		[SerializeField] public Vector3 _vfxRotationOffset;
		[SerializeField] public float _vfxDuration;
		[SerializeField] public float _vfxPlayDelay;

		[Header("Camera Shaker params")] 
		[SerializeField] public bool _needShake;
		[SerializeField] public float _shakeDuration;
		[SerializeField] public float _amplitude;
		[SerializeField] public float _frequency;
		
		[Header("Debug")] 
		[SerializeField] public Color _debugColor;
		
		public bool NeedVfx => _needVfx;
		public Vector3 VfxRotationOffset => _vfxRotationOffset;
		public Vector3 VfxPositionOffset => _vfxPositionOffset;
		public GameObject VfxObject => _vfxObject;
		public float VfxDuration => _vfxDuration;
		public float VfxPlayDelay => _vfxPlayDelay;
		public string AnimatorKey => _animatorKey;
		public SegmentHitBox HitBox => _hitBox;
		public float IdleDuration => _idleDuration;
		public float Damage => _damage;
		public float ShakeDuration => _shakeDuration;
		public bool NeedShake => _needShake;
		public float Amplitude => _amplitude;
		public float Frequency => _frequency;
		public Color DebugColor => _debugColor;
	}
}