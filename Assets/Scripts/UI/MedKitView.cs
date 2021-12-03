using System.Collections.Generic;
using Player.PickUp;
using UnityEngine;

namespace UI
{
	public class MedKitView : MonoBehaviour
	{
		[SerializeField] private TwoStateView _medKitView;
		[SerializeField] private Transform _kitParent;
		private PickUpMedKitBehaviour _pickUpMedKitBehaviour;
		private readonly List<TwoStateView> _kitViews = new List<TwoStateView>();
        
		public void Init(PickUpMedKitBehaviour pickUpMedKitBehaviour)
		{
			_pickUpMedKitBehaviour = pickUpMedKitBehaviour;
			_pickUpMedKitBehaviour.OnMedKitCountChanged += PickUpMedKitBehaviourOnOnMedKitCountChanged;
			Populate();
		}

		private void PickUpMedKitBehaviourOnOnMedKitCountChanged()
		{
			var index = _pickUpMedKitBehaviour.CurrentKitCount;

			foreach (var bullet in _kitViews)
			{
				bullet.SetActive(index > 0);
				index--;
			}
		}

		private void Populate()
		{
			for (int i = 0; i < _pickUpMedKitBehaviour.MaxKitCount; i++)
			{
				_kitViews.Add(Instantiate(_medKitView, _kitParent));
			}

			PickUpMedKitBehaviourOnOnMedKitCountChanged();
		}

		private void OnDisable()
		{
			_pickUpMedKitBehaviour.OnMedKitCountChanged -= PickUpMedKitBehaviourOnOnMedKitCountChanged;
		}
	}
}