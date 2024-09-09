using Array2DEditor;
using UnityEditor;

namespace GridSystem
{
	public class Array2CellDrawer
	{
		[CustomPropertyDrawer(typeof(Array2DCell))]
		public class Array2DExampleEnumDrawer : Array2DEnumDrawer<CellType>
		{
		}
	}
}