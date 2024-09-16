using Cysharp.Threading.Tasks;
using DG.Tweening;
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
		[SerializeField] private GameObject blocker;

		private static readonly int unzip = Animator.StringToHash("Unzip");
		private const float UNZIP_DURATION = .25f;

		private void Start()
		{
			CheckForBlocker();
		}

		public void Setup(Tile tile, Tile otherTile)
		{
			AttachedTile = tile;
			OtherAttachedTile = otherTile;

			otherTile.Obstacle = tile.Obstacle;

			transform.position = (tile.transform.position + otherTile.transform.position) / 2f;
			transform.rotation = Quaternion.LookRotation(otherTile.transform.position - tile.transform.position, Vector3.back);

			CheckForBlocker();
		}

		public override bool OnTapped()
		{
			if (AttachedTile.IsInDeck || OtherAttachedTile.IsInDeck) return false;
			if (OtherAttachedTile.LayerBlockCount > 0 || AttachedTile.LayerBlockCount > 0)
			{
				var otherAttachedTileOffset = OtherAttachedTile.transform.position - transform.position;
				var attachedTileOffset = AttachedTile.transform.position - transform.position;

				this.DOComplete();

				AttachedTile.SetInteractable(false);
				OtherAttachedTile.SetInteractable(false);
				DOTween.Punch(() => transform.position, x =>
				{
					transform.position = x;
					OtherAttachedTile.transform.position = x + otherAttachedTileOffset;
					AttachedTile.transform.position = x + attachedTileOffset;
				}, 0.1f * new Vector2(Random.Range(0, 2) * 2 - 1, Random.Range(0, 2) * 2 - 1), 0.35f).SetTarget(this).OnComplete(() =>
				{
					AttachedTile.SetInteractable(true);
					OtherAttachedTile.SetInteractable(true);
				});
				return false;
			}

			HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.RigidImpact);

			GlueAnimation();

			return !IsBlockingMovement;
		}

		private async void GlueAnimation()
		{
			animator.SetTrigger(unzip);

			AttachedTile.IsInDeck = true;
			OtherAttachedTile.IsInDeck = true;
			AttachedTile.SetInteractable(false);
			OtherAttachedTile.SetInteractable(false);

			await UniTask.WaitForSeconds(UNZIP_DURATION);

			DestroyObstacle();
			AttachedTile.MoveTileToHolder();
			OtherAttachedTile.MoveTileToHolder();
		}

		private void CheckForBlocker()
		{
			if (OtherAttachedTile.LayerBlockCount > 0 || AttachedTile.LayerBlockCount > 0)
			{
				SetBlocker(true);
			}
		}

		public void SetBlocker(bool isBlocked)
		{
			blocker.SetActive(isBlocked);
		}
	}
}