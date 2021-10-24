using System;
using Player.Behaviours.HealthSystem;
using UnityEngine;

namespace Enemy.EnemyBehaviours.SensorySystems
{
	public class SensorySystem : MonoBehaviour
	{
		[SerializeField] private VisionSystem _visionSystem;
		[SerializeField] private float _scanInterval = 0.03f;
		[SerializeField] private HealthBehaviour _healthBehaviour;
		
		private Mesh _visionDebugMesh;
		private Mesh _suspicionVisionDebugMesh;
		private float _scanTimer;
		private bool _targetIsSearched;

		public GameObject VisibleTarget { get; private set; }
		private readonly Collider[] _colliders = new Collider[50];

		private void OnEnable()
		{
			if (_healthBehaviour != null)
			{
				_healthBehaviour.OnTakeDamage += OnTakeDamage;
			}
		}

		private void OnDisable()
		{
			if (_healthBehaviour != null)
			{
				_healthBehaviour.OnTakeDamage -= OnTakeDamage;
			}
		}

		private void OnTakeDamage(DamageType damageType)
		{
			Scan(true);
		}

		private void Update()
		{
			_scanTimer -= Time.deltaTime;

			if (_scanTimer < 0)
			{
				_scanTimer = _scanInterval;
				Scan();
			}
		}

		private void Scan(bool inSight = false)
		{
			if (_targetIsSearched)
				return;
			
			var target = Scan(_visionSystem, inSight);

			if (target != null)
			{
				_targetIsSearched = true;
				VisibleTarget = target;
			}
		}

		private GameObject Scan(VisionSystem visionSystem, bool inSight = false)
		{
			Physics.OverlapSphereNonAlloc(transform.position, visionSystem.Distance, _colliders, visionSystem.VisibilityLayers);

			foreach (var otherCollider in _colliders)
			{
				if (otherCollider == null) continue;
				
				if (!inSight && visionSystem.IsInSight(transform, otherCollider.transform.position))
				{
					return otherCollider.gameObject;
				}

				return otherCollider.gameObject;
			}

			return null;
		}

		private void OnValidate()
		{
			_visionDebugMesh = _visionSystem.CreateSegmentMesh();
		}

		private void OnDrawGizmos()
		{
			if (_visionDebugMesh != null)
			{
				Gizmos.color = _visionSystem.DebugSenseColor;
				Gizmos.DrawMesh(_visionDebugMesh, transform.position, transform.rotation);
			}
		}
	}
}