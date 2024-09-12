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

		[SerializeField] private Animator animator;

		private static readonly int unzip = Animator.StringToHash("Unzip");
		private const float UNZIP_DURATION = .5f;

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
			if (OtherAttachedTile.LayerBlockCount > 0 || AttachedTile.LayerBlockCount > 0) return false;

			HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.RigidImpact);

			GlueAnimation();

			return !IsBlockingMovement;
		}

		private async void GlueAnimation()
		{
			animator.SetTrigger(unzip);

			AttachedTile.IsInDeck = true;
			OtherAttachedTile.IsInDeck = true;

			await UniTask.WaitForSeconds(UNZIP_DURATION);

			DestroyObstacle();
			AttachedTile.MoveTileToHolder();
			OtherAttachedTile.MoveTileToHolder();
		}
	}
}