using UnityEngine;

namespace UI
{
    public class BulletView : MonoBehaviour
    {
        [SerializeField] private GameObject _bulletView;
        public void SetActive(bool value)
        {
            _bulletView.SetActive(value);
        }
    }
}