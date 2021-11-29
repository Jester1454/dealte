using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Utils
{
	public class SavePointShowAnimation : MonoBehaviour
	{
		[SerializeField] private Light _light;
		[SerializeField] private float _targetIntensity;
		[SerializeField] private float _height;
		[SerializeField] private Collider _savePointCollider;
		[SerializeField] private bool _needTimer;
		[SerializeField] private float _timerDuration;
		
		private bool _isShowing = false;
		private float _startHeight;
		public IEnumerator Show(float duration)
		{
			if (_isShowing) yield break;
			
			_isShowing = true;
			_savePointCollider.enabled = true;
			_startHeight = transform.position.y;
			transform.DOMove(transform.transform.position + new Vector3(0, _height, 0), duration);
			var currentIntensity = _light.intensity;
			var speed = _targetIntensity / duration;
			
			while (currentIntensity < _targetIntensity)
			{
				currentIntensity += Time.deltaTime * speed;
				_light.intensity = currentIntensity;
				
				yield return null;
			}

			if (_needTimer)
			{
				StartCoroutine(Timer(_timerDuration, duration));
			}
		}

		private IEnumerator Timer(float timerDuration, float hideAnimationDuration)
		{
			yield return new WaitForSeconds(timerDuration);
			StartCoroutine(Hide(hideAnimationDuration));
		}

		public IEnumerator Hide(float duration)
		{
			if (!_isShowing) yield break;
			
			_savePointCollider.enabled = false;
			
			var currentIntensity = _light.intensity;
			var speed = _targetIntensity / duration;
			transform.DOMove(transform.transform.position + new Vector3(0, _startHeight, 0), duration);

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