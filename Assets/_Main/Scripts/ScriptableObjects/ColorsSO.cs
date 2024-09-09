using AYellowpaper.SerializedCollections;
using GridSystem;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Colors", menuName = "Make Ten Tiles/Colors", order = 1)]
	public class ColorsSO : ScriptableObject
	{
		public SerializedDictionary<TileType, Material> Materials = new SerializedDictionary<TileType, Material>();
	}
}