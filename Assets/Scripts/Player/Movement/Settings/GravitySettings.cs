namespace Player.Movement.Settings
{
	[System.Serializable]
	public class GravitySettings
	{
		public float Gravity = 20.0f; // Gravity applied when the player is airborne
		public float GroundedGravity = 5.0f; // A constant gravity that is applied when the player is grounded
		public float MaxFallSpeed = 40.0f; // The max speed at which the player can fall
	}
}