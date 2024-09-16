using DG.Tweening;
using Fiber.Managers;
using GridSystem;
using GridSystem.Tiles;
using Lofelt.NiceVibrations;
using UnityEngine;

namespace Obstacles
{
	public class KeyObstacle : BaseObstacle
	{
		public override bool IsBlockingMovement { get; } = false;

		[SerializeField] private float unlockMoveDuration = 0.5f;
		[SerializeField] private ParticleSystem keyParticle;
		[SerializeField] private GameObject blocker;

		private void Awake()
		{
			AttachedTile.OnTileRemoved += OnTileRemoved;
		}

		private void Start()
		{
			CheckForBlocker();
		}

		private void OnDestroy()
		{
			transform.DOKill();
		}

		private void OnTileRemoved(Tile tile)
		{
			AttachedTile.OnTileRemoved -= OnTileRemoved;

			HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.RigidImpact);
			keyParticle.gameObject.SetActive(true);

			var cage = GridManager.Instance.FindObstacle<CageObstacle>();
			if (cage)
			{
				transform.SetParent(LevelManager.Instance.CurrentLevel.transform);
				transform.DOMove(cage.transform.position, unlockMoveDuration).SetEase(Ease.InOutQuart).OnComplete(() =>
				{
					cage.Unlock(this);
					DestroyObstacle();
				});
			}
		}

		public override bool OnTapped()
		{
			return !IsBlockingMovement;
		}

		private void CheckForBlocker()
		{
			if (AttachedTile.LayerBlockCount > 0)
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