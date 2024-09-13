using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fiber.Managers;
using Fiber.Utilities;
using Fiber.Utilities.Extensions;
using GamePlay.Player;
using GridSystem.Tiles;
using Obstacles;
using TriInspector;
using UnityEditor;
using UnityEngine;

namespace GridSystem
{
	[DeclareFoldoutGroup("Properties")]
	[DeclareFoldoutGroup("Randomizer")]
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

		private int totalTileCount;

		private const int BLAST_COUNT = 3;

		private void Awake()
		{
			SetupTileBlockers();

			totalTileCount = gridCells.GetTileCount();
		}

		private void Start()
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, GridCells.GetLength(0) * Tile.TILE_HEIGHT);
			Player.Instance.CanInput = false;
			transform.DOMoveX(15, .35f).From().SetEase(Ease.OutBack).OnComplete(() => Player.Instance.CanInput = true);
		}

		private void OnEnable()
		{
			Tile.OnTilePlaced += OnTilePlaced;
		}

		private void OnDisable()
		{
			Tile.OnTilePlaced -= OnTilePlaced;
		}

		private void OnTilePlaced(Tile tile)
		{
			totalTileCount--;

			if (checkWinCoroutine is not null)
				StopCoroutine(checkWinCoroutine);

			checkWinCoroutine = StartCoroutine(CheckWin());
		}

		private Coroutine checkWinCoroutine;

		private IEnumerator CheckWin()
		{
			yield return new WaitForSeconds(0.5f);
			if (totalTileCount <= 0)
				LevelManager.Instance.Win();
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

		public Transform GetLayerTransform(int layerIndex)
		{
			return cellHolder.GetChild(layerIndex);
		}

		public T FindObstacle<T>(bool isReversed = true) where T : BaseObstacle
		{
			T obstacle;
			if (isReversed)
			{
				for (int i = gridCells.GetLength(0) - 1; i >= 0; i--)
				{
					obstacle = GetObstacle<T>(i);
					if (obstacle) return obstacle;
				}
			}
			else
			{
				for (int i = 0; i < gridCells.GetLength(0); i++)
				{
					obstacle = GetObstacle<T>(i);
					if (obstacle) return obstacle;
				}
			}

			return null;

			TO GetObstacle<TO>(int i) where TO : BaseObstacle
			{
				for (int x = 0; x < gridCells[i].GetLength(0); x++)
					for (int y = 0; y < gridCells[i].GetLength(1); y++)
						if (gridCells[i, x, y].CurrentTile && gridCells[i, x, y].CurrentTile.Obstacle && gridCells[i, x, y].CurrentTile.Obstacle is TO obs)
							return obs;

				return null;
			}
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

			// Layers
			for (int i = 0; i < grid.Length; i++)
			{
				sizes.Add(grid[i].GridSize);
				var xOffset = (nodeSize.x * sizes[i].x + xSpacing * (sizes[i].x - 1)) / 2f - nodeSize.x / 2f;
				var yOffset = (nodeSize.y * sizes[i].y + ySpacing * (sizes[i].y - 1)) / 2f - nodeSize.y / 2f;

				gridCells.Matrices.Add(new GridCell3D.GridCellMatrix(sizes[i].x, sizes[i].y));

				var layer = new GameObject("Layer_" + (i + 1)).transform;
				layer.SetParent(cellHolder);
				layer.localPosition = new Vector3(0, i * Tile.TILE_HEIGHT, 0);
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

			transform.position = new Vector3(transform.position.x, transform.position.y, gridCells.GetLength(0) * Tile.TILE_HEIGHT);
		}

		[Button]
		private void ClearGrid()
		{
			cellHolder.DestroyImmediateChildren();
		}

		private void OnValidate()
		{
			var totalWeight = 0;
			foreach (var gridSpawnerOptions in randomizer)
			{
				totalWeight += gridSpawnerOptions.Weight;
			}

			foreach (var gridSpawnerOption in randomizer)
			{
				gridSpawnerOption.Percentage = ((float)gridSpawnerOption.Weight / totalWeight * 100).ToString("F2") + "%";
			}

			if (gridCells is not null)
			{
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

							if (!Application.isPlaying)
							{
								SceneVisibilityManager.instance.DisablePicking(gridCell.CurrentTile.gameObject, true);
							}
						}
					}
				}
			}
		}

		#region Randomizer

		[System.Serializable]
		[DeclareHorizontalGroup("Colors")]
		private class Randomizer
		{
			[Group("Colors")] public TileType Color;
			[Group("Colors")] public int Weight = 1;
			[Group("Colors"), DisplayAsString, HideLabel] public string Percentage;
		}

		[ValidateInput(nameof(ValidateRandomizerGrid))]
		[Group("Randomizer")] [SerializeField] private Array2DCell[] randomGrids;
		[ValidateInput(nameof(ValidateRandomizer))]
		[Group("Randomizer")] [SerializeField] private Randomizer[] randomizer;

		[Group("Randomizer"), Button]
		private void Randomize()
		{
			if (!CheckCanRandom())
			{
				Debug.Log("Not enough cells to complete level!");
				return;
			}

			ClearGrid();
			var randomWeights = randomizer.Select(x => x.Weight).ToArray();

			sizes = new List<Vector2Int>();
			gridCells = new GridCell3D();

			for (int i = 0; i < randomGrids.Length; i++)
			{
				sizes.Add(randomGrids[i].GridSize);
				var xOffset = (nodeSize.x * sizes[i].x + xSpacing * (sizes[i].x - 1)) / 2f - nodeSize.x / 2f;
				var yOffset = (nodeSize.y * sizes[i].y + ySpacing * (sizes[i].y - 1)) / 2f - nodeSize.y / 2f;

				gridCells.Matrices.Add(new GridCell3D.GridCellMatrix(sizes[i].x, sizes[i].y));

				var layer = new GameObject("Layer_" + (i + 1)).transform;
				layer.SetParent(cellHolder);
				layer.localPosition = new Vector3(0, i * Tile.TILE_HEIGHT, 0);
				for (int y = 0; y < sizes[i].y; y++)
				{
					for (int x = 0; x < sizes[i].x; x++)
					{
						var cell = (GridCell)PrefabUtility.InstantiatePrefab(GameManager.Instance.PrefabsSO.GridCellPrefab, layer.transform);
						cell.transform.localPosition = new Vector3(x * (nodeSize.x + xSpacing) - xOffset, -y * (nodeSize.y + ySpacing) + yOffset);
						cell.transform.localRotation = transform.rotation;
						cell.gameObject.name = x + " - " + y;
						gridCells[i, x, y] = cell;

						if (randomGrids[i].GetCell(x, y) == CellType.Filled)
						{
							var tileType = randomizer.WeightedRandom(randomWeights).Color;
							cell.Setup(i, x, y, tileType);
						}
						else
						{
							cell.Setup(i, x, y, TileType.Empty);
						}
					}
				}
			}

			TryToSolveRandom(randomWeights);

			SetupTileBlockers();

			transform.position = new Vector3(transform.position.x, transform.position.y, GridCells.GetLength(0) * Tile.TILE_HEIGHT);
		}

		private void TryToSolveRandom(int[] randomWeights)
		{
			var dict = new Dictionary<(int i, int x, int y), (int i, int x, int y)>();

			for (int i = gridCells.GetLength(0) - 1; i >= 0; i--)
			{
				for (int x = 0; x < gridCells[i].GetLength(0); x++)
				{
					for (int y = 0; y < gridCells[i].GetLength(1); y++)
					{
						if (!gridCells[i, x, y].CurrentTile) continue;
						if (dict.ContainsKey((i, x, y))) continue;
						if (dict.ContainsValue((i, x, y))) continue;

						dict.Add((i, x, y), GetTen(gridCells[i, x, y].CurrentTile));
					}
				}
			}

			var notTens = dict.Where(x => x.Value == (-1, -1, -1)).Select(x => x.Key);
			var notTensCoordinates = notTens as (int i, int x, int y)[] ?? notTens.ToArray();
			foreach (var (i, x, y) in notTensCoordinates)
			{
				var cell = gridCells[i, x, y];
				var tileType = randomizer.WeightedRandom(randomWeights).Color;
				cell.ClearTile(false);
				cell.Setup(i, x, y, tileType);
			}

			if (notTensCoordinates.Any())
			{
				TryToSolveRandom(randomWeights);
			}

			return;

			(int i, int x, int y) GetTen(Tile tile)
			{
				for (int i = gridCells.GetLength(0) - 1; i >= 0; i--)
				{
					for (int x = 0; x < gridCells[i].GetLength(0); x++)
					{
						for (int y = 0; y < gridCells[i].GetLength(1); y++)
						{
							if (!gridCells[i, x, y].CurrentTile) continue;
							if (gridCells[i, x, y].CurrentTile.Equals(tile)) continue;

							if ((int)tile.Type + (int)gridCells[i, x, y].CurrentTile.Type == GameManager.BLAST_COUNT && !dict.ContainsValue((i, x, y)) && !dict.ContainsKey((i, x, y)))
								return (i, x, y);
						}
					}
				}

				return (-1, -1, -1);
			}
		}

		private bool CheckCanRandom()
		{
			var totalCount = 0;
			foreach (var array2DCell in randomGrids)
			{
				for (int x = 0; x < array2DCell.GridSize.x; x++)
				{
					for (int y = 0; y < array2DCell.GridSize.y; y++)
					{
						if (array2DCell.GetCell(x, y) != CellType.Empty)
							totalCount++;
					}
				}
			}

			return totalCount % 2 == 0;
		}

		private TriValidationResult ValidateRandomizer()
		{
			int totalCount = 0;
			foreach (var r in randomizer)
			{
				if ((int)r.Color != 5)
					totalCount += (int)r.Color;
			}

			return totalCount % 10 != 0 ? TriValidationResult.Error("Cannot be solved!") : TriValidationResult.Valid;
		}

		private TriValidationResult ValidateRandomizerGrid()
		{
			return CheckCanRandom() ? TriValidationResult.Valid : TriValidationResult.Error("Cannot be solved!");
		}

		#endregion

#endif

		public void SetupTileBlockers()
		{
			if (!Application.isPlaying)
				ResetTileBlockers();

			for (int layerIndex = 1; layerIndex < gridCells.GetLength(0); layerIndex++)
			{
				var tilesLayerUp = gridCells[layerIndex];
				for (int downLayerIndex = layerIndex - 1; downLayerIndex >= 0; downLayerIndex--)
				{
					var tilesLayerDown = gridCells[downLayerIndex];
					var normalizeDiffX = tilesLayerDown.GetLength(0) - tilesLayerUp.GetLength(0);
					var coverTwoTileX = Mathf.Abs(normalizeDiffX) % 2 == 1;
					var normalizeDiffY = tilesLayerDown.GetLength(1) - tilesLayerUp.GetLength(1);
					var coverTwoTileY = Mathf.Abs(normalizeDiffY) % 2 == 1;

					normalizeDiffX = Mathf.FloorToInt(normalizeDiffX / 2f);
					normalizeDiffY = Mathf.FloorToInt(normalizeDiffY / 2f);

					for (var x = 0; x < tilesLayerUp.GetLength(0); x++)
					{
						for (var y = 0; y < tilesLayerUp.GetLength(1); y++)
						{
							var cell = tilesLayerUp[x, y];
							if (!cell.CurrentTile) continue;

							var coverX = x + normalizeDiffX;
							var coverY = y + normalizeDiffY;

							if (coverX > tilesLayerDown.GetLength(0) || coverY > tilesLayerDown.GetLength(1)) continue;

							if (coverX >= 0 && coverX < tilesLayerDown.GetLength(0) && coverY >= 0 && coverY < tilesLayerDown.GetLength(1))
							{
								var coverCell = gridCells[downLayerIndex, coverX, coverY];
								if (coverCell && coverCell.CurrentTile)
									cell.CurrentTile.RegisterBlocker(coverCell.CurrentTile);
							}
							else // If the down layer's grid is smaller than the upper layer
							{
								if (coverX == tilesLayerDown.GetLength(0) && coverY >= 0 && IsInGrid(downLayerIndex, coverX - 1, coverY))
								{
									var coverCell = gridCells[downLayerIndex, coverX - 1, coverY];
									if (coverCell && coverCell.CurrentTile)
										cell.CurrentTile.RegisterBlocker(coverCell.CurrentTile);
								}
								else if (coverY == tilesLayerDown.GetLength(1) && coverX >= 0 && IsInGrid(downLayerIndex, coverX, coverY - 1))
								{
									var coverCell = gridCells[downLayerIndex, coverX, tilesLayerDown.GetLength(1) - 1];
									if (coverCell && coverCell.CurrentTile)
										cell.CurrentTile.RegisterBlocker(coverCell.CurrentTile);
								}
							}

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

		private void ResetTileBlockers()
		{
			for (int i = 0; i < gridCells.GetLength(0); i++)
			{
				for (int x = 0; x < gridCells[i].GetLength(0); x++)
				{
					for (int y = 0; y < gridCells[i].GetLength(1); y++)
					{
						var cell = gridCells[i, x, y];
						if (cell.CurrentTile)
							cell.CurrentTile.ResetBlockers();
					}
				}
			}
		}

		#endregion
	}
}