using Enemy.EnemyBehaviours.SensorySystems;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Enemy.EnemyStates
{
	public class DistanceToVisibleTargetCondition : ConditionTask<Transform>
	{
		[RequiredField] public BBParameter<SensorySystem> _sensorySystem;
		public BBParameter<float> _distance;
		
		protected override bool OnCheck()
		{
			return Vector3.Distance(agent.position, _sensorySystem.value.VisibleTarget.transform.position) < _distance.value;
		}
	}
}