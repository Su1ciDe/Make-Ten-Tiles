using Array2DEditor;
using UnityEngine;

namespace GridSystem
{
	[System.Serializable]
	public class Array2DGrid : Array2D<TileType>
	{
		[SerializeField] private CellRowGrid[] cells = new CellRowGrid[Consts.defaultGridSize];

		protected override CellRow<TileType> GetCellRow(int idx)
		{
			return cells[idx];
		}
	}

	[System.Serializable]
	public class CellRowGrid : CellRow<TileType>
	{
	}
}