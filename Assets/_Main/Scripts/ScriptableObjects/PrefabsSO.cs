using Fiber.Utilities;
using GridSystem;
using GridSystem.Tiles;
using HolderSystem;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Prefabs", menuName = "Make Ten Tiles/Prefabs", order = 0)]
	public class PrefabsSO : SerializedScriptableObject
	{
		public GridCell GridCellPrefab;
		public Tile TilePrefab;

		[Space]
		public HolderSlot HolderSlotPrefab;
		public HolderGroup HolderGroupPrefab;
	}
}