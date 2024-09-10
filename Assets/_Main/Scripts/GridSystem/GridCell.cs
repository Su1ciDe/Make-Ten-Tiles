using System.Collections.Generic;
using Fiber.Managers;
using Fiber.Utilities;
using GridSystem.Tiles;
using Obstacles;
using TriInspector;
using UnityEditor;
using UnityEngine;

namespace GridSystem
{
	[SelectionBase]
	[DeclareFoldoutGroup("Properties")]
	[DeclareBoxGroup("Editor")]
	[DeclareToggleGroup("Editor/Obstacles", Title = "Obstacle")]
	public class GridCell : MonoBehaviour
	{
		[field: SerializeField, ReadOnly, Group("Properties")] public Tile CurrentTile { get; set; }
		[field: SerializeField, ReadOnly, Group("Properties")] public Vector2Int Coordinates { get; set; }
		[field: SerializeField, ReadOnly, Group("Properties")] public int LayerIndex { get; set; }

		[Title("References")]
		[SerializeField] private Transform tileHolder;

		#region Setup

#if UNITY_EDITOR
		[Group("Editor")] [SerializeField] private TileType tileType;

		[Group("Editor/Obstacles")] [SerializeField] private bool hasObstacle;
		[Group("Editor/Obstacles"), Dropdown(nameof(GetObstacles))] [SerializeField] private BaseObstacle obstacle;
		[Group("Editor/Obstacles"), ShowIf(nameof(IsGlueObstacle))] [SerializeField] private GridCell gluedCell;

		[Button(ButtonSizes.Medium), Group("Editor")]
		private void SetupTile()
		{
			if (CurrentTile)
			{
				DestroyImmediate(CurrentTile.gameObject);
				CurrentTile = null;
			}

			CurrentTile = (Tile)PrefabUtility.InstantiatePrefab(GameManager.Instance.PrefabsSO.TilePrefab, tileHolder);
			CurrentTile.Setup(tileType, this);

			if (hasObstacle)
			{
				CurrentTile.SetupObstacle(obstacle);

				// If the obstacle is a GlueObstacle
				if (IsGlueObstacle() && gluedCell.CurrentTile)
				{
					var glueObstacle = (GlueObstacle)CurrentTile.Obstacle;
					glueObstacle.Setup(CurrentTile, gluedCell.CurrentTile);
				}
			}

			GridManager.Instance.SetupTileBlockers();
		}

		[Button, Group("Editor")]
		public void ClearTile()
		{
			if (CurrentTile)
			{
				DestroyImmediate(CurrentTile.gameObject);
				CurrentTile = null;
			}

			GridManager.Instance.SetupTileBlockers();
		}

		public void Setup(int layerIndex, int x, int y, TileType _tileType)
		{
			LayerIndex = layerIndex;
			Coordinates = new Vector2Int(x, y);
			this.tileType = _tileType;

			if (_tileType != TileType.Empty)
			{
				var tile = (Tile)PrefabUtility.InstantiatePrefab(GameManager.Instance.PrefabsSO.TilePrefab, tileHolder);
				tile.transform.localPosition = Vector3.zero;
				tile.Setup(_tileType, this);

				SceneVisibilityManager.instance.DisablePicking(tile.gameObject, true);
			}
		}

		private IEnumerable<BaseObstacle> GetObstacles()
		{
			const string path = "Assets/_Main/Prefabs/Obstacles";
			var obstacles = EditorUtilities.LoadAllAssetsFromPath<BaseObstacle>(path);
			return obstacles;
		}

		private bool IsGlueObstacle() => obstacle is GlueObstacle;

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(transform.position, new Vector3(1, 1));
		}
#endif

		#endregion
	}
}