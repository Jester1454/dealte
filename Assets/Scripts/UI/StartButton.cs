using System;
using Player;
using UnityEngine.UI;

namespace UnityEngine.AI
{
	[RequireComponent(typeof(Button))]
	public class StartButton : UnityEngine.MonoBehaviour
	{
		[SerializeField] private CharacterBehaviour _character;
		[SerializeField] private GameObject _uiObject;
		[SerializeField] private WakeUpBehavior _wakeUpBehavior;
		[SerializeField] private GameObject _onWakeUpObject;
		
		private Button _button;
		
		private void Awake()
		{
			_button = GetComponent<Button>();
			
			_button.onClick.AddListener(OnButtonClick);
			_wakeUpBehavior.OnFinishWakeUp += OnFinishWakeUp;
			
			if (_onWakeUpObject != null)
			{
				_onWakeUpObject.SetActive(false);
			}
		}

		private void OnFinishWakeUp()
		{
			if (_onWakeUpObject != null)
			{
				_onWakeUpObject.SetActive(true);
			}
			_wakeUpBehavior.OnFinishWakeUp -= OnFinishWakeUp;

		}

		private void OnButtonClick()
		{
			Cursor.visible = false;
			_character.WakeUp();
			_uiObject.SetActive(false);
		}
	}
}