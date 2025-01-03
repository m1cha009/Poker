using UnityEngine;

namespace _Scripts.SO
{
	[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
	public class GameData : ScriptableObject
	{
		[field: SerializeField] public int PlayerAmount { get; set; }
	}
}
