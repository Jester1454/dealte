namespace Player.Movement.Settings
{
	[System.Serializable]
	public class MovementSettings
	{
		public float Acceleration = 25.0f; // In meters/second
		public float Decceleration = 25.0f; // In meters/second
		public float MaxHorizontalSpeed = 8.0f; // In meters/second
		public float MaxWalkSpeed = 4.0f;
		public bool IsometricMovement;
	}
}