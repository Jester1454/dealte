using System;
using Player.PickUp;
using UnityEngine;

namespace Objects
{
    public class LightTrigger : MonoBehaviour
    {
        [SerializeField] private int _id;
        
        public Action<int> OnTrigger;
        public int Id => _id;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<ISavePoint>() != null)
            {
                OnTrigger?.Invoke(_id);
            }
        }
    }
}
