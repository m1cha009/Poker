using System.Collections.Generic;
using _Scripts.Enums;

namespace _Scripts
{
	public class BestPlayerHand
	{
		public HandsSet Hand { get; set; }
		public List<Card> Cards { get; set; } = new();
	}
}