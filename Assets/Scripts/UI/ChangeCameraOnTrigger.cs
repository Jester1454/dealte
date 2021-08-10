using Player.PickUp;

namespace UnityEngine.AI
{
	public class ChangeCameraOnTrigger : MonoBehaviour
	{
		[SerializeField] private GameObject _newCam;
		[SerializeField] private GameObject _oldCam;
		[SerializeField] private string _playerTag;
		
		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag(_playerTag))
			{
				if (!other.gameObject.GetComponent<PickUpWeaponBehaviour>().HasWeapon) return;
				
				if (_newCam == null) return;
				if (_oldCam == null) return;

				_newCam.SetActive(true);
				_oldCam.SetActive(false);
			}
		}
	}
}