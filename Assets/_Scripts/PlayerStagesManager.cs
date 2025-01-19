using System;
using _Scripts.Enums;
using UnityEngine;

namespace _Scripts
{
	// call, fold, check, raise, bet, all-in
	public class PlayerStagesManager : MonoBehaviour
	{
		[SerializeField] private PlayerActionsManager _playerActionsManager;
		private MoneyManager _moneyManager;
		private TableStagesManager _tableStagesManager;
		
		private Player _player;
		
		public event Action<PlayerStage> OnStageActionChanged;
		
		private void Awake()
		{
			TryGetComponent(out _moneyManager);
			TryGetComponent(out _tableStagesManager);
		}

		private void Start()
		{
			_playerActionsManager.OnStageActionClick += OnStageActionClickEvent;
		}

		private void OnDestroy()
		{
			_playerActionsManager.OnStageActionClick -= OnStageActionClickEvent;
		}
		
		public void SetupPlayerStage(Player player)
		{
			_player = player;
			
			_playerActionsManager.SetPlayerInfo(_player.PlayerName);
			
			if (_moneyManager.IsBet) // check if bet was made
			{
				// prepare buttons [Fold, Call BetSize, Raise]
			}
			else if (_tableStagesManager.CurrentStage == TableStage.PreFlop) // check if preflop
			{
				// prepare buttons [Fold, Call BB, Bet]\
				_playerActionsManager.gameObject.SetActive(true);
			}
			else
			{
				// prepare buttons [Fold, Check, Bet]
			}
		}
		
		private void OnStageActionClickEvent(PlayerStage playerStage)
		{
			if (_player == null)
			{
				Debug.LogError($"Player is null");
				
				return;
			}
			
			switch (playerStage)
			{
				case PlayerStage.Call:
					if (_moneyManager.PLayerCall(_player))
					{
						_player.InGameMoney = _moneyManager.CurrentBet;
					}
					else
					{
						Debug.LogError($"{_player.PlayerName} don't have enough money to CALL");
						
						return;
					}

					Debug.Log($"{_player.PlayerName} Called");
					
					OnStageActionChanged?.Invoke(PlayerStage.Call);
					
					break;
				case PlayerStage.Fold:
					_player.ClearCards();
					
					Debug.Log($"{_player.PlayerName} Folded");
					
					OnStageActionChanged?.Invoke(PlayerStage.Fold);
					
					break;
				case PlayerStage.Bet:
					if (_moneyManager.PlayerBet(_player,_moneyManager.CurrentBet * 2)) // TODO: Remove hardcoded amount
					{
						_player.InGameMoney = _moneyManager.CurrentBet;
					}
					else
					{
						Debug.LogError($"{_player.PlayerName} don't have enough money to BET");
						
						return;
					}
					
					Debug.Log($"{_player.PlayerName} Bet");
					
					OnStageActionChanged?.Invoke(PlayerStage.Bet);
					
					break;
				case PlayerStage.Check:
					break;
			}
		}
	}
}