using System.Collections;
using UnityEngine;

namespace Animations
{
	public class MaterialPropertyChangeAnimation : MonoBehaviour
	{
		[SerializeField] private float _propertyChangeSpeed;
		[SerializeField] private Vector2 _propertyLimit;
		[SerializeField] private bool _isDecrease;
		[SerializeField] private Renderer _rendererTarget;
		[SerializeField] private string _propertyName;
		
		private int _property;

		private void Awake()
		{
			_property = Shader.PropertyToID(_propertyName);
		}
		
		[ContextMenu("PlayAnimation")]
		public void PlayAnimation()
		{
			StartCoroutine(Animation());
		}
		
		private IEnumerator Animation()
		{
			var material = _rendererTarget.material;
			if (_isDecrease)
			{
				var currentValue = _propertyLimit.x;
				while (currentValue > _propertyLimit.y)
				{
					currentValue -= Time.deltaTime * _propertyChangeSpeed;
					material.SetFloat(_property, currentValue);
					yield return null;
				}
			}
			else
			{
				var currentValue = _propertyLimit.x;
				while (currentValue < _propertyLimit.y)
				{
					currentValue += Time.deltaTime * _propertyChangeSpeed;
					material.SetFloat(_property, currentValue);
					yield return null;
				}
			}
		}
	}
}