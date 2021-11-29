using System.Collections.Generic;
using System.Linq;
using Animations;
using Objects;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
	[SerializeField] private List<Trigger> _triggers;
	[SerializeField] private List<MaterialPropertyChangeAnimation> _animations;
	[SerializeField] private List<Collider> _colliders;
	private readonly Dictionary<int, bool> _activatedTriggers = new Dictionary<int, bool>();
	
	private void OnEnable()
	{
		foreach (var trigger in _triggers)
		{
			trigger.OnTrigger += OnTrigger;
			_activatedTriggers.Add(trigger.Id, false);
		}
	}
	
	private void OnDisable()
	{
		foreach (var trigger in _triggers)
		{
			trigger.OnTrigger -= OnTrigger;
		}
	}

	private void OnTrigger(int id)
	{
		if (_activatedTriggers.ContainsKey(id))
		{
			_activatedTriggers[id] = true;
		}

		var allTriggerFinish = _activatedTriggers.Values.All(x => x);

		if (allTriggerFinish)
		{
			foreach (var changeAnimation in _animations)
			{
				changeAnimation.PlayAnimation();
			}

			foreach (var collider in _colliders)
			{
				collider.enabled = false;
			}

			OnDisable();
		}
	}
}
