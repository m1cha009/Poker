using System;
using _Scripts.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
	public class PlayerActionsManager : MonoBehaviour
	{
		public event Action<PlayerStage> OnStageActionClick;

		[SerializeField] private TMP_Text _playerInfoText;
		[SerializeField] private Button _foldButton;
		[SerializeField] private Button _callButton;
		[SerializeField] private Button _checkButton;
		[SerializeField] private Button _betButton;
		[SerializeField] private Button _raiseButton;
		[SerializeField] private Button _allInButton;

		public void SetupButtons(ActionButtonStages actionButtonStages)
		{
			_foldButton.gameObject.SetActive((actionButtonStages & ActionButtonStages.Fold) == ActionButtonStages.Fold);
			_callButton.gameObject.SetActive((actionButtonStages & ActionButtonStages.Call) == ActionButtonStages.Call);
			_checkButton.gameObject.SetActive((actionButtonStages & ActionButtonStages.Check) == ActionButtonStages.Check);
			_betButton.gameObject.SetActive((actionButtonStages & ActionButtonStages.Bet) == ActionButtonStages.Bet);
			_raiseButton.gameObject.SetActive((actionButtonStages & ActionButtonStages.Raise) == ActionButtonStages.Raise);
			_allInButton.gameObject.SetActive((actionButtonStages & ActionButtonStages.AllIn) == ActionButtonStages.AllIn);
		}
		
		public void SetPlayerInfo(string name)
		{
			_playerInfoText.SetText($"{name} turn");
		}
		
		public void FoldClicked()
		{
			OnStageActionClick?.Invoke(PlayerStage.Fold);
		}
		
		public void CallClicked()
		{
			OnStageActionClick?.Invoke(PlayerStage.Call);
		}
		
		public void CheckClicked()
		{
			OnStageActionClick?.Invoke(PlayerStage.Check);
		}
		
		public void BetClicked()
		{
			OnStageActionClick?.Invoke(PlayerStage.Bet);
		}
		
		public void RaiseClicked()
		{
			OnStageActionClick?.Invoke(PlayerStage.Raise);
		}
		
		public void AllInClicked()
		{
			OnStageActionClick?.Invoke(PlayerStage.AllIn);
		}
	}
}