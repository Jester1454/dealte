using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Objects
{
    public interface ICollectaleItem
    {
        int Index { get; }
        void Collect(Transform target);
    }

    public class CollectableZone : MonoBehaviour
    {
        [SerializeField] private List<CollectablesPoint> _points;
        public Action OnFinishCollect;
        
        private Dictionary<CollectablesPoint, bool> _occupiedPointData = new Dictionary<CollectablesPoint, bool>();

        private void Awake()
        {
            foreach (var point in _points)
            {
                _occupiedPointData.Add(point, false);
            }
        }

        private bool TryPutToZone(int index, out CollectablesPoint point)
        {
            point = default;
            foreach (var kvp in _occupiedPointData)
            {
                if (!kvp.Value && kvp.Key.Index == index)
                {
                    _occupiedPointData[kvp.Key] = true;
                    point = kvp.Key;
                    CheckOnFinish();
                    return true;
                }
            }

            return false;
        }

        private void OnTriggerEnter(Collider other)
        {
            var collectaleItem = other.gameObject.GetComponent<ICollectaleItem>();
            if (collectaleItem != null)
            {
                if (TryPutToZone(collectaleItem.Index, out var point))
                {
                    collectaleItem.Collect(point.Transform);
                }
            }
        }

        private void CheckOnFinish()
        {
            if (_occupiedPointData.All(kvp => !kvp.Value))
            {
                OnFinishCollect?.Invoke();
                UnityEngine.Debug.LogError("nu vse konchilos huli");
            }
        }
    }

    [Serializable]
    public struct CollectablesPoint
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private int _index;

        public Transform Transform => _transform;
        public int Index => _index;
    }
}
