using System;
using System.Collections.Generic;
using _Scripts.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts
{
	public class PokerManager : MonoBehaviour
	{
		private readonly Card[] _cards = new Card[52];
		private readonly Stack<Card> _cardsStack = new();
		private readonly HandEvaluator _handEvaluator = new();
		private readonly List<Card> _tableCards = new();

		private void Start()
		{
			InitializeCards();

			Debug.Log("Cards initialized");
		}

		public Card[] DealCards()
		{
			var card = _cardsStack.Pop();
			var card2 = _cardsStack.Pop();

			// Debug.Log(GetCardString(card));

			return new[] { card, card2 };
		}

		public List<Card> DealFlop()
		{
			_tableCards.Clear();

			for (var i = 0; i < 3; i++)
			{
				var card = _cardsStack.Pop();
				_tableCards.Add(card);
			}

			return _tableCards;
		}

		public Card DealTurn()
		{
			var card = _cardsStack.Pop();
			_tableCards.Add(card);

			return card;
		}
		
		public Card DealRiver()
		{
			var card = _cardsStack.Pop();
			_tableCards.Add(card);

			return card;
		}

		public BestPlayerHand GetBestHand(List<Card> playerCards)
		{
			return _handEvaluator.EvaluateHand(playerCards, _tableCards);
		}

		public void ShuffleCards()
		{
			Shuffle();
			
			_cardsStack.Clear();
			// foreach (var card in _cards)
			// {
			// 	_cardsStack.Push(card);
			// }
			
			// setup predifined cards
			foreach (var card in FullHouseCards())
			{
				_cardsStack.Push(card);
			}
			
			Debug.Log("Cards shuffled");
		}
		
		private List<Card> FourthOKindCards()
		{
			var cards = new List<Card>()
			{
				new() { Suit = Suit.Clubs, Rank = Rank._2 },
				new() { Suit = Suit.Diamonds, Rank = Rank._2 },
				new() { Suit = Suit.Hearts, Rank = Rank._2 },
				new() { Suit = Suit.Spades, Rank = Rank._2 },
				new() { Suit = Suit.Spades, Rank = Rank._3 }, // table
				
				new() { Suit = Suit.Spades, Rank = Rank._4 }, // player
				new() { Suit = Suit.Spades, Rank = Rank._5 },
				
				new() { Suit = Suit.Spades, Rank = Rank._6 },
				new() { Suit = Suit.Spades, Rank = Rank._7 },
				
				new() { Suit = Suit.Spades, Rank = Rank._8 },
				new() { Suit = Suit.Spades, Rank = Rank._9 },
				
				new() { Suit = Suit.Spades, Rank = Rank._10 },
				new() { Suit = Suit.Spades, Rank = Rank._J },
				
				new() { Suit = Suit.Spades, Rank = Rank._Q },
				new() { Suit = Suit.Spades, Rank = Rank._K },
				
				new() { Suit = Suit.Spades, Rank = Rank._A },
				new() { Suit = Suit.Hearts, Rank = Rank._A },
			};

			return cards;
		}

		private List<Card> StraightCards()
		{
			var cards = new List<Card>()
			{
				new() { Suit = Suit.Spades, Rank = Rank._Q },
				new() { Suit = Suit.Spades, Rank = Rank._J },
				new() { Suit = Suit.Spades, Rank = Rank._4 },
				new() { Suit = Suit.Spades, Rank = Rank._10 },
				new() { Suit = Suit.Hearts, Rank = Rank._10 },
				
				new() { Suit = Suit.Spades, Rank = Rank._A },
				new() { Suit = Suit.Spades, Rank = Rank._K },
				
				new() { Suit = Suit.Spades, Rank = Rank._8 },
				new() { Suit = Suit.Spades, Rank = Rank._9 },
				
				new() { Suit = Suit.Hearts, Rank = Rank._K },
				new() { Suit = Suit.Diamonds, Rank = Rank._9 },
				
				new() { Suit = Suit.Diamonds, Rank = Rank._10 },
				new() { Suit = Suit.Clubs, Rank = Rank._10 },
				
				new() { Suit = Suit.Diamonds, Rank = Rank._4 },
				new() { Suit = Suit.Clubs, Rank = Rank._4 },
				
				new() { Suit = Suit.Diamonds, Rank = Rank._Q },
				new() { Suit = Suit.Clubs, Rank = Rank._Q },
			};

			return cards;
		}

		private List<Card> FullHouseCards()
		{
			var cards = new List<Card>()
			{
				new() { Suit = Suit.Spades, Rank = Rank._8 },
				new() { Suit = Suit.Spades, Rank = Rank._4 },
				new() { Suit = Suit.Hearts, Rank = Rank._4 },
				new() { Suit = Suit.Spades, Rank = Rank._A },
				new() { Suit = Suit.Hearts, Rank = Rank._A },

				new() { Suit = Suit.Hearts, Rank = Rank._8 },
				new() { Suit = Suit.Diamonds, Rank = Rank._4 },

				new() { Suit = Suit.Clubs, Rank = Rank._A },
				new() { Suit = Suit.Clubs, Rank = Rank._4 },
				
				new() { Suit = Suit.Clubs, Rank = Rank._8 },
				new() { Suit = Suit.Diamonds, Rank = Rank._8 },
			};

			return cards;
		}

		private void Shuffle()
		{
			var cardsLength = _cards.Length;

			while (cardsLength > 1)
			{
				var rnd = Random.Range(0, cardsLength);
				
				cardsLength--;
			
				(_cards[cardsLength], _cards[rnd]) = (_cards[rnd], _cards[cardsLength]);
			}
		}

		private void InitializeCards()
		{
			var k = 0;

			foreach (var suit in Enum.GetValues(typeof(Suit)))
			{
				foreach (var rank in Enum.GetValues(typeof(Rank)))
				{
					_cards[k] = new Card
					{
						Suit = (Suit)suit,
						Rank = (Rank)rank,
					};
					
					k++;
				}
			}
		}
	}
}
