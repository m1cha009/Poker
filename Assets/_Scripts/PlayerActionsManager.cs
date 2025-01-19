using System;
using _Scripts.Enums;
using TMPro;
using UnityEngine;

namespace _Scripts
{
	public class PlayerActionsManager : MonoBehaviour
	{
		public event Action<PlayerStage> OnStageActionClick;

		[SerializeField] public TMP_Text _playerInfoText;
		
		public void SetPlayerInfo(string name)
		{
			_playerInfoText.SetText($"{name} turn");
		}
		
		public void CallClicked()
		{
			OnStageActionClick?.Invoke(PlayerStage.Call);
		}

		public void FoldClicked()
		{
			OnStageActionClick?.Invoke(PlayerStage.Fold);
		}
		
		public void BetClicked()
		{
			OnStageActionClick?.Invoke(PlayerStage.Bet);
		}
	}
}