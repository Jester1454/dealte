using UnityEngine;

namespace Player.PickUp
{
	public interface IPickableMedKit
	{
		public void PickUp();
	}
	
	public class PickableMedKit : MonoBehaviour, IPickableMedKit
	{
		[SerializeField] private GameObject _uiObject;

		public void PickUp()
		{
			Destroy(gameObject);
			//Add animation ???
		}
		
		private void OnTriggerEnter(Collider other)
		{
			if (other.GetComponent<CharacterBehaviour>())
			{
				if (_uiObject != null)
				{
					_uiObject.SetActive(true);
				}
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (other.GetComponent<CharacterBehaviour>())
			{
				if (_uiObject != null)
				{
					_uiObject.SetActive(true);
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.GetComponent<CharacterBehaviour>())
			{
				if (_uiObject != null)
				{
					_uiObject.SetActive(false);
				}
			}
		}
	}
}