using System;
using UnityEngine;
using Utils;

namespace Enemy.EnemyBehaviours.SensorySystems
{
	[Serializable]
	public class VisionSystem
	{
		[SerializeField] private float _distance;
		[SerializeField] private float _height;
		[SerializeField] private float _angle;
		[SerializeField] private LayerMask _obstacleLayers;
		[SerializeField] private LayerMask _visibilityLayers;
		
		[SerializeField] private Color _debugSenseColor;

		public float Distance => _distance;
		public LayerMask VisibilityLayers => _visibilityLayers;
		public Color DebugSenseColor => _debugSenseColor;

		public bool IsInSight(Transform lookingTransform, Vector3 target)
		{
			var direction = target - lookingTransform.position;

			if (target.y > lookingTransform.position.y + _height)
			{
				return false;
			}

			direction.y = 0;
			var deltaAngle = Vector3.Angle(direction, lookingTransform.forward);

			if (deltaAngle > _angle)
			{
				return false;
			}

			var distance = Vector3.Distance(lookingTransform.position, target);

			if (distance > _distance)
			{
				return false;
			}

			if (Physics.Linecast(lookingTransform.position, target, _obstacleLayers))
			{
				return false;
			}

			return true;
		}

		public Mesh CreateSegmentMesh()
		{
			return GeometryUtils.CreateSegmentMesh(_distance, _height, _angle);
		}
	}
}