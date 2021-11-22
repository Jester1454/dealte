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
		
		private void Awake()
		{
			_playerInput = FindObjectOfType<PlayerInput>();
			_characterBehaviour = FindObjectOfType<CharacterBehaviour>();
		}

		private void Start()
		{
			OnInputDeviceChange(_playerInput);
			_playerInput.onControlsChanged += OnInputDeviceChange;
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

		private void OnInputDeviceChange(PlayerInput obj)
		{
			_showAimCursor = obj.currentControlScheme == _characterBehaviour.PlayerControls.KeyboardAndMouseScheme.name;

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

		void OnDisable()
		{
			_playerInput.onControlsChanged -= OnInputDeviceChange;
		}
	}
}