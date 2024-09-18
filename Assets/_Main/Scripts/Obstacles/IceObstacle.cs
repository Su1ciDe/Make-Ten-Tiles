using Cysharp.Threading.Tasks;
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

		[SerializeField] private Rigidbody[] fractures;
		[SerializeField] private float explosionForce;
		[SerializeField] private float explosionRadius;

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
			if (!(AttachedTile?.LayerBlockCount <= 0)) return;

			currentHealth--;

			if (currentHealth <= 0)
				DestroyObstacle();

			DestroyFractures();
		}

		private void DestroyFractures()
		{
			const int count = 2;
			for (int i = (HEALTH - currentHealth - 1) * count; i < (HEALTH - currentHealth) * count; i++)
			{
				var fracture = fractures[i];
				fracture.isKinematic = false;
				fracture.AddExplosionForce(explosionForce, transform.position, explosionRadius);
				fracture.transform.DOScale(0.1f, .5f).SetDelay(0.5f).OnComplete(() =>
				{
					fracture.isKinematic = true;
					fracture.gameObject.SetActive(false);
				});
			}
		}

		public override bool OnTapped()
		{
			HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.Warning);

			AttachedTile.SetInteractable(false);
			AttachedTile.transform.DOKill();
			AttachedTile.transform.DOShakeRotation(.35f, 10 * Vector3.up, 15, 2, false, ShakeRandomnessMode.Harmonic).SetEase(Ease.InQuart).OnComplete(() => AttachedTile.SetInteractable(true));

			return !IsBlockingMovement;
		}

		public override async void DestroyObstacle()
		{
			Tile.OnTappedToTile -= OnAnyTileTapped;
			AttachedTile.Obstacle = null;

			await UniTask.WaitForSeconds(1.1f);

			base.DestroyObstacle();
		}
	}
}