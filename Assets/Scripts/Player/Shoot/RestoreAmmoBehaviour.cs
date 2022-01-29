using Player.Behaviours.AttackSystem;
using UnityEngine;

namespace Player
{
	[RequireComponent(typeof(ShootBehavior), typeof(ComboAttackBehaviour))]
	public class RestoreAmmoBehaviour : MonoBehaviour
	{
		private ComboAttackBehaviour _attackBehaviour;
		private ShootBehavior _shootBehavior;
		private bool _isEnable = false;

		public void Enable()
		{
			_isEnable = true;
			_attackBehaviour = GetComponent<ComboAttackBehaviour>();
			_shootBehavior = GetComponent<ShootBehavior>();
			_attackBehaviour.OnEnemyDamage += OnEnemyDamage;
		}

		public void Disable()
		{
			_isEnable = false;
			_attackBehaviour.OnEnemyDamage -= OnEnemyDamage;
		}
		
		private void OnEnemyDamage(int hitCount)
		{
			_shootBehavior.CurrentAmmo += hitCount;
		}
	}
}