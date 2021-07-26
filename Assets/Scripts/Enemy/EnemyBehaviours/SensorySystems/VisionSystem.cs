using System;
using UnityEngine;

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

		public Mesh CreateVisibilityMesh()
		{
			var wedgeMesh = new Mesh();

			var segments = 10;
			var trianglesCount = (segments * 4) + 2 + 2;
			var verticesCount = trianglesCount * 3;
			var vertices = new Vector3[verticesCount];
			var triangles = new int[verticesCount];

			var bottomCenter = Vector3.zero;
			var bottomLeft = Quaternion.Euler(0, -_angle, 0) * Vector3.forward * _distance;
			var bottomRight = Quaternion.Euler(0, _angle, 0) * Vector3.forward * _distance;


			var topCenter = bottomCenter + Vector3.up * _height;
			var topLeft = bottomLeft + Vector3.up * _height;
			var topRight = bottomRight + Vector3.up * _height;

			var currentVerticesIndex = 0;
			
			//left side 
			vertices[currentVerticesIndex++] = bottomCenter;
			vertices[currentVerticesIndex++] = bottomLeft;
			vertices[currentVerticesIndex++] = topLeft;

			vertices[currentVerticesIndex++] = topLeft;
			vertices[currentVerticesIndex++] = topCenter;
			vertices[currentVerticesIndex++] = bottomCenter;
			
			//right side
			vertices[currentVerticesIndex++] = bottomCenter;
			vertices[currentVerticesIndex++] = topCenter;
			vertices[currentVerticesIndex++] = topRight;

			vertices[currentVerticesIndex++] = topRight;
			vertices[currentVerticesIndex++] = bottomRight;
			vertices[currentVerticesIndex++] = bottomCenter;

			var currentAngle = -_angle;
			var deltaAngle = (_angle * 2) / segments;
			for (int i = 0; i < segments; i++)
			{
				bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * _distance;
				bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * _distance;

				topLeft = bottomLeft + Vector3.up * _height;
				topRight = bottomRight + Vector3.up * _height;
				
				//far side
				vertices[currentVerticesIndex++] = bottomLeft;
				vertices[currentVerticesIndex++] = bottomRight;
				vertices[currentVerticesIndex++] = topRight;

				vertices[currentVerticesIndex++] = topRight;
				vertices[currentVerticesIndex++] = topLeft;
				vertices[currentVerticesIndex++] = bottomLeft;
			
				//top
				vertices[currentVerticesIndex++] = topCenter;
				vertices[currentVerticesIndex++] = topLeft;
				vertices[currentVerticesIndex++] = topRight;
			
				//bottom
				vertices[currentVerticesIndex++] = bottomCenter;
				vertices[currentVerticesIndex++] = bottomRight;
				vertices[currentVerticesIndex++] = bottomLeft;
				
				currentAngle += deltaAngle;
			}

			for (int i = 0; i < verticesCount; i++)
			{
				triangles[i] = i;
			}

			wedgeMesh.vertices = vertices;
			wedgeMesh.triangles = triangles;
			wedgeMesh.RecalculateNormals();
			
			return wedgeMesh;
		}
	}
}