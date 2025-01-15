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

		public event Action<TableStage> OnTableStateChanged;

		private void Start()
		{
			_playerActionsManager.OnCallClick += OnCallClickEvent;
			_playerActionsManager.OnFoldClick += OnFoldClickEvent;
			
			TriggerActionVisibility(false);
		}

		private void OnDestroy()
		{
			_playerActionsManager.OnCallClick -= OnCallClickEvent;
			_playerActionsManager.OnFoldClick -= OnFoldClickEvent;
		}

		public void Initialize(List<Player> players)
		{
			_activePlayers = new List<Player>(players);
			
			if (_activePlayers == null || _activePlayers.Count == 0)
			{
				return;
			}

			_currentPlayerIndex = 0;
			_currentTableStage = TableStage.PreFlop;
			
			TriggerActionVisibility(true);
			
			var playerName = _activePlayers[_currentPlayerIndex].GetPlayerName();
			_playerActionsManager.SetPlayerInfo(playerName);
		}

		public void TriggerActionVisibility(bool isVisible)
		{
			_playerActionsManager.gameObject.SetActive(isVisible);
		}

		private void OnCallClickEvent()
		{
			// _activePlayers.Add(_basePlayers[_currentPlayerIndex]);

			var playerName = _activePlayers[_currentPlayerIndex].GetPlayerName();
			Debug.Log($"{playerName} Called");

			PlayerActionClick();
		}
		
		private void OnFoldClickEvent()
		{
			var playerName = _activePlayers[_currentPlayerIndex].GetPlayerName();
			_activePlayers[_currentPlayerIndex].ClearCards();
			_activePlayers.RemoveAt(_currentPlayerIndex);

			_currentPlayerIndex--;
			
			Debug.Log($"{playerName} Folded");
			
			PlayerActionClick();
		}

		private void PlayerActionClick()
		{
			_currentPlayerIndex++;

			if (_activePlayers.Count == 1)
			{
				OnTableStateChanged?.Invoke(TableStage.PreFlop);
				
				return;
			}

			if (_currentPlayerIndex >= _activePlayers.Count)
			{
				ChangeTableStage();
			}
			
			var playerName = _activePlayers[_currentPlayerIndex].GetPlayerName();
			_playerActionsManager.SetPlayerInfo(playerName);
		}
		
		public List<Player> GetActivePlayers() => _activePlayers;

		private void ChangeTableStage()
		{
			_currentTableStage = (int)_currentTableStage + 1 >= Enum.GetValues(typeof(TableStage)).Length ? 0 : _currentTableStage + 1;
			
			OnTableStateChanged?.Invoke(_currentTableStage);
			
			_currentPlayerIndex = 0;
		}
	}
}