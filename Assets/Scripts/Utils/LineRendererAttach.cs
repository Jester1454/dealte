using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
	public class LineRendererAttach : MonoBehaviour
	{
		[SerializeField] private LineRenderer _lineRenderer;
		[SerializeField] private List<Transform> _attachPoints;
	
		private void Awake()
		{
			_lineRenderer.positionCount = _attachPoints.Count;
		}
	
		private void UpdatePositions()
		{
			for (var i = 0; i < _attachPoints.Count; i++)
			{
				_lineRenderer.SetPosition(i, _attachPoints[i].position);
			}
		}

		private void Update()
		{
			UpdatePositions();
		}
	}
}