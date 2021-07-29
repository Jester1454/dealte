using System.Collections;
using DG.Tweening;
using UnityEngine;
using Utils;

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
		[SerializeField] private LightShowAnimation _light;
		[SerializeField] private float _throwOffset;
		[SerializeField] private float _animationDuration;
		
		private Rigidbody _rigidbody;
		private LightShowAnimation _currentLight;
		
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
			
			_uiObject.SetActive(false);
			StartCoroutine(DestroyLight());
			
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			SetPickUpStatus(false);
		}

		private IEnumerator DestroyLight()
		{
			_currentLight.transform.SetParent(null);
			yield return StartCoroutine(_currentLight.Hide(_animationDuration));
			Destroy(_currentLight.gameObject);
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
			float distance;
			do
			{
				var previousPosition = _rigidbody.position;
				yield return new WaitForFixedUpdate();
				distance = Vector3.Distance(previousPosition, _rigidbody.position);
			} while (distance > Mathf.Epsilon);

			_rigidbody.velocity = Vector3.zero;
			_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			yield return new WaitForFixedUpdate();
					
			SetPickUpStatus(true);
			
			transform.rotation = Quaternion.identity;
			_currentLight = Instantiate(_light, transform.position, Quaternion.Euler(90, 0, 0), transform);
			
			StartCoroutine(_currentLight.Show(_animationDuration));
			transform.DOMove(_currentLight.transform.position + new Vector3(0, _throwOffset, 0), _animationDuration, true);
		}
	}
}