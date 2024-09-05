using System.Collections.Generic;
using Fiber.Managers;
using Fiber.Utilities;
using Fiber.Utilities.Extensions;
using GridSystem.Tiles;
using TriInspector;
using UnityEditor;
using UnityEngine;

namespace GridSystem
{
	[DeclareFoldoutGroup("Properties")]
	public class GridManager : Singleton<GridManager>
	{
		[SerializeField, ReadOnly, Group("Properties")] private GridCell3D gridCells;
		public GridCell3D GridCells => gridCells;

		[Title("Grid Settings")]
		[SerializeField] private Vector2 nodeSize = new Vector2(1, 1);
		[SerializeField] private float xSpacing = .1f;
		[SerializeField] private float ySpacing = .1f;

		[Title("References")]
		[SerializeField] private Transform cellHolder;

		[Title("Editor")]
		[SerializeField] private Array2DGrid[] grid;

		[SerializeField, HideInInspector] private List<Vector2Int> sizes = new List<Vector2Int>();

		private const int BLAST_COUNT = 3;

		private void Awake()
		{
			SetupTileBlockers();
		}

		#region Helpers

		public bool IsInGrid(int layerIndex, int x, int y)
		{
			return x >= 0 && x < gridCells[layerIndex].GetLength(0) && y >= 0 && y < gridCells[layerIndex].GetLength(1);
		}

		public GridCell GetCell(int layerIndex, int x, int y)
		{
			return IsInGrid(layerIndex, x, y) ? gridCells[layerIndex, x, y] : null;
		}

		public GridCell GetCell(int layerIndex, Vector2Int coordinates)
		{
			return IsInGrid(layerIndex, coordinates.x, coordinates.y) ? gridCells[layerIndex, coordinates.x, coordinates.y] : null;
		}

		public GridCell GetCell(Vector3Int coordinates)
		{
			return IsInGrid(coordinates.x, coordinates.y, coordinates.z) ? gridCells[coordinates.x, coordinates.y, coordinates.z] : null;
		}

		public Tile GetTile(int layerIndex, int x, int y)
		{
			return GetCell(layerIndex, x, y)?.CurrentTile;
		}

		public Tile GetTile(int layerIndex, Vector2Int coordinates)
		{
			return GetCell(layerIndex, coordinates)?.CurrentTile;
		}

		public Tile GetTile(Vector3Int coordinates)
		{
			return GetCell(coordinates)?.CurrentTile;
		}

		#endregion

		#region Setup

#if UNITY_EDITOR

		[Button(ButtonSizes.Large)]
		private void Setup()
		{
			ClearGrid();

			sizes = new List<Vector2Int>();
			gridCells = new GridCell3D();

			for (int i = 0; i < grid.Length; i++)
			{
				sizes.Add(grid[i].GridSize);
				var xOffset = (nodeSize.x * sizes[i].x + xSpacing * (sizes[i].x - 1)) / 2f - nodeSize.x / 2f;
				var yOffset = (nodeSize.y * sizes[i].y + ySpacing * (sizes[i].y - 1)) / 2f - nodeSize.y / 2f;

				gridCells.Matrices.Add(new GridCell3D.GridCellMatrix(sizes[i].x, sizes[i].y));

				var layer = new GameObject("Layer_" + (i + 1)).transform;
				layer.SetParent(cellHolder);
				layer.localPosition = new Vector3(layer.localPosition.x, i * Tile.TILE_HEIGHT, layer.localPosition.z);
				for (int y = 0; y < sizes[i].y; y++)
				{
					for (int x = 0; x < sizes[i].x; x++)
					{
						var gridCell = grid[i].GetCell(x, y);

						var cell = (GridCell)PrefabUtility.InstantiatePrefab(GameManager.Instance.PrefabsSO.GridCellPrefab, layer.transform);
						cell.transform.localPosition = new Vector3(x * (nodeSize.x + xSpacing) - xOffset, -y * (nodeSize.y + ySpacing) + yOffset);
						cell.transform.localRotation = transform.rotation;
						cell.gameObject.name = x + " - " + y;
						cell.Setup(i, x, y, gridCell);
						gridCells[i, x, y] = cell;
					}
				}
			}

			SetupTileBlockers();
		}

		public void SetupTileBlockers()
		{
			for (int layerIndex = 1; layerIndex < gridCells.GetLength(0); layerIndex++)
			{
				var tilesLayerUp = gridCells[layerIndex];
				for (int downLayerIndex = layerIndex - 1; downLayerIndex >= 0; downLayerIndex--)
				{
					var tilesLayerDown = gridCells[downLayerIndex];
					var normalizeDiffX = tilesLayerDown.GetLength(0) - tilesLayerUp.GetLength(0);
					var coverTwoTileX = normalizeDiffX % 2 >= 0;
					var normalizeDiffY = tilesLayerDown.GetLength(1) - tilesLayerUp.GetLength(1);
					var coverTwoTileY = normalizeDiffY % 2 >= 0;

					normalizeDiffX /= 2;
					normalizeDiffY /= 2;

					for (var x = 0; x < tilesLayerUp.GetLength(0); x++)
					{
						for (int y = 0; y < tilesLayerUp.GetLength(1); y++)
						{
							var cell = tilesLayerUp[x, y];
							if (!cell.CurrentTile) continue;

							var coverX = cell.Coordinates.x + normalizeDiffX;
							var coverY = cell.Coordinates.y + normalizeDiffY;

							if (coverX > tilesLayerDown.GetLength(0) || coverY > tilesLayerDown.GetLength(1)) continue;

							if (gridCells[downLayerIndex, coverX, coverY] && gridCells[downLayerIndex, coverX, coverY].CurrentTile)
								cell.CurrentTile.RegisterBlocker(gridCells[downLayerIndex, coverX, coverY].CurrentTile);

							if (coverTwoTileX && coverTwoTileY)
							{
								if (IsInGrid(downLayerIndex, coverX + 1, coverY) && gridCells[downLayerIndex, coverX + 1, coverY] && gridCells[downLayerIndex, coverX + 1, coverY].CurrentTile)
									cell.CurrentTile.RegisterBlocker(gridCells[downLayerIndex, coverX + 1, coverY].CurrentTile);
								if (IsInGrid(downLayerIndex, coverX, coverY + 1) && gridCells[downLayerIndex, coverX, coverY + 1] && gridCells[downLayerIndex, coverX, coverY + 1].CurrentTile)
									cell.CurrentTile.RegisterBlocker(gridCells[downLayerIndex, coverX, coverY + 1].CurrentTile);
								if (IsInGrid(downLayerIndex, coverX + 1, coverY + 1) && gridCells[downLayerIndex, coverX + 1, coverY + 1] &&
								    gridCells[downLayerIndex, coverX + 1, coverY + 1].CurrentTile)
									cell.CurrentTile.RegisterBlocker(gridCells[downLayerIndex, coverX + 1, coverY + 1].CurrentTile);
							}
							else if (coverTwoTileX && IsInGrid(downLayerIndex, coverX + 1, coverY) && gridCells[downLayerIndex, coverX + 1, coverY] &&
							         gridCells[downLayerIndex, coverX + 1, coverY].CurrentTile)
							{
								cell.CurrentTile.RegisterBlocker(gridCells[downLayerIndex, coverX + 1, coverY].CurrentTile);
							}
							else if (coverTwoTileY && IsInGrid(downLayerIndex, coverX, coverY + 1) && gridCells[downLayerIndex, coverX, coverY + 1] &&
							         gridCells[downLayerIndex, coverX, coverY + 1].CurrentTile)
							{
								cell.CurrentTile.RegisterBlocker(gridCells[downLayerIndex, coverX, coverY + 1].CurrentTile);
							}
						}
					}
				}
			}
		}

		[Button]
		private void ClearGrid()
		{
			cellHolder.DestroyImmediateChildren();
		}

		private void OnValidate()
		{
			if (gridCells is null) return;
			for (int i = 0; i < gridCells.GetLength(0); i++)
			{
				var cells = gridCells[i];
				for (int x = 0; x < cells.GetLength(0); x++)
				{
					for (int y = 0; y < cells.GetLength(1); y++)
					{
						var gridCell = GetCell(i, x, y);
						if (!gridCell) return;
						if (gridCell.CurrentTile is null) continue;

						SceneVisibilityManager.instance.DisablePicking(gridCell.CurrentTile.gameObject, true);
					}
				}
			}
		}
#endif

		#endregion
	}
}