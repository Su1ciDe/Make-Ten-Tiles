using System.Collections.Generic;

namespace GridSystem
{
	[System.Serializable]
	public class GridCell3D
	{
		public List<GridCellMatrix> Matrices;
		public int Count => count;
		private int count;

		public GridCell this[int x, int y, int z]
		{
			get => Matrices[x][y, z];
			set => Matrices[x][y, z] = value;
		}
		public GridCellMatrix this[int x]
		{
			get => Matrices[x];
			set => Matrices[x] = value;
		}

		public GridCell3D(int index0, int index1, int index2)
		{
			Matrices = new List<GridCellMatrix>();
			for (int i = 0; i < index0; i++)
				Matrices[i] = new GridCellMatrix(index1, index2);

			CalculateCount();
		}

		public GridCell3D()
		{
			Matrices = new List<GridCellMatrix>();
		}

		public int GetLength(int dimension)
		{
			return dimension switch
			{
				0 => Matrices.Count,
				1 => Matrices[0].Arrays.Length,
				2 => Matrices[0].Arrays[0].Cells.Length,
				_ => 0
			};
		}

		private void CalculateCount()
		{
			count = 0;
			foreach (var matrix in Matrices)
			{
				foreach (var array in matrix.Arrays)
					count += array.Cells.Length;
			}
		}

		public int GetTileCount()
		{
			var tileCount = 0;
			foreach (var matrix in Matrices)
			{
				foreach (var array in matrix.Arrays)
				{
					foreach (var cell in array.Cells)
					{
						if (cell.CurrentTile is not null)
							tileCount++;
					}
				}
			}

			return tileCount;
		}

		[System.Serializable]
		public class GridCellMatrix
		{
			public GridCellArray[] Arrays;
			public GridCell this[int x, int y]
			{
				get => Arrays[x][y];
				set => Arrays[x][y] = value;
			}

			public GridCellMatrix(int index0, int index1)
			{
				Arrays = new GridCellArray[index0];
				for (int i = 0; i < index0; i++)
					Arrays[i] = new GridCellArray(index1);
			}

			public int GetLength(int dimension)
			{
				return dimension switch
				{
					0 => Arrays.Length,
					1 => Arrays[0].Cells.Length,
					_ => 0
				};
			}
		}

		[System.Serializable]
		public class GridCellArray
		{
			public GridCell[] Cells;
			public GridCell this[int index]
			{
				get => Cells[index];
				set => Cells[index] = value;
			}

			public GridCellArray(int index0)
			{
				Cells = new GridCell[index0];
			}
		}
	}
}