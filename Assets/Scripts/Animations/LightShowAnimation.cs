using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
	public class LightShowAnimation : MonoBehaviour
	{
		[SerializeField] private Light _light;
		[SerializeField] private float _targetIntensity;

		public IEnumerator Show(float duration)
		{
			var currentIntensity = _light.intensity;
			var speed = _targetIntensity / duration;
			
			while (currentIntensity < _targetIntensity)
			{
				currentIntensity += Time.deltaTime * speed;
				_light.intensity = currentIntensity;
				
				yield return null;
			}
		}

		public IEnumerator Hide(float duration)
		{
			var currentIntensity = _light.intensity;
			var speed = _targetIntensity / duration;

			while (currentIntensity > 0)
			{
				currentIntensity -= Time.deltaTime * speed;

				if (currentIntensity < 0)
				{
					currentIntensity = 0;
				}
				_light.intensity = currentIntensity;
				
				yield return null;
			}
		}
	}
}