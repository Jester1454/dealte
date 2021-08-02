using Player;
using UnityEngine.UI;

namespace UnityEngine.AI
{
	[RequireComponent(typeof(Button))]
	public class StartButton : UnityEngine.MonoBehaviour
	{
		[SerializeField] private CharacterBehaviour _character;
		[SerializeField] private GameObject _uiObject;
		private Button _button;
		
		private void Awake()
		{
			_button = GetComponent<Button>();
			
			_button.onClick.AddListener(OnButtonClick);
		}

		private void OnButtonClick()
		{
			Cursor.visible = false;
			_character.WakeUp();
			_uiObject.SetActive(false);
		}
	}
}