using System;
using _Scripts.Enums;
using TMPro;
using UnityEngine;

namespace _Scripts
{
	public class PlayerActionsManager : MonoBehaviour
	{
		public event Action<PlayerStageAction> OnStageActionClick;

		[SerializeField] public TMP_Text _playerInfoText;
		
		public void CallClicked()
		{
			OnStageActionClick?.Invoke(PlayerStageAction.Call);
		}

		public void FoldClicked()
		{
			OnStageActionClick?.Invoke(PlayerStageAction.Fold);
		}

		public void SetPlayerInfo(string name)
		{
			_playerInfoText.SetText($"{name} turn");
		}
	}
}