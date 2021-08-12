using System;
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
		private bool _isDisable;

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
			if (_pickableObjects.Count == 0 || _isDisable) 
				return;

			var pickableObject = _pickableObjects.FirstOrDefault(pick => pick.IsActive);
			if (pickableObject == null)
				return;

			pickableObject.PickUp();

			var container = _pickUpContainters.FirstOrDefault(cont => !CheckContainerFull(cont));
			if (container == null)
				return;
			
			pickableObject.Transform.SetParent(container, true);
			pickableObject.Transform.position = container.position;
			pickableObject.Transform.rotation = container.rotation;
			
			_throwBehaviour.AddThrowingObject(pickableObject);
			_pickableObjects.Remove(pickableObject);
		}
		
		private bool CheckContainerFull(Transform container)
		{
			foreach (Transform child in container)
			{
				if (child.gameObject.activeSelf)
					return true;
			}

			return false;
		}
		
		public void Disable()
		{
			_isDisable = true;
		}
		
		public void Enable()
		{
			_isDisable = false;
		}
	}
}