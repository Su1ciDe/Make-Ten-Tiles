using DG.Tweening;
using Fiber.Managers;
using GridSystem.Tiles;
using Lofelt.NiceVibrations;
using TriInspector;
using UnityEngine;

namespace Obstacles
{
	[DeclareFoldoutGroup("Properties")]
	public class IceObstacle : BaseObstacle
	{
		public override bool IsBlockingMovement { get; } = true;

		// [field: SerializeField, ReadOnly, Group("Properties")] public int ASD { get; private set; }

		private int currentHealth;

		private const int HEALTH = 2;

		private void Awake()
		{
			Tile.OnTappedToTile += OnAnyTileTapped;
			currentHealth = HEALTH;
		}

		private void OnDestroy()
		{
			Tile.OnTappedToTile -= OnAnyTileTapped;
		}

		private void OnAnyTileTapped(Tile tile)
		{
			if (AttachedTile?.LayerBlockCount <= 0)
			{
				currentHealth--;

				if (currentHealth <= 0)
				{
					DestroyObstacle();
				}
			}
		}

		public override bool OnTapped()
		{
			HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.Warning);

			AttachedTile.SetInteractable(false);
			AttachedTile.transform.DOKill();
			AttachedTile.transform.DOShakeRotation(.5f, 10 * Vector3.up, 10, 2, false, ShakeRandomnessMode.Harmonic).SetEase(Ease.InQuart).OnComplete(() => AttachedTile.SetInteractable(true));

			return !IsBlockingMovement;
		}

		public override void DestroyObstacle()
		{
			//TODO: Add some visual effects
			base.DestroyObstacle();
		}
	}
}