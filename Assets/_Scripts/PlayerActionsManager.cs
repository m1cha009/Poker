using System;
using TMPro;
using UnityEngine;

namespace _Scripts
{
	public class PlayerActionsManager : MonoBehaviour
	{
		public event Action OnCallClick;
		public event Action OnFoldClick;

		[SerializeField] public TMP_Text _playerInfoText;
		
		public void CallClicked()
		{
			OnCallClick?.Invoke();
		}

		public void FoldClicked()
		{
			OnFoldClick?.Invoke();
		}

		public void SetPlayerInfo(int playerIndex)
		{
			_playerInfoText.SetText($"Player {playerIndex+1} turn");
		}
	}
}