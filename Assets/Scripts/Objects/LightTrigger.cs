using System;
using UnityEngine;

namespace Objects
{
    public class LightTrigger : MonoBehaviour
    {
        [SerializeField] private int _id;
        
        public Action<int> OnTrigger;
        public int Id => _id;
        private bool _isTriggered;
        
        private void OnTriggerEnter(Collider other)
        {
            Trigger(other);
        }

        private void OnTriggerStay(Collider other)
        {
            Trigger(other);
        }

        private void Trigger(Collider other)
        {
            if(_isTriggered) return;
            
            if (other.GetComponent<ISavePoint>() != null)
            {
                OnTrigger?.Invoke(_id);
                _isTriggered = true;
            }
        }
    }
}
