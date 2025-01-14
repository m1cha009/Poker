using UnityEngine;

namespace _Scripts.SO
{
	[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
	public class GameDataSo : ScriptableObject
	{
		[field: SerializeField] public int PlayerAmount { get; set; }
	}
}
