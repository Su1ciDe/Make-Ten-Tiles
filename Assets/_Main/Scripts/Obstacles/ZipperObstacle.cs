using Cysharp.Threading.Tasks;
using Fiber.Managers;
using GridSystem.Tiles;
using Lofelt.NiceVibrations;
using TriInspector;
using UnityEngine;

namespace Obstacles
{
	public class ZipperObstacle : BaseObstacle
	{
		[field: SerializeField, ReadOnly, Group("Properties")] public Tile OtherAttachedTile { get; set; }
		public override bool IsBlockingMovement { get; } = true;

		public void Setup(Tile tile, Tile otherTile)
		{
			AttachedTile = tile;
			OtherAttachedTile = otherTile;

			otherTile.Obstacle = tile.Obstacle;

			transform.position = (tile.transform.position + otherTile.transform.position) / 2f;
			transform.rotation = Quaternion.LookRotation(otherTile.transform.position - tile.transform.position, Vector3.back);
		}

		public override bool OnTapped()
		{
			HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.RigidImpact);

			GlueAnimation();

			return !IsBlockingMovement;
		}

		private async void GlueAnimation()
		{
			AttachedTile.IsInDeck = true;
			OtherAttachedTile.IsInDeck = true;

			await UniTask.WaitForSeconds(1);

			DestroyObstacle();
			AttachedTile.MoveTileToHolder();
			OtherAttachedTile.MoveTileToHolder();
		}
	}
}