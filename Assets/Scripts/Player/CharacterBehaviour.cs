using Player.Behaviours.AttackSystem;
using Player.Behaviours.HealthSystem;
using Player.Movement;
using Player.PickUp;
using UnityEngine;

namespace Player
{
	public class CharacterBehaviour : MonoBehaviour
	{
		[SerializeField] private CharacterBehaviourState _initBehaviourState = CharacterBehaviourState.Movement;
		[SerializeField] private CharacterMovement _characterMovement;
		[SerializeField] private AttackBehaviour _attackBehaviour;
		[SerializeField] private DodgeRollBehaviour _dodgeRollBehaviour;
		[SerializeField] private HealthBehaviour _healthBehaviour;
		[SerializeField] private ThrowBehaviour _throwBehaviour;
		[SerializeField] private PickUpBehaviour _pickUpBehaviour;
		[SerializeField] private WakeUpBehavior _wakeUpBehavior;
		[SerializeField] private PickUpWeaponBehaviour _pickUpWeaponBehaviour;
		
		private PlayerControls _playerControls;
		private Vector2 _cameraInput;
		private Vector2 _moveInput;
		private CharacterBehaviourState _currentBehaviourState;

		private static readonly int _positionMoving = Shader.PropertyToID("_PositionMoving");

		public bool OnPause { get; set; } = false;

		private void OnEnable()
		{
			_playerControls.Enable();
			_healthBehaviour.OnDeath += OnDeath;
		}

		private void OnDeath()
		{
			_currentBehaviourState = CharacterBehaviourState.Death;
			_characterMovement.Disable();
			_attackBehaviour.Disable();
			_dodgeRollBehaviour.Disable();
			_throwBehaviour.Disable();
			_pickUpBehaviour.Disable();
		}
		
		private void Awake()
		{
			Init();
		}

		private void Init()
		{
			_currentBehaviourState = _initBehaviourState;
			
			_playerControls = new PlayerControls();
			_playerControls.Gameplay.Attack.performed += context => Attack();
			_playerControls.Gameplay.DodgeRoll.performed += context => DodgeRoll();
			_playerControls.Gameplay.Cast.performed += context => Throw();
			_playerControls.Gameplay.Interact.performed += context => Interact();

			if (_currentBehaviourState == CharacterBehaviourState.Rest)
			{
				_wakeUpBehavior.Enable();
				_characterMovement.SetActiveWalk(true);

				_characterMovement.Disable();
				_attackBehaviour.Disable();
				_dodgeRollBehaviour.Disable();
				_throwBehaviour.Disable();
				_pickUpWeaponBehaviour.Disable();
			}
			else
			{
				_wakeUpBehavior.Disable();
			}
		}
		
		public void WakeUp()
		{
			_wakeUpBehavior.OnFinishWakeUp += OnFinishWakeUp;
			_wakeUpBehavior.WakeUp();
		}

		private void OnFinishWakeUp()
		{
			_currentBehaviourState = CharacterBehaviourState.Movement;
			
			_wakeUpBehavior.OnFinishWakeUp -= OnFinishWakeUp;

			_characterMovement.Enable();
			_wakeUpBehavior.Disable();
			_pickUpWeaponBehaviour.Enable();
		}

		private void Interact()
		{
			if (_currentBehaviourState == CharacterBehaviourState.Movement)
			{
				_pickUpBehaviour.PickUp();

				if (_pickUpWeaponBehaviour.IsEnabled)
				{
					_pickUpWeaponBehaviour.OnFinishPickUpWeapon += FinishPickUpWeapon;
					_pickUpWeaponBehaviour.PickUpWeapon();
				}
			}
		}

		private void FinishPickUpWeapon()
		{
			_currentBehaviourState = CharacterBehaviourState.Movement;
			_characterMovement.SetActiveWalk(false);
			
			_pickUpWeaponBehaviour.OnFinishPickUpWeapon -= OnFinishWakeUp;
			
			_pickUpWeaponBehaviour.Disable();
			
			_attackBehaviour.Enable();
			_dodgeRollBehaviour.Enable();
			_throwBehaviour.Enable();
			_pickUpBehaviour.Enable();
		}

		private void Throw()
		{
			if (!_throwBehaviour.IsEnable)
				return;
			
			if (_currentBehaviourState != CharacterBehaviourState.Movement || !_throwBehaviour.CanThrow())
				return;

			_currentBehaviourState = CharacterBehaviourState.Throw;
			
			_characterMovement.Stop();
			_throwBehaviour.Throw();
			_throwBehaviour.OnFinishThrowing += OnFinishThrowing;
		}

		private void OnFinishThrowing()
		{
			_currentBehaviourState = CharacterBehaviourState.Movement;
			_throwBehaviour.OnFinishThrowing -= OnFinishThrowing;
			_characterMovement.Continue(false);
		}

		private void DodgeRoll()
		{
			if (!_dodgeRollBehaviour.IsEnable)
				return;
			
			if (_currentBehaviourState == CharacterBehaviourState.DodgeRoll)
				return;
			
			_currentBehaviourState = CharacterBehaviourState.DodgeRoll;
			_dodgeRollBehaviour.MakeDodgeRoll();
			_dodgeRollBehaviour.OnDodgeRollFinish += OnDodgeRollFinish;
			_characterMovement.Stop();
		}

		private void OnDodgeRollFinish()
		{
			_dodgeRollBehaviour.OnDodgeRollFinish -= OnDodgeRollFinish;
			_currentBehaviourState = CharacterBehaviourState.Movement;
			_characterMovement.Continue(true);
		}

		private void Attack()
		{
			if (!_attackBehaviour.IsEnable)
				return;
			
			if (_currentBehaviourState == CharacterBehaviourState.DodgeRoll)
				return;
			
			_currentBehaviourState = CharacterBehaviourState.Attack;

			_characterMovement.Stop();
			_attackBehaviour.Attack();
			
			_attackBehaviour.OnFinish += OnFinishAttack;
		}

		private void OnFinishAttack()
		{
			_currentBehaviourState = CharacterBehaviourState.Movement;
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
			
			Shader.SetGlobalVector(_positionMoving, transform.position);
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

	public enum CharacterBehaviourState
	{
		Rest,
		Movement,
		Attack,
		DodgeRoll,
		Death,
		Throw
	}
}