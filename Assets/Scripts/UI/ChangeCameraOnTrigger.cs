using Cinemachine;
using Player.PickUp;
using UnityEngine;

namespace UI
{
	public class ChangeCameraOnTrigger : MonoBehaviour
	{
		[SerializeField] private CinemachineVirtualCamera _newCam;
		[SerializeField] private CinemachineVirtualCamera _oldCam;
		[SerializeField] private string _playerTag;
		
		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag(_playerTag))
			{
				if (!other.gameObject.GetComponent<PickUpWeaponBehaviour>().HasWeapon) return;
				
				if (_newCam == null) return;
				if (_oldCam == null) return;

				_newCam.gameObject.SetActive(true);
				_oldCam.gameObject.SetActive(false);
				CinemachineCameraShaker.Instance.UpdateCamera(_newCam);
			}
		}
	}
}