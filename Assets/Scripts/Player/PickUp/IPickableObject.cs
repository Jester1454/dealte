using UnityEngine;

namespace Player.PickUp
{
	public interface IPickableObject
	{
		Transform Transform { get; }
		void PickUp();
		void SetPickUpStatus(bool value);
		bool IsActive { get; }
	}
}