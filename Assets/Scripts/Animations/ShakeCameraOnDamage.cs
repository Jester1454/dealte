using System;
using System.Collections.Generic;
using Player.Behaviours.HealthSystem;
using UnityEngine;

[RequireComponent(typeof(HealthBehaviour))]
public class ShakeCameraOnDamage : MonoBehaviour
{
	[SerializeField] private List<ShakeDamageData> _shakeDamageDatas;
	
	private HealthBehaviour _healthBehaviour;

	private void OnEnable()
	{
		_healthBehaviour = GetComponent<HealthBehaviour>();
		_healthBehaviour.OnTakeDamage += OnTakeDamage;
	}

	private void OnTakeDamage(DamageType damageType)
	{
		var data = _shakeDamageDatas.Find(x => x.DamageType == damageType);
		CinemachineCameraShaker.Instance.ShakeCamera(data.ShakeDuration, data.Amplitude, data.Frequency);
	}

	private void OnDisable()
	{
		_healthBehaviour.OnTakeDamage -= OnTakeDamage;
	}
}

[Serializable]
public struct ShakeDamageData
{
	[SerializeField] private DamageType _damageType;
	[SerializeField] private float _shakeDuration;
	[SerializeField] private float _amplitude;
	[SerializeField] private float _frequency;

	public DamageType DamageType => _damageType;
	public float ShakeDuration => _shakeDuration;
	public float Amplitude => _amplitude;
	public float Frequency => _frequency;
}
