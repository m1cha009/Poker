using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Enums;
using _Scripts.SO;
using UnityEngine;

namespace _Scripts
{
	public class PlayersManager : MonoBehaviour
	{
		[SerializeField] private Player _playerPrefab;
		[SerializeField] private List<GameObject> _playerPos;
		[SerializeField] private PlayersDataSo _playerDataSo;
		[SerializeField] private GameDataSo _gameDataSo;
		[SerializeField] private PlayerActionsManager _playerActionsManager;
		[SerializeField] private PokerManager _pokerManager;
		
		private readonly List<Player> _activePlayers = new();
		private readonly List<Player> _tablePlayers = new();
		private int _currentPlayerIndex;
		private MoneyManager _moneyManager;
		private PlayerStagesManager _playerStagesManager;
		private TableStagesManager _tableStagesManager;

		private void Awake()
		{
			TryGetComponent(out _moneyManager);
			TryGetComponent(out _playerStagesManager);
			TryGetComponent(out _tableStagesManager);
		}

		private void Start()
		{
			InitializePlayers();
			
			TriggerActionVisibility(false);
			
			_playerStagesManager.OnStageActionChanged += OnPlayerStageActionChangedEvent;
		}

		private void OnDestroy()
		{
			_playerStagesManager.OnStageActionChanged -= OnPlayerStageActionChangedEvent;
		}
		
		public void GetNewCards(int startPlayerIndex)
		{
			_pokerManager.ShuffleCards();
			_tableStagesManager.ClearTableCards();
			_activePlayers.Clear();
			
			for (var i = 0; i < _tablePlayers.Count; i++)
			{
				var currentIndex = (startPlayerIndex + i) % _tablePlayers.Count;
				var player = _tablePlayers[currentIndex];
				
				player.ClearPlayerState();
				
				var cards = _pokerManager.DealCards();
				
				foreach (var card in cards)
				{
					player.SetCard(card);
				}
				
				if (i == _tablePlayers.Count - 2)
				{
					if (!_moneyManager.PayBlind(player, false))
					{
						Debug.LogError("Couldn't pay SB");
						return;
					}
					
					player.TriggerBlind(true, false);
				}
				else if (i == _tablePlayers.Count - 1)
				{
					if (!_moneyManager.PayBlind(player, true))
					{
						Debug.LogError("Couldn't pay BB");
						return;
					}
					
					player.TriggerBlind(true, true);
				}
				
				player.DisplayCards();
				_currentPlayerIndex = 0;
				
				_activePlayers.Add(player);
			}
			
			_playerStagesManager.SetupPlayerStage(_activePlayers[0]);
		}
		
		private void InitializePlayers()
		{
			for (var i = 0; i < _gameDataSo.PlayerAmount; i++)
			{
				var player = Instantiate(_playerPrefab, _playerPos[i].transform);
				player.InitializePlayer(_playerDataSo.PlayersData[i]);
				
				_tablePlayers.Add(player);
			}
		}
		
		private void OnPlayerStageActionChangedEvent(PlayerStage playerStage)
		{
			switch(playerStage)
			{
				case PlayerStage.Call:
					_currentPlayerIndex++;
					break;
				case PlayerStage.Fold:
					_activePlayers.RemoveAt(_currentPlayerIndex);
					break;
				case PlayerStage.Bet:
					_currentPlayerIndex = _currentPlayerIndex + 1 % _activePlayers.Count;
					
					break;
				case PlayerStage.Check:
					break;
			}
			
			CheckWinningCondition();
		}

		private void TriggerActionVisibility(bool isVisible) 
		{
			_playerActionsManager.gameObject.SetActive(isVisible);
		}
		
		private void CheckWinningCondition()
		{
			if (_activePlayers.Count == 1)
			{
				var player = _activePlayers.First();
				_moneyManager.AddMoneyToWinner(player);
				
				TriggerActionVisibility(false);
				_activePlayers.ForEach(p => p.InGameMoney = 0);
				
				return;
			}

			if (_moneyManager.IsBet)
			{
				Debug.Log("Somebody Bet");
			}
			else if (_currentPlayerIndex >= _activePlayers.Count)
			{
				if (_tableStagesManager.CurrentStage == TableStage.River)
				{
					var winner = _pokerManager.CheckWinner(_activePlayers);
					_moneyManager.AddMoneyToWinner(winner);
					TriggerActionVisibility(false);
					
					return;
				}
			
				_tableStagesManager.NextTableStage();
			
				_currentPlayerIndex = 0;
				
				_activePlayers.ForEach(player => player.InGameMoney = 0);
			}
			
			_playerStagesManager.SetupPlayerStage(_activePlayers[_currentPlayerIndex]);
		}
	}
}