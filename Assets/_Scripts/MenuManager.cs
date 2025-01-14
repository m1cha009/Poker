using _Scripts.SO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts
{
	public class MenuManager : MonoBehaviour
	{
		[SerializeField] private TMP_Dropdown _playerAmountDropdown;
		[SerializeField] private Button _startGameButton;
		[SerializeField] private GameDataSo _gameDataSo;

		private const string GameSceneString = "Game";

		private void Start()
		{
			_gameDataSo.PlayerAmount = 2;
			
			_playerAmountDropdown.onValueChanged.AddListener(ValueChanged);
			_startGameButton.onClick.AddListener(ChangeScene);
		}

		private void OnDestroy()
		{
			_playerAmountDropdown.onValueChanged.RemoveListener(ValueChanged);
			_startGameButton.onClick.RemoveListener(ChangeScene);
		}

		private void ValueChanged(int newValue)
		{
			_gameDataSo.PlayerAmount = newValue + 2;
		}

		private void ChangeScene()
		{
			SceneManager.LoadScene(GameSceneString);
		}
	}
}