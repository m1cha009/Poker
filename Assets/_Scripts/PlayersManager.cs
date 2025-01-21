using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Enums;
using _Scripts.SO;
using Unity.VisualScripting;
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
				_activePlayers.Add(player);
			}

			_currentPlayerIndex = 0;
			_lastPlayerIndex = 0;
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
		
		private int _lastPlayerIndex;
		
		private void OnPlayerStageActionChangedEvent(PlayerStage playerStage)
		{
			switch(playerStage)
			{
				case PlayerStage.Call:
					_currentPlayerIndex = (_currentPlayerIndex + 1) % _activePlayers.Count;
					break;
				case PlayerStage.Fold:
					_activePlayers.RemoveAt(_currentPlayerIndex);
					
					// _lastPlayerIndex = (_lastPlayerIndex - 1 + _activePlayers.Count) % _activePlayers.Count;
					
					if (_currentPlayerIndex >= _activePlayers.Count)
					{
						_currentPlayerIndex = 0;
					}
					break;
				case PlayerStage.Bet:
					_currentPlayerIndex = (_currentPlayerIndex + 1) % _activePlayers.Count;
					break;
				case PlayerStage.Check:
					_currentPlayerIndex = (_currentPlayerIndex + 1) % _activePlayers.Count;
					break;
			}

			if (_moneyManager.IsBet)
			{
				if (HasAllBet())
				{
					var winner = HasWinner(); 
					if (winner != null) 
					{ 
						_moneyManager.AddMoneyToWinner(winner); 
						TriggerActionVisibility(false);
					
						return;
					}
				
					ChangeTableStage();
				}
			}
			else
			{
				if (_currentPlayerIndex == _lastPlayerIndex)
				{
					ChangeTableStage();
				}
			}
			
			_playerStagesManager.SetupPlayerStage(_activePlayers[_currentPlayerIndex]);
		}

		private bool HasAllBet()
		{
			foreach (var player in _activePlayers)
			{
				if (!Mathf.Approximately(player.InGameMoney, _moneyManager.CurrentBet))
				{
					return false;
				}
			}

			return true;
		}

		private Player HasWinner()
		{
			if (_activePlayers.Count == 1)
			{
				return _activePlayers.First();
			}

			if (_tableStagesManager.CurrentStage == TableStage.River)
			{
				return _pokerManager.CheckWinner(_activePlayers);
			}

			return null;
		}

		private void ChangeTableStage()
		{
			_currentPlayerIndex = 0;
				
			_activePlayers.ForEach(player => player.InGameMoney = 0);

			_moneyManager.ClearBet();
			
			_tableStagesManager.NextTableStage();
		}
		
		private void TriggerActionVisibility(bool isVisible) 
		{
			_playerActionsManager.gameObject.SetActive(isVisible);
		}
	}
}