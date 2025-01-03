using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using _Scripts.Helpers;
using _Scripts.SO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField] private Button _menuButton;
		[SerializeField] private Button _dealCardsButton;
		[SerializeField] private Button _flopButton;
		[SerializeField] private Button _winnerButton;
		[SerializeField] private GameData _gameData;
		[SerializeField] private Player _playerPrefab;
		[SerializeField] private List<GameObject> _playerPos;
		[SerializeField] private PokerManager _pokerManager;
		[SerializeField] private GameObject[] _tableCardPos;
		[SerializeField] private CardComponent _cardPrefab;

		private int _playerAmount;
		private List<Player> _players = new();
		private const string SceneNameString = "Menu";
		private List<CardComponent> _tableCardsComponent = new();

		private void Start()
		{
			_playerAmount = _gameData.PlayerAmount;
			
			_menuButton.onClick.AddListener(OnMenuClick);
			_dealCardsButton.onClick.AddListener(OnDealCardsClick);
			_winnerButton.onClick.AddListener(OnWinnerClick);
			
			InitializePlayers();
			InitializeTableCards();
		}

		private void OnDestroy()
		{
			_menuButton.onClick.RemoveListener(OnMenuClick);
			_dealCardsButton.onClick.RemoveListener(OnDealCardsClick);
			_winnerButton.onClick.RemoveListener(OnWinnerClick);
		}

		private void InitializePlayers()
		{
			for (var i = 0; i < _playerAmount; i++)
			{
				var player = Instantiate(_playerPrefab, _playerPos[i].transform);
				player.ClearCards();
				
				_players.Add(player);
			}
		}

		private void InitializeTableCards()
		{
			for (var i = 0; i < 5; i++)
			{
				var flopCard = Instantiate(_cardPrefab, _tableCardPos[i].transform);

				flopCard.SetDefaultCardImage();
				
				_tableCardsComponent.Add(flopCard);
			}
		}
		
		private void OnMenuClick()
		{
			SceneManager.LoadScene(SceneNameString);
		}
		
		private void OnDealCardsClick()
		{
			_pokerManager.ShuffleCards();
			
			foreach (var player in _players)
			{
				player.ClearCards();
				
				var cards = _pokerManager.DealCards();

				foreach (var card in cards)
				{
					player.SetCard(card);
				}
				
				player.DisplayCards();
			}
			
			ShowCards();
		}
		
		private void ShowCards()
		{
			var tableCards = new List<Card>();
			
			var flopCards = _pokerManager.DealFlop();
			tableCards.AddRange(flopCards);
			var turnCard = _pokerManager.DealTurn();
			tableCards.Add(turnCard);
			var riverCard = _pokerManager.DealRiver();
			tableCards.Add(riverCard);

			var k = 0;

			foreach (var card in tableCards)
			{
				var cardSprite = CardSpritesHelper.GetCardSprite(card);
				
				_tableCardsComponent[k].SetCardImage(cardSprite);
				k++;
			}
		}
		
		private void OnWinnerClick()
		{
			CheckWinner();
		}

		private void CheckWinner()
		{
			ClearConsole();
			Dictionary<Player, BestPlayerHand> handsDic = new();
			
			foreach (var player in _players)
			{
				var playerCards = player.GetPlayerCards();
				var bestPlayerHand = _pokerManager.GetBestHand(playerCards);
				
				handsDic.Add(player, bestPlayerHand);
			}

			// decide which player has best hand

			var orderedByHandSet = handsDic.OrderByDescending(x => x.Value.Hand)
				.ThenByDescending(x => x.Value.Cards, Comparer<List<Card>>.Create((x, y) =>
				{
					for (var i = 0; i < Math.Min(x.Count, y.Count); i++)
					{
						if (x[i].Rank > y[i].Rank)
						{
							return 1;
						}
				
						if (x[i].Rank < y[i].Rank)
						{
							return -1;
						}
					}

					return 0;
				}));
			
			var k = 1;
			foreach (var orderedHand in orderedByHandSet)
			{
				var cardsValues = string.Empty;
				foreach (var card in orderedHand.Value.Cards)
				{
					cardsValues += $"{card.Rank}";
				}

				var playerCards = orderedHand.Key.GetPlayerCards();
				var playerRankCards = string.Empty;
				foreach (var card in playerCards)
				{
					playerRankCards += $"{card.Rank}";
				}

				Debug.Log($"{k}. Player cards:[{playerRankCards}], Hand:{orderedHand.Value.Hand}, Best cards: {cardsValues}");
				k++;
			}
		}
		
		private void ClearConsole()
		{
#if UNITY_EDITOR
			var logEntries = Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
			var clearMethod = logEntries?.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
			clearMethod?.Invoke(null, null);
#endif
		}
	}
}