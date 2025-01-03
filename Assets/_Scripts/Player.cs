using System.Collections.Generic;
using _Scripts.Helpers;
using UnityEngine;

namespace _Scripts
{
	public class Player : MonoBehaviour
	{
		[SerializeField] private CardComponent _card1;
		[SerializeField] private CardComponent _card2;

		private bool _isFirst = true;
		private readonly List<Card> _playerCards = new();

		public void SetCard(Card card)
		{
			if (_playerCards.Count >= 2)
			{
				Debug.LogError($"Player already has 2 cards: {_playerCards[0]}, {_playerCards[1]}");
				return;
			}
			
			_playerCards.Add(card);
		}

		public List<Card> GetPlayerCards() => _playerCards;

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

		public void ClearCards()
		{
			_playerCards.Clear();
			
			_card1.SetDefaultCardImage();
			_card2.SetDefaultCardImage();
		}
	}
}