using System.Collections.Generic;
using _Scripts.Data;
using _Scripts.Helpers;
using TMPro;
using UnityEngine;

namespace _Scripts
{
	public class Player : MonoBehaviour
	{
		[SerializeField] private CardComponent _card1;
		[SerializeField] private CardComponent _card2;
		[SerializeField] private TMP_Text _playerNameText;
		[SerializeField] private GameObject _sbBlind;
		[SerializeField] private GameObject _bbBlind;
		[SerializeField] private TMP_Text _totalMoneyText;
		[SerializeField] private TMP_Text _inGameMoneyText;

		private bool _isFirst = true;
		private readonly List<CardData> _playerCards = new();
		private string _playerName;
		private float _totalMoney;
		private float _inGameMoney;

		public string PlayerName
		{
			get => _playerName;
			private set
			{
				SetNameText(value);
				
				_playerName = value;
			}
		}

		public float TotalMoney
		{
			get => _totalMoney;
			set
			{
				SetTotalMoneyText(value);
				
				_totalMoney = value;
			}
		}
		
		public float InGameMoney
		{
			get => _inGameMoney;
			set
			{
				SetInGameMoneyText(value);
				
				_inGameMoney = value;
			}
		}

		public void InitializePlayer(PlayerData playerData)
		{
			PlayerName = playerData.Name;
			TotalMoney = playerData.Money;
			InGameMoney = 0;
			
			ClearCards();
		}

		public void SetCard(CardData cardData)
		{
			if (_playerCards.Count >= 2)
			{
				Debug.LogError($"Player already has 2 cards: {_playerCards[0]}, {_playerCards[1]}");
				return;
			}
			
			_playerCards.Add(cardData);
		}

		public List<CardData> GetPlayerCards() => _playerCards;

		public void DisplayCards()
		{
			foreach (var card in _playerCards)
			{
				var cardSprite = CardSpritesHelper.GetCardSprite(card);

				if (_isFirst)
				{
					_card1.SetCardImage(cardSprite);
					_isFirst = false;
				}
				else
				{
					_card2.SetCardImage(cardSprite);
					_isFirst = true;
				}
				
			}
		}

		public void ClearPlayerState()
		{
			ClearCards();
			TriggerBlind(false);
			InGameMoney = 0;
		}

		public void ClearCards()
		{
			_playerCards.Clear();
			
			_card1.SetDefaultCardImage();
			_card2.SetDefaultCardImage();
		}

		private void SetInGameMoneyText(float value)
		{
			if (value == 0)
			{
				_inGameMoneyText.SetText(string.Empty);
				
				return;
			}
			
			_inGameMoneyText.SetText($"€{value}");
		}

		public void TriggerBlind(bool isOn, bool isBB = false)
		{
			if (!isOn)
			{
				_sbBlind.SetActive(false);
				_bbBlind.SetActive(false);
				
				return;
			}

			if (isBB)
			{
				_sbBlind.SetActive(false);
				_bbBlind.SetActive(true);
			}
			else
			{
				_sbBlind.SetActive(true);
				_bbBlind.SetActive(false);
			}
		}
		
		private void SetNameText(string playerName)
		{
			_playerNameText.SetText(playerName);
		}

		private void SetTotalMoneyText(float money)
		{
			_totalMoneyText.SetText($"€{money}");
		}
	}
}