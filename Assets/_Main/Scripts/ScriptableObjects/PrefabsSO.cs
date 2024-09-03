using Fiber.Utilities;
using GridSystem;
using GridSystem.Tiles;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Prefabs", menuName = "Make Ten Tiles/Prefabs", order = 0)]
	public class PrefabsSO : SerializedScriptableObject
	{
		public GridCell GridCellPrefab;
		public Tile TilePrefab;
	}
}