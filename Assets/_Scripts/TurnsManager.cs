using System;
using System.Collections.Generic;
using _Scripts.Enums;
using UnityEngine;

namespace _Scripts
{
	public class TurnsManager : MonoBehaviour
	{
		[SerializeField] private PokerManager _pokerManager;
		[SerializeField] private PlayerActionsManager _playerActionsManager;
		
		private List<Player> _activePlayers = new();
		private List<Player> _basePlayers = new();

		private int _currentPlayerIndex;
		private TableStage _currentTableStage = TableStage.PreFlop;

		public event Action<TableStage> OnChangeTableStage;

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
			_basePlayers = players;
			
			if (_basePlayers == null || _basePlayers.Count == 0)
			{
				return;
			}

			_currentPlayerIndex = 0;
			_currentTableStage = TableStage.PreFlop;
			
			TriggerActionVisibility(true);
			
			DisplayPlayer(_currentPlayerIndex);
		}

		public void TriggerActionVisibility(bool isVisible)
		{
			_playerActionsManager.gameObject.SetActive(isVisible);
		}

		private void OnCallClickEvent()
		{
			// _activePlayers.Add(_basePlayers[_currentPlayerIndex]);
			
			Debug.Log($"player {_currentPlayerIndex} called");

			PlayerActionClick();
		}
		
		private void OnFoldClickEvent()
		{
			_basePlayers[_currentPlayerIndex].ClearCards();
			
			Debug.Log($"player {_currentPlayerIndex} folded");
			
			PlayerActionClick();
		}

		private void PlayerActionClick()
		{
			_currentPlayerIndex++;

			if (_currentPlayerIndex < _basePlayers.Count)
			{
				DisplayPlayer(_currentPlayerIndex);
			}
			else
			{
				ChangeTableStage();
			}
		}

		private void DisplayPlayer(int index)
		{
			_basePlayers[index].DisplayCards();
			_playerActionsManager.SetPlayerInfo(index);
		}

		private void ChangeTableStage()
		{
			_currentTableStage = (int)_currentTableStage + 1 >= Enum.GetValues(typeof(TableStage)).Length ? 0 : _currentTableStage + 1;
			
			OnChangeTableStage?.Invoke(_currentTableStage);
			
			_currentPlayerIndex = 0;
		}
	}
}