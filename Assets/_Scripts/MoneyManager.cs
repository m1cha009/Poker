using System;
using System.Collections.Generic;
using System.Diagnostics;
using _Scripts.SO;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace _Scripts
{
	public class MoneyManager : MonoBehaviour
	{
		[SerializeField] private TMP_Text _potValueText;
		[SerializeField] private TMP_Text _callButtonText;
		[SerializeField] private GameDataSo _gameDataSo;

		public float PotSize { get; private set; }
		public bool IsBet { get; private set; }
		public float CurrentBet { get; private set; }

		private void Start()
		{
			CurrentBet = _gameDataSo.BigBlind;
		}

		public bool PayCall(Player player)
		{
			var payAmount = CurrentBet - player.InGameMoney;
			return IsMoneyReduced(player, payAmount);
		}

		public bool PayBlind(Player player, bool isBB)
		{
			var blindValue = isBB ? CurrentBet : CurrentBet / 2;
			return IsMoneyReduced(player, blindValue);
		}

		public bool PayBet(Player player, float amount)
		{
			var result = IsMoneyReduced(player, amount);
			if (!result)
			{
				return false;
			}
			
			CurrentBet = player.InGameMoney;
			IsBet = true;

			return true;
		}

		public void AddMoneyToWinner(Player player)
		{
			player.TotalMoney += PotSize;
			
			Debug.Log($"{player.PlayerName} Won €{PotSize}");
			
			ClearPot();
		}

		public float GetPlayerMoney(Player player)
		{
			return player.TotalMoney;
		}
		
		public void ClearBet()
		{
			CurrentBet = _gameDataSo.BigBlind;
			IsBet = false;
		}

		private void ClearPot()
		{
			PotSize = 0;
			_potValueText.SetText($"€{PotSize}");
		}
		
		private bool IsMoneyReduced(Player player, float amount)
		{
			if (player.TotalMoney < amount)
			{
				return false;
			}
			
			player.TotalMoney -= amount;
			player.InGameMoney += amount;
			PotSize += amount;
			_potValueText.SetText($"€{PotSize}");

			return true;
		}
	}
}