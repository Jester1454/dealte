using System.Collections;
using Animations;
using UnityEngine;

namespace Player.PickUp
{
	public interface IPickableMedKit
	{
		void PickUp();
	}
	
	public class PickableMedKit : MonoBehaviour, IPickableMedKit
	{
		[SerializeField] private GameObject _uiObject;
		[SerializeField] private MaterialPropertyChangeAnimation _animation;
		
		public void PickUp()
		{
			StartCoroutine(Animation());
		}

		private IEnumerator Animation()
		{
			yield return _animation.PlayCoroutineAnimation();
			Destroy(gameObject);
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