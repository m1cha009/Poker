using System.Collections.Generic;
using _Scripts.Data;
using _Scripts.Enums;

namespace _Scripts
{
	public class BestPlayerHand
	{
		public HandsSet Hand { get; set; }
		public List<CardData> Cards { get; set; } = new();
	}
}