namespace UnityEngine.AI
{
	public class ShowOnTrigger : MonoBehaviour
	{
		[SerializeField] private GameObject _object;
		[SerializeField] private string _playerTag;
		[SerializeField] private bool _hideOnExit;
		
		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag(_playerTag))
			{
				if (_object == null) return;
				
				_object.SetActive(true);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (!_hideOnExit) return;
			
			if (other.CompareTag(_playerTag))
			{
				if (_object == null) return;

				_object.SetActive(false);
			}
		}
	}
}