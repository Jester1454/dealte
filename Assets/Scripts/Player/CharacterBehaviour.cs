using Player.Behaviours.AttackSystem;
using Player.Behaviours.HealthSystem;
using Player.Movement;
using Player.PickUp;
using UnityEngine;

namespace Player
{
	public class CharacterBehaviour : MonoBehaviour
	{
		[SerializeField] private PlayerBehaviour _initBehaviour = PlayerBehaviour.Movement;
		[SerializeField] private CharacterMovement _characterMovement;
		[SerializeField] private AttackBehaviour _attackBehaviour;
		[SerializeField] private DodgeRollBehaviour _dodgeRollBehaviour;
		[SerializeField] private HealthBehaviour _healthBehaviour;
		[SerializeField] private ThrowBehaviour _throwBehaviour;
		[SerializeField] private PickUpBehaviour _pickUpBehaviour;
		
		private PlayerControls _playerControls;
		private Vector2 _cameraInput;
		private Vector2 _moveInput;
		private PlayerBehaviour _currentBehaviour;
		
		public bool OnPause { get; set; } = false;

		private void OnEnable()
		{
			_playerControls.Enable();
			_healthBehaviour.OnDeath += OnDeath;
		}

		private void OnDeath()
		{
			_currentBehaviour = PlayerBehaviour.Death;
			_characterMovement.Disable();
			_attackBehaviour.Disable();
			_dodgeRollBehaviour.Disable();
			_throwBehaviour.Disable();
		}

		private void Awake()
		{
			Init();
		}

		private void Init()
		{
			_currentBehaviour = _initBehaviour;
			
			_playerControls = new PlayerControls();
			_playerControls.Gameplay.Attack.performed += context => Attack();
			_playerControls.Gameplay.DodgeRoll.performed += context => DodgeRoll();
			_playerControls.Gameplay.Cast.performed += context => Throw();
			_playerControls.Gameplay.Interact.performed += context => Interact();
		}

		private void Interact()
		{
			if (_currentBehaviour == PlayerBehaviour.Movement)
			{
				_pickUpBehaviour.PickUp();
			}
		}

		private void Throw()
		{
			if (_currentBehaviour != PlayerBehaviour.Movement || !_throwBehaviour.CanThrow())
				return;

			_currentBehaviour = PlayerBehaviour.Throw;
			
			_characterMovement.Stop();
			_throwBehaviour.Throw();
			_throwBehaviour.OnFinishThrowing += OnFinishThrowing;
		}

		private void OnFinishThrowing()
		{
			_currentBehaviour = PlayerBehaviour.Movement;
			_throwBehaviour.OnFinishThrowing -= OnFinishThrowing;
			_characterMovement.Continue(false);
		}

		private void DodgeRoll()
		{
			if (_currentBehaviour == PlayerBehaviour.DodgeRoll)
				return;
			
			_currentBehaviour = PlayerBehaviour.DodgeRoll;
			_dodgeRollBehaviour.MakeDodgeRoll();
			_dodgeRollBehaviour.OnDodgeRollFinish += OnDodgeRollFinish;
			_characterMovement.Stop();
		}

		private void OnDodgeRollFinish()
		{
			_dodgeRollBehaviour.OnDodgeRollFinish -= OnDodgeRollFinish;
			_currentBehaviour = PlayerBehaviour.Movement;
			_characterMovement.Continue(true);
		}

		private void Attack()
		{
			if (_currentBehaviour == PlayerBehaviour.DodgeRoll)
				return;
			
			_currentBehaviour = PlayerBehaviour.Attack;

			_characterMovement.Stop();
			_attackBehaviour.Attack();
			
			_attackBehaviour.OnFinish += OnFinishAttack;
		}

		private void OnFinishAttack()
		{
			_currentBehaviour = PlayerBehaviour.Movement;
			_characterMovement.Continue(false);
			_attackBehaviour.OnFinish -= OnFinishAttack;
		}

		private void Update()
		{
			if (OnPause)
				return;
			
			UpdateLookPositionInput();
			UpdateMovementInput();

			OnMovementState();
		}

		private void OnMovementState()
		{
			_characterMovement.UpdateCameraInput(_cameraInput);
			_characterMovement.UpdateMovementInput(_moveInput);
		}

		private void UpdateLookPositionInput()
		{
			_cameraInput = _playerControls.Gameplay.Camera.ReadValue<Vector2>();
		}

		private void UpdateMovementInput()
		{
			_moveInput = _playerControls.Gameplay.Movement.ReadValue<Vector2>();
		}

		private void OnDisable()
		{
			DeInit();
		}
		
		private void DeInit()
		{
			_playerControls.Disable();
		}
	}

	public enum PlayerBehaviour
	{
		Movement,
		Attack,
		DodgeRoll,
		Death,
		Throw
	}
}