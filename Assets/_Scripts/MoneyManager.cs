using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Scripts
{
	public class MoneyManager : MonoBehaviour
	{
		[SerializeField] private TMP_Text _potValueText;
		
		private int _potSize;

		private List<int> _playersMoney = new();

		private void SetPotValue(int newValue)
		{
			_potValueText.SetText($"€{newValue}");
		}
	}
}