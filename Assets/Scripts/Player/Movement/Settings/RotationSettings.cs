using UnityEngine;

namespace Player.Movement.Settings
{
	[System.Serializable]
	public class RotationSettings
	{
		[Header("Control Rotation")]
		public float MinPitchAngle = -45.0f;
		public float MaxPitchAngle = 75.0f;

		[Header("Character Orientation")]
		[SerializeField] private bool _useControlRotation = false;
		[SerializeField] private bool _orientRotationToMovement = true;
		public float MinRotationSpeed = 600.0f; // The turn speed when the player is at max speed (in degrees/second)
		public float MaxRotationSpeed = 1200.0f; // The turn speed when the player is stationary (in degrees/second)

		public bool UseControlRotation { get { return _useControlRotation; } set { SetUseControlRotation(value); } }
		public bool OrientRotationToMovement { get { return _orientRotationToMovement; } set { SetOrientRotationToMovement(value); } }

		private void SetUseControlRotation(bool useControlRotation)
		{
			_useControlRotation = useControlRotation;
			_orientRotationToMovement = !_useControlRotation;
		}

		private void SetOrientRotationToMovement(bool orientRotationToMovement)
		{
			_orientRotationToMovement = orientRotationToMovement;
			_useControlRotation = !_orientRotationToMovement;
		}
	}
}