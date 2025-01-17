using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Scripts
{
	public class MoneyManager : MonoBehaviour
	{
		[SerializeField] private TMP_Text _potValueText;
		[SerializeField] private TMP_Text _callButtonText;
		
		private float _potSize;

		private List<int> _playersMoney = new();

		public bool PLayerBet(Player player, float amount)
		{
			if (player.Money < amount)
			{
				return false;
			}
			
			player.Money -= amount;
			_potSize += amount;
			_potValueText.SetText($"€{_potSize}");

			return true;
		}

		public void AddMoneyToWinner(Player player)
		{
			player.Money += _potSize;
			
			ClearPot();
		}

		public float GetPlayerMoney(Player player)
		{
			return player.Money;
		}
		
		private void ClearPot()
		{
			_potSize = 0;
			_potValueText.SetText($"€{_potSize}");
		}
	}
}