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
			
			_player.SetCurrentPlayerBg(true);
			
			_playerActionsManager.SetPlayerInfo(_player.PlayerName);
			_playerActionsManager.gameObject.SetActive(true);
			
			if (_moneyManager.IsBet) // check if bet was made
			{
				if (Mathf.Approximately(_moneyManager.CurrentBet, player.InGameMoney))
				{
					// prepare buttons [Fold, Check, Bet]
					_playerActionsManager.SetupButtons(ActionButtonStages.Fold | ActionButtonStages.Check | ActionButtonStages.Bet);
				}
				else
				{
					// prepare buttons [Fold, Call BB, Bet]
					_playerActionsManager.SetupButtons(ActionButtonStages.Fold | ActionButtonStages.Call | ActionButtonStages.Bet);
				}
			}
			else
			{
				// prepare buttons [Fold, Check, Bet]
				_playerActionsManager.SetupButtons(ActionButtonStages.Fold | ActionButtonStages.Check | ActionButtonStages.Bet);
			}
		}
		
		private void OnStageActionClickEvent(PlayerStage playerStage)
		{
			if (_player == null)
			{
				Debug.LogError($"Player is null");
				
				return;
			}
			
			_player.SetCurrentPlayerBg(false);
			
			switch (playerStage)
			{
				case PlayerStage.Call:
					if (!_moneyManager.PayCall(_player))
					{
						Debug.LogError("Couldn't pay call");
						return;
					}

					Debug.Log($"{_player.PlayerName} Called €{_moneyManager.CurrentBet}");
					
					OnStageActionChanged?.Invoke(PlayerStage.Call);
					
					break;
				case PlayerStage.Fold:
					_player.ClearCards();
					
					Debug.Log($"{_player.PlayerName} Folded");
					
					OnStageActionChanged?.Invoke(PlayerStage.Fold);
					
					break;
				case PlayerStage.Bet:
					if (!_moneyManager.PayBet(_player, _moneyManager.CurrentBet * 2))
					{
						Debug.LogError("Couldn't pay bet");
						return;
					}
					
					Debug.Log($"{_player.PlayerName} Bet €{_moneyManager.CurrentBet}");
					
					OnStageActionChanged?.Invoke(PlayerStage.Bet);
					
					break;
				case PlayerStage.Check:
					Debug.Log($"{_player.PlayerName} Check");
					
					OnStageActionChanged?.Invoke(PlayerStage.Check);
					
					break;
			}
		}
	}
}