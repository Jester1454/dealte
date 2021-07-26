using System.Collections.Generic;
using System.Linq;
using Player.Behaviours.AttackSystem;
using UnityEngine;

namespace Player.PickUp
{
	public class PickUpBehaviour : MonoBehaviour
	{
		[SerializeField] private ThrowBehaviour _throwBehaviour;
		[SerializeField] private List<Transform> _pickUpContainters;
		
		private List<IPickableObject> _pickableObjects = new List<IPickableObject>();
		
		private void OnTriggerEnter(Collider other)
		{
			var pickableObject = other.GetComponent<IPickableObject>();

			if (pickableObject != null)
			{
				_pickableObjects.Add(pickableObject);
			}
		}
    
		private void OnTriggerExit(Collider other)
		{
			var pickableObject = other.GetComponent<IPickableObject>();

			if (_pickableObjects.Contains(pickableObject))
			{
				_pickableObjects.Remove(pickableObject);
			}
		}

		public void PickUp()
		{
			if (_pickableObjects.Count == 0) 
				return;

			var pickableObject = _pickableObjects.FirstOrDefault(pick => pick.IsActive);
			if (pickableObject == null)
				return;

			pickableObject.PickUp();
			foreach (var containter in _pickUpContainters)
			{
				if (containter.childCount > 0) continue;
				
				pickableObject.Transform.SetParent(containter, true);
				pickableObject.Transform.position = containter.position;
				pickableObject.Transform.rotation = containter.rotation;
			}
			
			_throwBehaviour.AddThrowingObject(pickableObject);
			_pickableObjects.Remove(pickableObject);
		}
	}
}