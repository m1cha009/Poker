using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using _Scripts.Enums;
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
		[SerializeField] private GameData _gameData;
		[SerializeField] private Player _playerPrefab;
		[SerializeField] private List<GameObject> _playerPos;
		[SerializeField] private PokerManager _pokerManager;
		[SerializeField] private GameObject[] _tableCardPos;
		[SerializeField] private CardComponent _cardPrefab;
		
		private TurnsManager _turnsManager;

		private int _playerAmount;
		private readonly List<Player> _players = new();
		private const string SceneNameString = "Menu";
		private readonly List<CardComponent> _tableCardsComponent = new();
		private List<Card> _tableCards = new();

		private void Awake()
		{
			TryGetComponent(out _turnsManager);
		}

		private void Start()
		{
			_playerAmount = _gameData.PlayerAmount;
			
			_menuButton.onClick.AddListener(OnMenuClick);
			_dealCardsButton.onClick.AddListener(OnDealCardsClick);
			
			_turnsManager.OnChangeTableStage += OnTableStateChanged;
			
			InitializePlayers();
			InitializeTableCards();
		}

		private void OnDestroy()
		{
			_menuButton.onClick.RemoveListener(OnMenuClick);
			_dealCardsButton.onClick.RemoveListener(OnDealCardsClick);
			
			_turnsManager.OnChangeTableStage -= OnTableStateChanged;
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
				var templateCards = Instantiate(_cardPrefab, _tableCardPos[i].transform);

				templateCards.SetDefaultCardImage();
				templateCards.gameObject.SetActive(false);
				
				_tableCardsComponent.Add(templateCards);
			}
		}
		
		private void OnMenuClick()
		{
			SceneManager.LoadScene(SceneNameString);
		}
		
		private void OnDealCardsClick()
		{
			ClearTableCards();
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

			_turnsManager.Initialize(_players);
		}

		private void OnTableStateChanged(TableStage tableStage)
		{
			Debug.Log($"OnTableStateChanged: {tableStage}");
			
			if (tableStage == TableStage.PreFlop)
			{
				CheckWinner();
				_turnsManager.TriggerActionVisibility(false);
				
				return;
			}
			
			switch (tableStage)
			{
				case TableStage.Flop:
					var flopCards = _pokerManager.DealFlop();
					_tableCards.AddRange(flopCards);
					
					break;
				case TableStage.Turn:
					var turnCard = _pokerManager.DealTurn();
					_tableCards.Add(turnCard);
					
					break;
				case TableStage.River:
					var riverCard = _pokerManager.DealRiver();
					_tableCards.Add(riverCard);
					
					break;
			}
			
			DisplayTableCards();
		}

		private void ClearTableCards()
		{
			_tableCards.Clear();
			
			foreach (var cardComponent in _tableCardsComponent)
			{
				cardComponent.SetDefaultCardImage();
				cardComponent.gameObject.SetActive(false);
			}
		}

		private void DisplayTableCards()
		{
			var k = 0;
			
			foreach (var card in _tableCards)
			{
				var cardSprite = CardSpritesHelper.GetCardSprite(card);
				
				_tableCardsComponent[k].SetCardImage(cardSprite);
				_tableCardsComponent[k].gameObject.SetActive(true);
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