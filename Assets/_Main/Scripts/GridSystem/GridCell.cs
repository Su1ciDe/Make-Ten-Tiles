using Fiber.Managers;
using GridSystem.Tiles;
using TriInspector;
using UnityEditor;
using UnityEngine;

namespace GridSystem
{
	[SelectionBase]
	[DeclareFoldoutGroup("Properties")]
	public class GridCell : MonoBehaviour
	{
		[field: SerializeField, ReadOnly, Group("Properties")] public Tile CurrentTile { get; set; }
		[field: SerializeField, ReadOnly, Group("Properties")] public Vector2Int Coordinates { get; set; }
		[field: SerializeField, ReadOnly, Group("Properties")] public int LayerIndex { get; set; }

		[Title("References")]
		[SerializeField] private Transform tileHolder;

		#region MyRegion

#if UNITY_EDITOR
		public void Setup(int layerIndex, int x, int y, CellType cellType)
		{
			LayerIndex = layerIndex;
			Coordinates = new Vector2Int(x, y);

			if (cellType != CellType.Empty)
			{
				var tile = (Tile)PrefabUtility.InstantiatePrefab(GameManager.Instance.PrefabsSO.TilePrefab, tileHolder);
				tile.transform.localPosition = Vector3.zero;
				tile.Setup(cellType, this);

				SceneVisibilityManager.instance.DisablePicking(tile.gameObject, true);
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(transform.position, new Vector3(1, 1));
		}
#endif

		#endregion
	}
}