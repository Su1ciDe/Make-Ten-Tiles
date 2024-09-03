using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GridSystem.Tiles
{
	[DeclareFoldoutGroup("Properties")]
	public class Tile : MonoBehaviour
	{
		[field: Title("Properties")]
		[field: SerializeField, ReadOnly, Group("Properties")] public GridCell CurrentCell { get; private set; }
		[field: SerializeField, ReadOnly, Group("Properties")] public int BlockCount { get; private set; }

		[Title("References")]
		[SerializeField] private GameObject blockImage;
		[SerializeField] private Collider col;
		[SerializeField] private TextMeshPro txtAmount;

		public event UnityAction<Tile> OnTileRemoved;

		public void Setup(CellType cellType, GridCell cell)
		{
			txtAmount.SetText(((int)cellType).ToString());
			CurrentCell = cell;
			CurrentCell.CurrentTile = this;
		}

		#region Blocker

		public void RegisterBlocker(Tile downTile)
		{
			OnTileRemoved += downTile.OnBlockerRemoved;

			downTile.AddBlockerCount();
		}

		private void OnBlockerRemoved(Tile blockerTile)
		{
			blockerTile.OnTileRemoved -= OnBlockerRemoved;

			BlockCount--;
			SetBlockState();
		}

		public void SetBlockState()
		{
			if (BlockCount > 0)
			{
				blockImage.SetActive(true);
				col.enabled = false;
			}
			else
			{
				blockImage.SetActive(false);
				col.enabled = true;
			}
		}

		public void AddBlockerCount()
		{
			BlockCount++;
			SetBlockState();
		}

		#endregion
	}
}