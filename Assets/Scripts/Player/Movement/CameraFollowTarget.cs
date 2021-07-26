using UnityEngine;

namespace Movement
{
	public class CameraFollowTarget : MonoBehaviour
	{
		[SerializeField] private float _rotationSpeed = 25f;
		[SerializeField] private Vector3 _offset;

		public void SetPosition(Vector3 position)
		{
			transform.position = _offset + position;
		}

		public void SetControlRotation(Vector2 controlRotation)
		{
			Quaternion pivotTargetLocalRotation = Quaternion.Euler(controlRotation.x, controlRotation.y, 0.0f);

			transform.localRotation = _rotationSpeed > 0.0f
				? Quaternion.Slerp(transform.localRotation, pivotTargetLocalRotation, _rotationSpeed * Time.deltaTime)
				: pivotTargetLocalRotation;
		}
	}
}