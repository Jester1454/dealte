using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class AimCursor : MonoBehaviour
	{
		[SerializeField] private GameObject _cursorPrefab;
		[SerializeField] private LayerMask _layer;
		[SerializeField] private float _cursorHeight;
		
		private CharacterBehaviour _characterBehaviour;
		private PlayerInput _playerInput;
		private bool _showAimCursor;
		private Transform _currentCursor;

		public bool ShowAimCursor => _showAimCursor;
		public Vector3 CursorPosition => _currentCursor.position;

		public void Enable()
		{
			_playerInput = FindObjectOfType<PlayerInput>();
			_characterBehaviour = FindObjectOfType<CharacterBehaviour>();
			UpdateShowAimCursorStatus(_playerInput);
			OnInputDeviceChange(_playerInput);
			_playerInput.onControlsChanged += OnInputDeviceChange;
		}

		public void Disable()
		{
			_showAimCursor = false;
		}

		private void Update()
		{
			if (!_showAimCursor) return;

			var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
			if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _layer))
			{
				var newCursorPosition = hitInfo.point;
				newCursorPosition.y = transform.position.y + _cursorHeight;
				_currentCursor.position = newCursorPosition;
			}
		}

		private void UpdateShowAimCursorStatus(PlayerInput input)
		{
			if (input == null) return;
			
			_showAimCursor = input.currentControlScheme == _characterBehaviour.PlayerControls.KeyboardAndMouseScheme.name;
		}
		
		private void OnInputDeviceChange(PlayerInput obj)
		{
			UpdateShowAimCursorStatus(obj);

			if (_showAimCursor)
			{
				if (_currentCursor == null)
				{
					_currentCursor = Instantiate(_cursorPrefab).transform;
				}
			}
			else
			{
				if (_currentCursor != null)
				{
					Destroy(_currentCursor.gameObject);
				}
			}
		}

		private void OnDisable()
		{
			_playerInput.onControlsChanged -= OnInputDeviceChange;
		}
	}
}