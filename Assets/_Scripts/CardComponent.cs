using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
	public class CardComponent : MonoBehaviour
	{
		[SerializeField] private Image _cardImage;
		[SerializeField] private Sprite _backCardSprite;

		public void SetCardImage(Sprite cardSprite)
		{
			_cardImage.sprite = cardSprite;
		}

		public void SetDefaultCardImage()
		{
			_cardImage.sprite = _backCardSprite;
		}
	}
}