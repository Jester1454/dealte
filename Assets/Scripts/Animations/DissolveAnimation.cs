using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animations
{
	public class DissolveAnimation : MonoBehaviour
	{
		[SerializeField] private Material _dissolveMaterial;
		[SerializeField] private List<Renderer> _rendererList;
		[SerializeField] private Color _dissolveColor;
		[SerializeField] private float _dissolveIntensity;
		[SerializeField] private float _dissolveChangeColor;
		[SerializeField] private Vector2 _dissolveLimit;
		private static readonly int _emissionColor = Shader.PropertyToID("_EdgeColor");
		private static readonly int _dissolveProperty = Shader.PropertyToID("_EdgeWidth");
		private static readonly int _baseColor = Shader.PropertyToID("_BaseColor");

		private Dictionary<Renderer, Material[]> _oldMaterials = new Dictionary<Renderer, Material[]>();
		private bool _isStart;
		
		[ContextMenu("Start Animation")]
		public void StartAnimation()
		{
			if (_isStart) return;
			_isStart = true;
			
			foreach (var rendererValue in _rendererList)
			{
				_oldMaterials.Add(rendererValue, rendererValue.materials);

				var newMaterials = new Material[rendererValue.materials.Length];
				for (var i = 0; i < rendererValue.materials.Length; i++)
				{
					newMaterials[i] = CreateDissolveMaterial(rendererValue.materials[i]);
				}

				rendererValue.materials = newMaterials;
			}

			StartCoroutine(Flash());
		}

		private IEnumerator Flash()
		{
			var currentDissolve = _dissolveLimit.x;
			while (_isStart)
			{
				while (currentDissolve < _dissolveLimit.y)
				{
					currentDissolve += Time.deltaTime * _dissolveChangeColor;
					ApplyProperty(_dissolveProperty, currentDissolve);
					yield return null;
				}
        
				while (currentDissolve > _dissolveLimit.x)
				{
					currentDissolve -= Time.deltaTime * _dissolveChangeColor;
					ApplyProperty(_dissolveProperty, currentDissolve);
					yield return null;
				}
				yield return null;
			}
		}

		private void ApplyProperty(int propertyId, float value)
		{
			foreach (var rendererValue in _rendererList)
			{
				foreach (var material in rendererValue.materials)
				{
					material.SetFloat(propertyId, value);
				}
			}
		}
		
		private Material CreateDissolveMaterial(Material oldMaterial)
		{
			var newMaterial = new Material(_dissolveMaterial);
			newMaterial.SetColor(_emissionColor, _dissolveColor * _dissolveIntensity);
			newMaterial.SetColor(_baseColor, oldMaterial.color);
			return newMaterial;
		}
		
		[ContextMenu("Finish Animation")]
		public void FinishAnimation()
		{
			if (!_isStart) return;
			_isStart = false;

			foreach (var rendererValue in _rendererList)
			{
				foreach (var material in rendererValue.materials)
				{
					Destroy(material);
				}
				
				if (_oldMaterials.TryGetValue(rendererValue, out var value))
				{
					rendererValue.materials = value;
				}
			}
			_oldMaterials.Clear();
		}
	}
}