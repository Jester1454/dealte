using System.Collections;
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
		[SerializeField] private float _throwOffset;
		
		private Rigidbody _rigidbody;
		private GameObject _currentThrowObject;
		
		private bool _canPickUp = false;
		public Transform Transform => transform;
		public bool IsActive => _canPickUp;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!_canPickUp)
				return;
		
			if (other.CompareTag(_playerTag))
			{
				_uiObject.SetActive(true);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (!_canPickUp)
				return;
			
			if (other.CompareTag(_playerTag))
			{
				_uiObject.SetActive(false);
			}
		}
		
		public void PickUp()
		{
			if (!_canPickUp)
				return;
			
			Destroy(_currentThrowObject);
			_uiObject.SetActive(false);
			_pickObject.SetActive(true);
			
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			_canPickUp = false;
			
		}

		public void SetPickUpStatus(bool value)
		{
			_canPickUp = value;
		}

		public void Throw(float force, Vector3 direction)
		{
			_rigidbody.transform.SetParent(null);
			_rigidbody.constraints = RigidbodyConstraints.None;
			_rigidbody.AddForce(direction * force, ForceMode.Impulse);

			StartCoroutine(Throw());
		}

		private IEnumerator Throw()
		{
			var distance = 0f;
			do
			{
				var previousPosition = _rigidbody.position;
				yield return new WaitForFixedUpdate();
				distance = Vector3.Distance(previousPosition, _rigidbody.position);
			} while (distance > Mathf.Epsilon);

			_rigidbody.velocity = Vector3.zero;
			yield return new WaitForFixedUpdate();
			
					
			_pickObject.SetActive(false);
			SetPickUpStatus(true);
			_currentThrowObject = Instantiate(_throwObject, transform.position + new Vector3(0, _throwOffset, 0), Quaternion.Euler(90, 0, 0));
		}
	}
}