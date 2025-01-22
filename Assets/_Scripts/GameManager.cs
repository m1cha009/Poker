using _Scripts.SO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Scripts
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField] private Button _menuButton;
		[SerializeField] private Button _dealCardsButton;
		[SerializeField] private GameDataSo _gameDataSo;
		
		private PlayersManager _playersManager;

		private const string SceneNameString = "Menu";
		private int _entryPlayerIndex;

		private void Awake()
		{
			TryGetComponent(out _playersManager);
		}

		private void Start()
		{
			_menuButton.onClick.AddListener(OnMenuClick);
			_dealCardsButton.onClick.AddListener(OnDealCardsClick);
			
			// _entryPlayerIndex = Random.Range(0, _gameDataSo.PlayerAmount);
			_entryPlayerIndex = 0;
		}

		private void OnDestroy()
		{
			_menuButton.onClick.RemoveListener(OnMenuClick);
			_dealCardsButton.onClick.RemoveListener(OnDealCardsClick);
		}
		
		private void OnMenuClick()
		{
			SceneManager.LoadScene(SceneNameString);
		}

		private void OnDealCardsClick()
		{
			_playersManager.GetNewCards(_entryPlayerIndex);
			_entryPlayerIndex++;
		}
	}
}