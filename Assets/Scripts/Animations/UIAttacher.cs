using UnityEngine;

namespace UI.HUD
{
	public class UIAttacher : MonoBehaviour
	{
		private void LateUpdate()
		{
			transform.LookAt(Camera.main.transform);
			transform.Rotate(0, 180, 0);
		}
	}
}