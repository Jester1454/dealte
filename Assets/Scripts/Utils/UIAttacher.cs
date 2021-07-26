using UnityEngine;

namespace UI.HUD
{
	public class UIAttacher : MonoBehaviour
	{
		[SerializeField] private Vector3 _positionOffset = Vector3.up;
		[SerializeField] private Transform _target;
        
		private void LateUpdate()
		{
			// transform.position = Camera.main.WorldToScreenPoint(_target.position + _positionOffset);
			transform.LookAt(Camera.main.transform);
			transform.Rotate(0, 180, 0);
		}
	}
}