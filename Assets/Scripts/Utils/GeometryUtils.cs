using UnityEngine;

namespace Utils
{
	public static class GeometryUtils
	{
		public static Mesh CreateSegmentMesh(float distance, float height, float angle)
		{
			var wedgeMesh = new Mesh();

			var segments = 10;
			var trianglesCount = (segments * 4) + 2 + 2;
			var verticesCount = trianglesCount * 3;
			var vertices = new Vector3[verticesCount];
			var triangles = new int[verticesCount];

			var bottomCenter = Vector3.zero;
			var bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
			var bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;


			var topCenter = bottomCenter + Vector3.up * height;
			var topLeft = bottomLeft + Vector3.up * height;
			var topRight = bottomRight + Vector3.up * height;

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

			var currentAngle = -angle;
			var deltaAngle = (angle * 2) / segments;
			for (int i = 0; i < segments; i++)
			{
				bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
				bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

				topLeft = bottomLeft + Vector3.up * height;
				topRight = bottomRight + Vector3.up * height;
				
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