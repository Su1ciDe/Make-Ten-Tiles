using DG.Tweening;
using Fiber.Managers;
using Lofelt.NiceVibrations;
using UnityEngine;

namespace Obstacles
{
	public class CageObstacle : BaseObstacle
	{
		public override bool IsBlockingMovement { get; } = true;

		[SerializeField] private float unlockMoveDuration = 0.25f;

		[SerializeField] private GameObject blocker;

		private void Start()
		{
			CheckForBlocker();
		}

		public override bool OnTapped()
		{
			HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.Warning);

			AttachedTile.SetInteractable(false);
			AttachedTile.transform.DOKill();
			AttachedTile.transform.DOShakeRotation(.35f, 10 * Vector3.up, 15, 2, false, ShakeRandomnessMode.Harmonic).SetEase(Ease.InQuart).OnComplete(() => AttachedTile.SetInteractable(true));

			return !IsBlockingMovement;
		}

		public void Unlock(KeyObstacle key)
		{
			transform.DOScale(0, unlockMoveDuration).SetEase(Ease.InBack).OnComplete(DestroyObstacle);
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