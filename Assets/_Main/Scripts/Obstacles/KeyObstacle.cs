using GridSystem;
using GridSystem.Tiles;
using UnityEngine;

namespace Obstacles
{
	public class KeyObstacle : BaseObstacle
	{
		public override bool IsBlockingMovement { get; } = false;

		private void Awake()
		{
			AttachedTile.OnTileRemoved += OnTileRemoved;
		}

		private void OnTileRemoved(Tile tile)
		{
			AttachedTile.OnTileRemoved -= OnTileRemoved;

			var cage = GridManager.Instance.FindObstacle<CageObstacle>();
			if (cage)
			{
				cage.Unlock(this);
			}
		}

		public override bool OnTapped()
		{
			return !IsBlockingMovement;
		}
	}
}