using UnityEditor;
using UnityEngine;

namespace Utils
{
	[InitializeOnLoad]
	static class EditorsInitializer
	{
		static EditorsInitializer()
		{
			if (EditorPrefs.GetBool("switch_light_on"))
			{
				SwitchLightButton.Enable();
			}
		}
	}
	
	public class SwitchLightButton : EditorWindow
	{
		private static Light _light;
		private static float _previousIntensity;
		private static bool _isEnabled = false;
		
		[MenuItem("Tools/Light Switch Enable")]
		public static void Enable()
		{
			SceneView.duringSceneGui += OnSceneGUI;
		}
 
		[MenuItem("Tools/Light Switch Disable")]
		public static void Disable()
		{
			SceneView.duringSceneGui -= OnSceneGUI;
		}
		
		private static void OnSceneGUI(SceneView sceneview)
		{
			Handles.BeginGUI();
			GUILayout.BeginArea(new Rect(300, 10, 100, 70));
			{
				if (GUILayout.Button("Switch light"))
				{
					SwitchLight();
				}	
			}
			GUILayout.EndArea();
			
			Handles.EndGUI();
		}

		private static void SwitchLight()
		{
			if (_light == null)
			{
				var lights = FindObjectsOfType<Light>();

				foreach (var light in lights)
				{
					if (light.type == LightType.Directional)
					{
						_light = light;
					}
				}
			}

			if (_isEnabled)
			{
				_light.intensity = _previousIntensity;
				_isEnabled = false;
			}
			else
			{
				_previousIntensity = _light.intensity;
				_light.intensity = 10;
				_isEnabled = true;
			}
		}
	}
}