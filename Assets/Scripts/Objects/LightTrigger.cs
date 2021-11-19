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
            var pickableObject = other.GetComponent<IPickableObject>();

            if (pickableObject != null && pickableObject.IsActive)
            {
                OnTrigger?.Invoke(_id);
            }
        }
    }
}
