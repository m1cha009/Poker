using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.SO
{
	[CreateAssetMenu(fileName = "PlayerNames", menuName = "Scriptable Objects/PlayerNames", order = 0)]
	public class PlayerNamesSo : ScriptableObject
	{
		public List<string> PlayerNames;
	}
}