using Array2DEditor;
using UnityEngine;

namespace GridSystem
{
	[System.Serializable]
	public class Array2DCell : Array2D<CellType>
	{
		[SerializeField] private CellRowRandom[] cells = new CellRowRandom[Consts.defaultGridSize];

		protected override CellRow<CellType> GetCellRow(int idx)
		{
			return cells[idx];
		}
	}

	[System.Serializable]
	public class CellRowRandom : CellRow<CellType>
	{
	}
}