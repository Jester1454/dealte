using Player.Behaviours.HealthSystem;
using UnityEngine;

[RequireComponent(typeof(HealthBehaviour))]
public class ShakeCameraOnDamage : MonoBehaviour
{
	[SerializeField] protected float _shakeDuration;
	[SerializeField] protected float _amplitude;
	[SerializeField] protected float _frequency;
	
	private HealthBehaviour _healthBehaviour;

	private void OnEnable()
	{
		_healthBehaviour = GetComponent<HealthBehaviour>();
		_healthBehaviour.OnTakeDamage += OnTakeDamage;
	}

	private void OnTakeDamage()
	{
		CinemachineCameraShaker.Instance.ShakeCamera(_shakeDuration, _amplitude, _frequency);
	}

	private void OnDisable()
	{
		_healthBehaviour.OnTakeDamage -= OnTakeDamage;
	}
}
