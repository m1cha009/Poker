using System;
using UnityEngine;

namespace _Scripts.Data
{
	[Serializable]
	public class PlayerData
	{
		[field: SerializeField] public string Name { get; private set; }
		[field: SerializeField] public float Money { get; private set; }
	}
}