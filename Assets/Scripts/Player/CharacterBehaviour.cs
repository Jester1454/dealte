using System.Collections;
using Player.Behaviours.AttackSystem;
using Player.Behaviours.HealthSystem;
using Player.Movement;
using Player.PickUp;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class CharacterBehaviour : MonoBehaviour
	{
		[SerializeField] private CharacterBehaviourState _initBehaviourState = CharacterBehaviourState.Movement;
		[SerializeField] private CharacterMovement _characterMovement;
		[SerializeField] private ComboAttackBehaviour _comboAttackBehaviour;
		[SerializeField] private DodgeRollBehaviour _dodgeRollBehaviour;
		[SerializeField] private PlayerHealthBehaviour _healthBehaviour;
		[SerializeField] private WakeUpBehavior _wakeUpBehavior;
		[SerializeField] private PickUpWeaponBehaviour _pickUpWeaponBehaviour;
		[SerializeField] private SavePointBehaviour _savePointBehaviour;
		[SerializeField] private ShootAimBehaviour _aimBehaviour;
		[SerializeField] private ShootBehavior _shootBehaviour;
		[SerializeField] private AimCursor _aimCursor;
		
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
			_comboAttackBehaviour.Disable();
			_dodgeRollBehaviour.Disable();
			_savePointBehaviour.Disable();
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

			_playerControls.Gameplay.Aim.started += context => StartAiming();
			_playerControls.Gameplay.Aim.canceled += context => FinishAiming();
			_playerControls.Gameplay.Shoot.started += context => StartCoroutine(Shoot());
			
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
			if (!_aimBehaviour.IsEnable)
				return;

			if (!_shootBehaviour.IsEnable)
				return;
			//
			// if (_shootBehaviour.CurrentAmmo <= 0)
			// 	return;

			if (_currentBehaviourState != CharacterBehaviourState.Shoot &&
			    _currentBehaviourState != CharacterBehaviourState.Movement) return;
			
			_characterMovement.Stop();
			_currentBehaviourState = CharacterBehaviourState.Aiming;
			_aimBehaviour.StartAiming();
		}

		private void FinishAiming()
		{
			if (_currentBehaviourState == CharacterBehaviourState.Aiming)
			{
				_characterMovement.Continue(false);
				_currentBehaviourState = CharacterBehaviourState.Movement;
				_stopMovementInput = false;
				_aimBehaviour.EndAiming();
			}
		}

		private void CancelAiming()
		{
			if (_currentBehaviourState != CharacterBehaviourState.Aiming)
				return;
			
			_stopMovementInput = false;
			_characterMovement.Continue(false);
			_aimBehaviour.EndAiming();
		}
		
		private IEnumerator Shoot()
		{
			if (_currentBehaviourState == CharacterBehaviourState.Shoot)
				yield break;
			
			_characterMovement.Stop();
			_stopMovementInput = true;

			if (_aimCursor.ShowAimCursor)
			{
				_currentBehaviourState = CharacterBehaviourState.Shoot;
				yield return _shootBehaviour.Shoot(_aimCursor.CursorPosition - transform.position, true);
			}
			else
			{
				_currentBehaviourState = CharacterBehaviourState.Shoot;
				_aimBehaviour.EndAiming();
				yield return _shootBehaviour.Shoot(transform.forward, true);

				if (_playerControls.Gameplay.Aim.phase == InputActionPhase.Started)
				{
					StartAiming();
					yield break;
				}
			}
			
			_stopMovementInput = false;
			_characterMovement.Continue(false);
			_currentBehaviourState = CharacterBehaviourState.Movement;
		}
		
		private void InitHUD()
		{
			InitHealthBar();
			InitAmmoBar();
		}

		private void InitHealthBar()
		{
			var healthBar = FindObjectOfType<HealthBar>();
			if (healthBar != null)
			{
				healthBar.Init(_healthBehaviour, _savePointBehaviour);
			}
		}
		
		private void InitAmmoBar()
		{
			var ammoView = FindObjectOfType<AmmoView>();
			if (ammoView != null)
			{
				ammoView.Init(_shootBehaviour);
			}
		}
		
		public void WakeUp()
		{
			InitHUD();
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
			
			if (_currentBehaviourState == CharacterBehaviourState.Shoot)
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
			if (!_comboAttackBehaviour.IsEnable)
				return;
			
			if (_currentBehaviourState == CharacterBehaviourState.DodgeRoll)
				return;
			
			if (_currentBehaviourState == CharacterBehaviourState.Shoot)
				return;
			
			CancelAiming();
			_currentBehaviourState = CharacterBehaviourState.Attack;

			_characterMovement.Stop();
			_comboAttackBehaviour.Attack();
			
			_comboAttackBehaviour.OnFinish += OnFinishAttack;
		}

		private void OnFinishAttack()
		{
			_currentBehaviourState = CharacterBehaviourState.Movement;
			_characterMovement.Continue(false);
			_comboAttackBehaviour.OnFinish -= OnFinishAttack;
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
			_comboAttackBehaviour.Enable();
			_dodgeRollBehaviour.Enable();
			_aimBehaviour.Enable();
			_shootBehaviour.Enable();
			_savePointBehaviour.Enable();
			InitHUD();
		}

		private void DisableWeaponBehaviours()
		{
			_characterMovement.Disable();
			_comboAttackBehaviour.Disable();
			_dodgeRollBehaviour.Disable();
			_aimBehaviour.Disable();
			_shootBehaviour.Disable();
			_pickUpWeaponBehaviour.Disable();
			_savePointBehaviour.Disable();
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
		Shoot,
	}
}