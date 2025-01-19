using System;

namespace _Scripts.Enums
{
	[Flags]
	public enum ActionButtonStages
	{
		None = 0,
		Fold = 1,
		Check = 2,
		Call = 4,
		Bet = 8,
		Raise = 16,
		AllIn = 32,
	}
}