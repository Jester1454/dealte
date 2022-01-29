using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animations
{
    public class FlashLightAnimation : MonoBehaviour
    {
        [SerializeField] private Renderer _mesh;
        [SerializeField] private float _targetIntensity;
        [SerializeField] private Color _flashColor;
        private static readonly int _emissionColor = Shader.PropertyToID("_EmissionColor");
        
        public IEnumerator Flash(float duration, int loops = 1)
        {
            var speed = _targetIntensity / (duration / (loops + 1));
            var currentIntensity = 0f;
            var materials = _mesh.materials;
            var materialByEmissionActive = new List<(bool, Color)>();
            foreach (var material in materials)
            {
                var emissionEnabled = material.IsKeywordEnabled("_EMISSION");
                if (emissionEnabled)
                {
                    materialByEmissionActive.Add((true, material.GetColor(_emissionColor)));
                }
                else
                {
                    material.EnableKeyword("_EMISSION");
                    materialByEmissionActive.Add((false, Color.clear));
                }
            }

            for (var i = 0; i < loops; i++)
            {
                while (currentIntensity < _targetIntensity)
                {
                    currentIntensity += Time.deltaTime * speed;
                    
                    foreach (var material in materials)
                    {
                        if (material == null) yield break;
                        material.SetColor(_emissionColor, _flashColor * currentIntensity);
                    }
				
                    yield return null;
                }
            
                while (currentIntensity > 0)
                {
                    currentIntensity -= Time.deltaTime * speed;

                    if (currentIntensity < 0)
                    {
                        currentIntensity = 0;
                    }
                    
                    foreach (var material in materials)
                    {
                        if (material == null) yield break;
                        material.SetColor(_emissionColor, _flashColor * currentIntensity);
                    }
				
                    yield return null;
                }   
            }

            for (var i = 0; i < materials.Length; i++)
            {
                var material = materials[i];
                if (materialByEmissionActive[i].Item1)
                {
                    material.SetColor(_emissionColor, materialByEmissionActive[i].Item2);
                }
                else
                {
                    material.DisableKeyword("_EMISSION");
                }
            }
        }

        private static float GetEmissionMultiplier(Material mat) 
        {
            var colour = mat.GetColor(_emissionColor);        
            return Mathf.Max(colour.r, colour.g, colour.b);
        }
    }
}
