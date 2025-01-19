using System;
using _Scripts.SO;
using TMPro;
using UnityEngine;

namespace _Scripts
{
	public class MoneyManager : MonoBehaviour
	{
		[SerializeField] private TMP_Text _potValueText;
		[SerializeField] private TMP_Text _callButtonText;
		[SerializeField] private GameDataSo _gameDataSo;
		
		private float _potSize;
		
		public bool IsBet { get; private set; }
		public float CurrentBet { get; private set; }

		private void Start()
		{
			CurrentBet = _gameDataSo.BigBlind;
		}

		public bool PLayerCall(Player player)
		{
			return IsMoneyReduced(player, CurrentBet);
		}

		public bool PlayerBet(Player player, float amount)
		{
			if (!IsMoneyReduced(player, amount))
			{
				return false;
			}
			
			CurrentBet = amount;
			IsBet = true;

			return true;
		}

		public void AddMoneyToWinner(Player player)
		{
			player.TotalMoney += _potSize;
			
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
			_potSize = 0;
			_potValueText.SetText($"€{_potSize}");
		}
		
		private bool IsMoneyReduced(Player player, float amount)
		{
			if (player.TotalMoney < amount)
			{
				return false;
			}
			
			player.TotalMoney -= amount;
			_potSize += amount;
			_potValueText.SetText($"€{_potSize}");

			return true;
		}
	}
}