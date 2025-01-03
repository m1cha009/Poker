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
		[SerializeField] private GameData _gameData;

		private const string GameSceneString = "Game";

		private void Start()
		{
			_gameData.PlayerAmount = 2;
			
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
			_gameData.PlayerAmount = newValue + 2;
		}

		private void ChangeScene()
		{
			SceneManager.LoadScene(GameSceneString);
		}
	}
}