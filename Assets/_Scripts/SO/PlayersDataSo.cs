using System.Collections.Generic;
using _Scripts.Data;
using UnityEngine;

namespace _Scripts.SO
{
	[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/Player Data", order = 0)]
	public class PlayersDataSo : ScriptableObject
	{
		[field: SerializeField] public List<PlayerData> PlayersData { get; private set; }
	}
}