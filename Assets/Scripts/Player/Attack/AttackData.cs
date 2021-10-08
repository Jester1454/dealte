using System;
using UnityEngine;

namespace Player.Behaviours.AttackSystem
{
	[Serializable]
	public struct AttackData
	{
		[SerializeField] private float _damage;
		[SerializeField] private SegmentHitBox _hitBox;
		[SerializeField] private string _animatorKey;
		[SerializeField] private float _idleDuration;
		
		[Header("VFX params")]
		[SerializeField] private GameObject _vfxObject;
		[SerializeField] private Vector3 _vfxPositionOffset;
		[SerializeField] private Vector3 _vfxRotationOffset;
		[SerializeField] private float _vfxDuration;
		[SerializeField] private float _vfxPlayDelay;
		
		[Header("Camera Shaker params")] 
		[SerializeField] private float _shakeDuration;
		[SerializeField] private float _amplitude;
		[SerializeField] private float _frequency;

		[Header("Debug")] 
		[SerializeField] private Color _debugColor;
		
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
		public float Amplitude => _amplitude;
		public float Frequency => _frequency;
		public Color DebugColor => _debugColor;
	}
}