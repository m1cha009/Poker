using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Helpers
{
	public class CardSpritesHelper : MonoBehaviour
	{
		public Sprite[] _cardSprites;
		
		private static readonly Dictionary<string, Sprite> _cardsSpritesDic = new();

		private void Start()
		{
			foreach (var cardSprite in _cardSprites)
			{
				_cardsSpritesDic[cardSprite.name] = cardSprite;
			}
		}

		public static Sprite GetCardSprite(Card card)
		{
			var keyString = $"card{card.Suit}{card.Rank}";
			var cardSprite = _cardsSpritesDic.GetValueOrDefault(keyString);
				
			if (cardSprite == null)
			{
				Debug.Log($"{keyString} is not a valid card");
				return null;
			}

			return cardSprite;
		}
	}
}