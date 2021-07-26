using UnityEngine;

namespace Player.PickUp
{
	public interface IPickableObject
	{
		Transform Transform { get; }
		void PickUp();
		void SetActive(bool value);
		bool IsActive { get; }
	}
}