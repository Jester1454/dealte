using UnityEngine;

namespace Player.PickUp
{
	public interface IThrowingObject
	{
		void Throw(float force, Vector3 direction);
	}
	
	public class PickableKey : MonoBehaviour, IPickableObject, IThrowingObject
	{
		[SerializeField] private GameObject _uiObject;
		[SerializeField] private string _playerTag;
		[SerializeField] private GameObject _pickObject;
		[SerializeField] private GameObject _throwObject;
		
		private Rigidbody _rigidbody;
		
		private bool _isEnabled = false;
		public Transform Transform => transform;
		public bool IsActive => _isEnabled;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!_isEnabled)
				return;
		
			if (other.CompareTag(_playerTag))
			{
				_uiObject.SetActive(true);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (!_isEnabled)
				return;
			
			if (other.CompareTag(_playerTag))
			{
				_uiObject.SetActive(false);
			}
		}
		
		public void PickUp()
		{
			if (!_isEnabled)
				return;
			
			_uiObject.SetActive(false);
			_pickObject.SetActive(true);
			_throwObject.SetActive(false);
			
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			_isEnabled = false;
			
		}

		public void SetActive(bool value)
		{
			_isEnabled = value;
		}

		public void Throw(float force, Vector3 direction)
		{
			_rigidbody.transform.SetParent(null);
			_rigidbody.constraints = RigidbodyConstraints.None;
			_rigidbody.AddForce(direction * force, ForceMode.Impulse);
		
			_pickObject.SetActive(false);
			_throwObject.SetActive(true);
			SetActive(true);
		}
	}
}