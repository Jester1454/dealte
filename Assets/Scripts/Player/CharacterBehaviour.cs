using Player.Behaviours.AttackSystem;
using Player.Behaviours.HealthSystem;
using Player.Movement;
using Player.PickUp;
using UI;
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
		[SerializeField] private ChargeThrowBehaviour _throwBehaviour;
		[SerializeField] private PickUpBehaviour _pickUpBehaviour;
		[SerializeField] private WakeUpBehavior _wakeUpBehavior;
		[SerializeField] private PickUpWeaponBehaviour _pickUpWeaponBehaviour;
		[SerializeField] private SavePointBehaviour _savePointBehaviour;
		[SerializeField] private AimBehaviour _aimBehaviour;
		
		private PlayerControls _playerControls;
		private Vector2 _cameraInput;
		private Vector2 _moveInput;
		private CharacterBehaviourState _currentBehaviourState;
		private bool _stopMovementInput = false;
		private static readonly int _positionMoving = Shader.PropertyToID("_PositionMoving");

		public PlayerControls PlayerControls => _playerControls;
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
			_savePointBehaviour.Disable();
			_aimBehaviour.Disable();
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
			_playerControls.Gameplay.Interact.performed += context => Interact();

			_playerControls.Gameplay.Aiming.started += context => StartAiming();
			_playerControls.Gameplay.Aiming.canceled += context => FinishAiming();

			if (_currentBehaviourState == CharacterBehaviourState.Rest)
			{
				_wakeUpBehavior.Enable();
				_characterMovement.SetActiveWalk(true);
				DisableWeaponBehaviours();
			}
			else
			{
				_wakeUpBehavior.Disable();
				_pickUpWeaponBehaviour.Disable();
				_characterMovement.Enable();
				EnableWeaponBehaviours();
			}
		}

		private void StartAiming()
		{
			if (!_throwBehaviour.IsEnable)
				return;

			if (!_throwBehaviour.CanThrow)
				return;

			if (_currentBehaviourState != CharacterBehaviourState.Movement)
				return;
			
			_characterMovement.SetActiveStrafeMovement(true);

			_currentBehaviourState = CharacterBehaviourState.Aiming;
			_aimBehaviour.StartAiming();
			ChargeThrow();
		}

		private void FinishAiming()
		{
			if (_currentBehaviourState == CharacterBehaviourState.Aiming)
			{
				_characterMovement.SetActiveStrafeMovement(false);

				_aimBehaviour.EndAiming();
				Throw();
			}
		}

		private void CancelAiming()
		{
			if (_currentBehaviourState != CharacterBehaviourState.Aiming)
				return;
			
			_characterMovement.SetActiveStrafeMovement(false);

			_aimBehaviour.EndAiming();
			_throwBehaviour.FinishCharge(true);
		}
		
		private void ChargeThrow()
		{
			_throwBehaviour.StartThrowCharge();
			_throwBehaviour.OnFinishThrowing += OnFinishThrowing;
		}

		private void Throw()
		{
			if (_currentBehaviourState != CharacterBehaviourState.Aiming)
				return;

			_characterMovement.SetActiveStrafeMovement(false);
			_characterMovement.Stop();
			_stopMovementInput = true;
			
			_currentBehaviourState = CharacterBehaviourState.Throw;
			_throwBehaviour.FinishCharge(false);
		}

		private void OnFinishThrowing()
		{
			_currentBehaviourState = CharacterBehaviourState.Movement;
			_throwBehaviour.OnFinishThrowing -= OnFinishThrowing;
			
			_stopMovementInput = false;
			_characterMovement.Continue(false);
		}
		
		private void InitHealthBar()
		{
			var healthBar = FindObjectOfType<HealthBar>();
			if (healthBar != null)
			{
				healthBar.Init(_healthBehaviour);
			}
		}
		
		public void WakeUp()
		{
			InitHealthBar();
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
			_pickUpBehaviour.PickUp();

			if (_pickUpWeaponBehaviour.IsEnabled)
			{
				_stopMovementInput = true;
				_pickUpWeaponBehaviour.OnFinishPickUpWeapon += FinishPickUpWeapon;
				_pickUpWeaponBehaviour.PickUpWeapon();
			}
		}

		private void FinishPickUpWeapon()
		{
			_currentBehaviourState = CharacterBehaviourState.Movement;
			_characterMovement.SetActiveWalk(false);
			_stopMovementInput = false;
			
			_pickUpWeaponBehaviour.OnFinishPickUpWeapon -= OnFinishWakeUp;
			
			_pickUpWeaponBehaviour.Disable();
			EnableWeaponBehaviours();
		}

		private void DodgeRoll()
		{
			if (!_dodgeRollBehaviour.IsEnable)
				return;
			
			if (_currentBehaviourState == CharacterBehaviourState.DodgeRoll)
				return;
			
			if (_currentBehaviourState == CharacterBehaviourState.Throw)
				return;
			if (_currentBehaviourState == CharacterBehaviourState.Attack)
				return;

			CancelAiming();
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
			
			if (_currentBehaviourState == CharacterBehaviourState.Throw)
				return;
			
			CancelAiming();
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

			if (!_stopMovementInput)
			{
				UpdateLookPositionInput();
				UpdateMovementInput();

				OnMovementState();	
			}
			
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
		
		private void EnableWeaponBehaviours()
		{
			_attackBehaviour.Enable();
			_dodgeRollBehaviour.Enable();
			_throwBehaviour.Enable();
			_pickUpBehaviour.Enable();
			_savePointBehaviour.Enable();
			_aimBehaviour.Enable();
			InitHealthBar();
		}

		private void DisableWeaponBehaviours()
		{
			_characterMovement.Disable();
			_attackBehaviour.Disable();
			_dodgeRollBehaviour.Disable();
			_throwBehaviour.Disable();
			_pickUpWeaponBehaviour.Disable();
			_pickUpBehaviour.Disable();
			_savePointBehaviour.Disable();
			_aimBehaviour.Disable();
		}
	}

	public enum CharacterBehaviourState
	{
		Rest,
		Movement,
		Attack,
		DodgeRoll,
		Death,
		Aiming,
		Throw
	}
}