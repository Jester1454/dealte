using UnityEngine.UI;

namespace UnityEngine.AI
{
	[RequireComponent(typeof(Button))]
	public class ExitButton : UnityEngine.MonoBehaviour
	{
		private Button _button;
		
		private void Awake()
		{
			_button = GetComponent<Button>();
			
			_button.onClick.AddListener(OnButtonClick);
		}

		private void OnButtonClick()
		{
			Application.Quit();
		}
	}
}