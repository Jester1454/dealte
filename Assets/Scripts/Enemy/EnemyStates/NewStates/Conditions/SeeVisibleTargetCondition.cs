using Enemy.EnemyBehaviours.SensorySystems;
using NodeCanvas.Framework;

namespace Enemy.EnemyStates
{
	public class SeeVisibleTargetCondition : ConditionTask<SensorySystem>
	{
		protected override bool OnCheck()
		{
			return agent.VisibleTarget != null;
		}
	}
}