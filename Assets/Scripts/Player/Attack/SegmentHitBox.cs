using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Player.Behaviours.AttackSystem
{
	[Serializable]
	public class SegmentHitBox
	{
		[SerializeField] private float _range;
		[SerializeField] private LayerMask _visibilityLayers;
		[SerializeField] private float _angle;

		private readonly Collider[] _colliders = new Collider[50];

		public List<GameObject> GetHits(Vector3 position, Vector3 direction)
		{
			Physics.OverlapSphereNonAlloc(position, _range, _colliders, _visibilityLayers);
			var hitObjects = new List<GameObject>();
			DebugDraw(position, direction);

			foreach (var otherCollider in _colliders)
			{
				if (otherCollider == null) continue;
				
				if (IsInHitBox(position, direction, otherCollider.transform.position))
				{
					hitObjects.Add(otherCollider.gameObject);
				}
			}

			return hitObjects;
		}
		
		public bool IsInHitBox(Vector3 position, Vector3 forward, Vector3 target)
		{
			var direction = target - position;

			direction.y = 0;
			forward.y = 0;
			var deltaAngle = Vector3.Angle(direction, forward);

			if (deltaAngle > _angle)
			{
				return false;
			}

			var distance = Vector3.Distance(position, target);

			if (distance > _range)
			{
				return false;
			}

			return true;
		}
		
		public Mesh CreateSegmentMesh()
		{
			return GeometryUtils.CreateSegmentMesh(_range, 1f, _angle);
		}
		
		public void DebugDraw(Vector3 position, Vector3 direction)
		{
			var directionVector2 = new Vector2(direction.x, direction.z);
			var viewAngleA = DirFromAngle(-_angle / 2, directionVector2);
			var viewAngleB = DirFromAngle(_angle / 2, directionVector2);

			Debug.DrawLine(position, position + viewAngleA * _range, Color.red);
			Debug.DrawLine(position, position + viewAngleB * _range, Color.red);
		}
		
		private static float Angle(Vector2 value)
		{
			if (value.x < 0)
			{
				return 360 - (Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg * -1);
			}
			else
			{
				return Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg;
			}
		}
		
		private static Vector3 DirFromAngle(float angleInDegrees, Vector2 direction)
		{
			var currentDir = Angle(direction);
			angleInDegrees += currentDir;
			return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
		}
	}
}