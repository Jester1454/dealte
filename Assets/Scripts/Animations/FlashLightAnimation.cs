﻿using System.Collections;
using UnityEngine;

namespace Animations
{
    public class FlashLightAnimation : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer _mesh;
        [SerializeField] private float _targetIntensity;
        [SerializeField] private Color _flashColor;
        private static readonly int _emissionColor = Shader.PropertyToID("_EmissionColor");
        
        public IEnumerator Flash(float duration, int loops = 1)
        {
            var speed = _targetIntensity / (duration / (loops + 1));
            var currentIntensity = 0f;
            _mesh.material.EnableKeyword("_EMISSION");

            for (var i = 0; i < loops; i++)
            {
                while (currentIntensity < _targetIntensity)
                {
                    currentIntensity += Time.deltaTime * speed;
                    _mesh.material.SetColor(_emissionColor, _flashColor * currentIntensity);;
				
                    yield return null;
                }
            
                while (currentIntensity > 0)
                {
                    currentIntensity -= Time.deltaTime * speed;

                    if (currentIntensity < 0)
                    {
                        currentIntensity = 0;
                    }
                    _mesh.material.SetColor(_emissionColor, _flashColor * currentIntensity);;
				
                    yield return null;
                }   
            }

            _mesh.material.DisableKeyword("_EMISSION");
        }
    }
}
