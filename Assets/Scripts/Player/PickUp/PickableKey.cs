using System.Collections;
using DG.Tweening;
using UnityEngine;
using Utils;

namespace Player.PickUp
{
	public interface IThrowingObject
	{
		void Throw(Vector3 target, float speed);
	}
	
	public class PickableKey : MonoBehaviour, IPickableObject, IThrowingObject
	{
		[SerializeField] private GameObject _uiObject;
		[SerializeField] private string _playerTag;
		[SerializeField] private LightShowAnimation _light;
		[SerializeField] private float _throwOffset;
		[SerializeField] private float _animationDuration;
		[SerializeField] private bool _canPicUpOnAwake = false;
		[SerializeField] private Collider _pickUpCollider;
		
		private LightShowAnimation _currentLight;
		private Rigidbody _rigidbody;
		
		private bool _canPickUp = false;
		public Transform Transform => transform;
		public bool IsActive => _canPickUp;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
			
			if (_canPicUpOnAwake)
			{
				StartCoroutine(SetThrowState());
			}
			else
			{
				SetPickUpStatus(false);
			}
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

		private void OnTriggerStay(Collider other)
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
			_pickUpCollider.enabled = value;
		}

		public void Throw(Vector3 target, float speed)
		{
			if (_canPickUp)
				return;
			
			StartCoroutine(ThrowCoroutine(target, speed));
		}

		private IEnumerator ThrowCoroutine(Vector3 target, float speed)
		{
			_rigidbody.transform.SetParent(null);
			_rigidbody.constraints = RigidbodyConstraints.None;
			transform.rotation = Quaternion.identity;

			float distance;
			do
			{
				_rigidbody.MovePosition(Vector3.Lerp(_rigidbody.position, target, speed * Time.deltaTime));
				yield return new WaitForFixedUpdate();
				distance = Vector3.Distance(target, _rigidbody.position);
			} while (distance > 1);

			yield return StartCoroutine(SetThrowState());
		}

		private IEnumerator SetThrowState()
		{
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			yield return new WaitForFixedUpdate();
			
			transform.rotation = Quaternion.identity;
			_currentLight = Instantiate(_light, transform.position, Quaternion.Euler(90, 0, 0), transform);
			
			StartCoroutine(_currentLight.Show(_animationDuration));
			transform.DOMove(_currentLight.transform.position + new Vector3(0, _throwOffset, 0), _animationDuration, true)
			.onComplete += () =>
			{
				SetPickUpStatus(true);
			};
		}
	}
}