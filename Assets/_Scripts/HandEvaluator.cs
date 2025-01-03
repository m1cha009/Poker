using System.Collections.Generic;
using System.Linq;
using _Scripts.Enums;

namespace _Scripts
{
	public class HandEvaluator
	{
		public BestPlayerHand EvaluateHand(List<Card> playerCards, List<Card> tableCards)
		{
			var bestHand = new BestPlayerHand();
			var allCardsList = new List<Card>();
			allCardsList.AddRange(playerCards);
			allCardsList.AddRange(tableCards);

			if (IsFlushStraight(allCardsList, out var flushStraightHand))
			{
				if (flushStraightHand.Hand is HandsSet.RoyalFlush or HandsSet.StraightFlush)
				{
					return flushStraightHand;
				}
				
				bestHand = flushStraightHand;
			}

			if (IsFourOfKind(allCardsList, out var fourOfKind))
			{
				return fourOfKind;
			}
			
			if (IsFullHouse(allCardsList, out var fullHouseHand))
			{
				return fullHouseHand;
			}
			
			if (bestHand.Hand is HandsSet.Flash)
			{
				return bestHand;
			}

			if (IsStraight(allCardsList, out var bestStraightHand))
			{
				return bestStraightHand;
			}

			if (IsThreeOfKind(allCardsList, out var bestThreeOfKind))
			{
				return bestThreeOfKind;
			}

			if (IsPairs(allCardsList, out var bestPairHand))
			{
				return bestPairHand;
			}

			if (IsHighestCard(allCardsList, out var bestHighHand))
			{
				return bestHighHand;
			}

			return null;
		}

		private bool IsFlushStraight(List<Card> allCardsList, out BestPlayerHand bestHand)
		{
			bestHand = new BestPlayerHand();
			
			var isFlush = IsFlush(allCardsList, out var flushHand);

			if (!isFlush)
			{
				return false;
			}
			
			bestHand.Cards = flushHand.Cards;

			if (IsCardsInSequence(flushHand.Cards))
			{
				if (flushHand.Cards.First().Rank == Rank._A && flushHand.Cards.Last().Rank == Rank._10)
				{
					// Royal flush
					bestHand.Hand = HandsSet.RoyalFlush;
				}
				else
				{
					// StraightFlush
					bestHand.Hand = HandsSet.StraightFlush;
				}
			}
			else
			{
				bestHand.Hand = HandsSet.Flash;
			}

			return true;
		}

		private bool IsFullHouse(List<Card> allCardsList, out BestPlayerHand bestFullHouse)
		{
			bestFullHouse = new BestPlayerHand();

			var highestThreeOfKind = FindHighestXOfKind(allCardsList, 3);

			var remainingCards = allCardsList.Except(highestThreeOfKind).ToList();
			var highestOnePair = FindHighestPair(remainingCards);

			if (highestThreeOfKind.Any() && highestOnePair.Any())
			{
				bestFullHouse.Cards.AddRange(highestThreeOfKind);
				bestFullHouse.Cards.AddRange(highestOnePair);

				bestFullHouse.Hand = HandsSet.FullHouse;
				
				return true;
			}
			
			return false;
		}

		private bool IsFlush(List<Card> allCardsList, out BestPlayerHand tempBestHand)
		{
			tempBestHand = new BestPlayerHand();
			
			var flashGroup = allCardsList
				.GroupBy(card => card.Suit)
				.Where(group => group.Count() >= 5)
				.SelectMany(group => group)
				.ToList();


			if (!flashGroup.Any())
			{
				return false;
			}


			var orderedFlash = flashGroup.OrderByDescending(card => card.Rank)
				.ToList()
				.GetRange(0,5);

			tempBestHand.Cards.AddRange(orderedFlash);
			tempBestHand.Hand = HandsSet.Flash;
			
			return true;
		}

		private bool IsStraight(List<Card> allCardsList, out BestPlayerHand tempBestHand)
		{
			tempBestHand = new BestPlayerHand();
			
			var orderedCards = allCardsList.OrderByDescending(card => card.Rank)
				.GroupBy(card => card.Rank)
				.Select(card => card.First())
				.ToList();

			if (orderedCards.Count < 5)
			{
				return false;
			}

			var straightSequence = new List<Card>();

			foreach (var card in orderedCards)
			{
				if (straightSequence.Count == 0 || card.Rank == straightSequence.Last().Rank - 1)
				{
					straightSequence.Add(card);
				}
				else
				{
					straightSequence.Clear();
					straightSequence.Add(card);
				}

				if (straightSequence.Count == 5)
				{
					tempBestHand.Cards.AddRange(straightSequence);
					tempBestHand.Hand = HandsSet.Straight;
					
					return true;
				}
			}

			if (straightSequence.Count == 4 && orderedCards[0].Rank == Rank._A &&
			    straightSequence.Last().Rank == Rank._2)
			{
				straightSequence.Add(orderedCards[0]);
				tempBestHand.Cards.AddRange(straightSequence);
				tempBestHand.Hand = HandsSet.Straight;

				return true;
			}
			
			return false;
		}

		private bool IsFourOfKind(List<Card> allCardsList, out BestPlayerHand bestFourOfKind)
		{
			bestFourOfKind = new BestPlayerHand();

			var fourOfKindCards = FindHighestXOfKind(allCardsList, 4);

			if (fourOfKindCards.Any())
			{
				var kickerCard = allCardsList.Except(fourOfKindCards).OrderByDescending(card => card.Rank).First();
				
				bestFourOfKind.Cards.AddRange(fourOfKindCards);
				bestFourOfKind.Cards.Add(kickerCard);
				
				bestFourOfKind.Hand = HandsSet.FourOfAKind;
				
				return true;
			}

			return false;
		}

		private bool IsThreeOfKind(List<Card> allCardsList, out BestPlayerHand bestThreeOfKind)
		{
			bestThreeOfKind = new BestPlayerHand();

			var threeOfKindCards = FindHighestXOfKind(allCardsList, 3);

			if (threeOfKindCards.Any())
			{
				var kickerCards = allCardsList.Except(threeOfKindCards).OrderByDescending(card => card.Rank).ToList().GetRange(0,2);
				
				bestThreeOfKind.Cards.AddRange(threeOfKindCards);
				bestThreeOfKind.Cards.AddRange(kickerCards);
				
				bestThreeOfKind.Hand = HandsSet.ThreeOfAKind;
				
				return true;
			}

			return false;
		}

		private bool IsPairs(List<Card> allCardsList, out BestPlayerHand bestPlayerHand)
		{
			bestPlayerHand = new BestPlayerHand();
			
			var pairsList = new List<Card>();
			var remainingTableCards = new List<Card>();
			
			for (var i = 0; i < allCardsList.Count; i++)
			{
				if (pairsList.Contains(allCardsList[i]))
				{
					continue;
				}
				
				for (var j = i+1; j < allCardsList.Count; j++)
				{
					if (allCardsList[i].Rank == allCardsList[j].Rank)
					{
						pairsList.Add(allCardsList[i]);
						pairsList.Add(allCardsList[j]);

						break;
					}
				}

				if (!pairsList.Contains(allCardsList[i]))
				{
					remainingTableCards.Add(allCardsList[i]);
				}
			}

			if (pairsList.Count == 2)
			{
				bestPlayerHand.Hand = HandsSet.OnePair;
				bestPlayerHand.Cards.AddRange(pairsList);

				var orderedRemainingCards = remainingTableCards.OrderByDescending(x => x.Rank)
					.ToList()
					.GetRange(0, 3);
				
				bestPlayerHand.Cards.AddRange(orderedRemainingCards);
				
				return true;
			}

			if (pairsList.Count == 4)
			{
				bestPlayerHand.Hand = HandsSet.TwoPairs;
				bestPlayerHand.Cards.AddRange(pairsList);

				var orderedRemainingCards = remainingTableCards.OrderByDescending(x => x.Rank).First();
				bestPlayerHand.Cards.Add(orderedRemainingCards);

				return true;
			}

			if (pairsList.Count > 2)
			{
				bestPlayerHand.Hand = HandsSet.TwoPairs;
				
				var orderedList = pairsList.OrderByDescending(x => x.Rank).ToList().GetRange(0, 4);
				bestPlayerHand.Cards.AddRange(orderedList);
				
				var orderedRemainingCards = remainingTableCards.OrderByDescending(x => x.Rank).First();
				bestPlayerHand.Cards.Add(orderedRemainingCards);
				
				return true;
			}
			
			return false;
		}
		
		private bool IsHighestCard(List<Card> allCardsList, out BestPlayerHand bestPlayerHand)
		{
			bestPlayerHand = new BestPlayerHand();
			
			var bestCards = allCardsList.OrderByDescending(x => x.Rank).ToList().GetRange(0,5);
			
			bestPlayerHand.Cards.AddRange(bestCards);
			bestPlayerHand.Hand = HandsSet.Highest;
			
			return true;
		}
		
		private List<Card> FindHighestXOfKind(List<Card> cards, int amount)
		{
			var grouped = cards.GroupBy(card => card.Rank)
				.Where(group => group.Count() == amount)
				.OrderByDescending(group => group.Key);

			if (grouped.Any())
			{
				return grouped.First().ToList();
			}

			return new List<Card>();
		}

		private List<Card> FindHighestPair(List<Card> cards)
		{
			var groupedCards = cards.GroupBy(card => card.Rank)
				.Where(group => group.Count() >= 2)
				.ToList();

			if (!groupedCards.Any())
			{
				return new List<Card>();
			}

			var orderedCards = groupedCards.OrderByDescending(group => group.Key)
				.First()
				.ToList()
				.GetRange(0, 2);
			
			return orderedCards;
		}
		
		private bool IsCardsInSequence(List<Card> cards)
		{
			var orderedCards = cards.OrderByDescending(card => card.Rank).ToList();

			for (var i = 0; i < orderedCards.Count - 1; i++)
			{
				if (orderedCards[i].Rank != orderedCards[i+1].Rank + 1)
				{
					return false;
				}
			}
			
			return true;
		}
	}
}