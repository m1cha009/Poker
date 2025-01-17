using System;
using System.Collections.Generic;
using _Scripts.Enums;
using UnityEngine;

namespace _Scripts
{
	public class TurnsManager : MonoBehaviour
	{
		[SerializeField] private PlayerActionsManager _playerActionsManager;
		
		private List<Player> _activePlayers;
		private int _currentPlayerIndex;
		private TableStage _currentTableStage = TableStage.PreFlop;
		private MoneyManager _moneyManager;

		public event Action<TableStage> OnTableStateChanged;

		private void Awake()
		{
			TryGetComponent(out _moneyManager);
		}

		private void Start()
		{
			TriggerActionVisibility(false);
			
			_playerActionsManager.OnStageActionClick += OnStageActionClickEvent;
		}

		private void OnDestroy()
		{
			_playerActionsManager.OnStageActionClick -= OnStageActionClickEvent;
		}

		public void Initialize(List<Player> players)
		{
			_activePlayers = players;
			
			if (_activePlayers == null || _activePlayers.Count == 0)
			{
				return;
			}

			_currentTableStage = TableStage.PreFlop;
			
			TriggerActionVisibility(true);
			
			var playerName = _activePlayers[_currentPlayerIndex].PlayerName;
			_playerActionsManager.SetPlayerInfo(playerName);
		}

		public void TriggerActionVisibility(bool isVisible) 
		{
			_playerActionsManager.gameObject.SetActive(isVisible);
		}
		
		public List<Player> GetActivePlayers() => _activePlayers;

		private void ChangeTableStage()
		{
			_currentTableStage = (int)_currentTableStage + 1 >= Enum.GetValues(typeof(TableStage)).Length ? 0 : _currentTableStage + 1;
			
			OnTableStateChanged?.Invoke(_currentTableStage);
			
			_currentPlayerIndex = 0;
		}
		
		private void OnStageActionClickEvent(PlayerStageAction playerStageAction)
		{
			var player = _activePlayers[_currentPlayerIndex];
			
			switch (playerStageAction)
			{
				case PlayerStageAction.Call:
					if (_moneyManager.PLayerBet(player, 2))
					{
						player.SetPlayMoneyText(2);
					}
					else
					{
						Debug.LogError($"{player.PlayerName} don't have enough money to call");
						
						return;
					}

					Debug.Log($"{player.PlayerName} Called");
					
					_currentPlayerIndex++;
					
					break;
				case PlayerStageAction.Fold:
					_activePlayers[_currentPlayerIndex].ClearCards();
					_activePlayers.RemoveAt(_currentPlayerIndex);
			
					Debug.Log($"{player.PlayerName} Folded");
					
					break;
				case PlayerStageAction.Bet:
					break;
				case PlayerStageAction.Check:
					break;
			}
			
			PlayerActionClick();
		}
		
		private void PlayerActionClick()
		{
			if (_activePlayers.Count == 1)
			{
				OnTableStateChanged?.Invoke(TableStage.PreFlop);
				
				return;
			}

			if (_currentPlayerIndex >= _activePlayers.Count)
			{
				ChangeTableStage();
			}
			
			var playerName = _activePlayers[_currentPlayerIndex].PlayerName;
			_playerActionsManager.SetPlayerInfo(playerName);
		}
	}
}