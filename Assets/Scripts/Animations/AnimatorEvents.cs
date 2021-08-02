using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AnimatorMoveEvent : UnityEvent<Vector3, Quaternion> { }

namespace RPGCharacterAnimsFREE
{
    public class AnimatorEvents : MonoBehaviour
    {
        [NonSerialized] public Action OnHit;
        [NonSerialized] public Action OnShoot;
        [NonSerialized] public Action OnFootR;
        [NonSerialized] public Action OnFootL;
        [NonSerialized] public Action OnLand;
        [NonSerialized] public Action OnWeaponSwitch;
        [NonSerialized] public Action OnMove;
        [NonSerialized] public Action OnStartAttack;
        [NonSerialized] public Action OnFinishAttack;
        [NonSerialized] public Action OnDodgeRollFinish;
        [NonSerialized] public Action OnStartThrowing;
        [NonSerialized] public Action OnFinishThrowing;
        [NonSerialized] public Action OnWakeUpFinish;
        [NonSerialized] public Action OnPickUpWeaponFinish;

        public void StartAttack()
        {
            OnStartAttack?.Invoke();
        }

        public void FinishAttack()
        {
            OnFinishAttack?.Invoke();
        }

        public void Hit()
        {
            OnHit?.Invoke();
        }

        public void Shoot()
        {
            OnShoot?.Invoke();
        }

        public void FootR()
        {
            OnFootR?.Invoke();
        }

        public void FootL()
        {
            OnFootL?.Invoke();
        }

        public void Land()
        {
            OnLand?.Invoke();
        }

        public void WeaponSwitch()
        {
            OnWeaponSwitch?.Invoke();
        }

        public void DodgeRollFinish()
        {
            OnDodgeRollFinish?.Invoke();
        }

        public void StartThrowing()
        {
            OnStartThrowing?.Invoke();
        }

        public void FinishThrowing()
        {
            OnFinishThrowing?.Invoke();
        }

        public void FinishWakeUp()
        {
            OnWakeUpFinish?.Invoke();
        }

        public void FinishWeaponPickUp()
        {
            OnPickUpWeaponFinish?.Invoke();
        }
    }
}