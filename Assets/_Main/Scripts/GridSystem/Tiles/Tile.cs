using DG.Tweening;
using Fiber.Managers;
using Fiber.AudioSystem;
using Lofelt.NiceVibrations;
using Obstacles;
using TMPro;
using TriInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GridSystem.Tiles
{
	[DeclareFoldoutGroup("Properties")]
	public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		public static Tile CurrentTile;
		public bool IsInDeck { get; set; } = false;

		[field: Title("Properties")]
		[field: SerializeField, ReadOnly, Group("Properties")] public TileType Type { get; private set; }
		[field: SerializeField, ReadOnly, Group("Properties")] public GridCell CurrentCell { get; private set; }
		[field: SerializeField, ReadOnly, Group("Properties")] public int LayerBlockCount { get; private set; }
		[field: SerializeField, ReadOnly, Group("Properties")] public BaseObstacle Obstacle { get; set; }
		public bool IsCompleted { get; set; } = false;

		[Title("References")]
		[SerializeField] private GameObject layerBlocker;
		[SerializeField] private Transform obstacleHolder;
		[SerializeField] private new Renderer renderer;
		[SerializeField] private Collider col;
		[SerializeField] private TextMeshPro txtAmount;
		[SerializeField] private TrailRenderer trail;

		public static float JUMP_DURATION = .5F;
		private const float JUMP_POWER = 5;
		public static float BLAST_DURATION = .35F;

		private const float HIGHLIGHT_DURATION = .15F;
		private const float HIGHLIGHT_POS = .15F;
		private const float HIGHLIGHT_SCALE = 1.1F;

		public static float TILE_HEIGHT = .5F;

		public static event UnityAction<Tile> OnTappedToTile;
		public event UnityAction<Tile> OnTileRemoved;
		public static event UnityAction<Tile> OnTilePlaced;

		private void Awake()
		{
			SetupMaterials();
		}

		private void OnDestroy()
		{
			transform.DOKill();
		}

		public void Setup(TileType tileType, GridCell cell)
		{
			Type = tileType;
			txtAmount.SetText(((int)tileType).ToString());
			CurrentCell = cell;
			CurrentCell.CurrentTile = this;

			SetupMaterials();
		}

		private void SetupMaterials()
		{
			var mat = GameManager.Instance.ColorsSO.Materials[Type];
			var mats = Application.isPlaying ? renderer.materials : renderer.sharedMaterials;
			for (var i = 0; i < mats.Length; i++)
				mats[i] = mat;
			if (Application.isPlaying)
				renderer.materials = mats;
			else
				renderer.sharedMaterials = mats;

			trail.startColor = mat.color;
			var endColor = mat.color;
			endColor.a = 0;
			trail.endColor = endColor;
		}

		public Tween Jump(Vector3 pos)
		{
			transform.localScale = Vector3.one;
			return transform.DOLocalJump(pos, transform.position.y / 2f + JUMP_POWER, 1, JUMP_DURATION);
		}

		public void Blast()
		{
			Destroy(gameObject);
			// transform.DOShakeRotation(BLAST_DURATION, 4 * Vector3.forward, 25, 2, false, ShakeRandomnessMode.Harmonic).SetEase(Ease.InQuart).OnComplete(() => Destroy(gameObject));
			// return transform.DOScale(1.3f, BLAST_DURATION).SetEase(Ease.OutSine);
		}

		public void Highlight()
		{
			transform.DOComplete();
			transform.DOLocalMoveZ(HIGHLIGHT_POS, HIGHLIGHT_DURATION).SetRelative().SetEase(Ease.OutBack);
			transform.DOScale(HIGHLIGHT_SCALE, HIGHLIGHT_DURATION).SetEase(Ease.OutBack);
		}

		public void HideHighlight()
		{
			transform.DOKill();
			transform.DOLocalMoveZ(0, HIGHLIGHT_DURATION).SetEase(Ease.InBack);
			transform.DOScale(1, HIGHLIGHT_DURATION).SetEase(Ease.InBack).OnKill(() => { transform.localScale = Vector3.one; });
		}

		#region Layer Blocker

		public void RegisterBlocker(Tile downTile)
		{
			OnTileRemoved += downTile.OnBlockerRemoved;

			if (!Application.isPlaying)
			{
				downTile.AddBlockerCount();
			}
		}

		public void ResetBlockers()
		{
			LayerBlockCount = 0;
		}

		private void OnBlockerRemoved(Tile blockerTile)
		{
			blockerTile.OnTileRemoved -= OnBlockerRemoved;

			LayerBlockCount--;
			CheckBlockState();
		}

		private void CheckBlockState()
		{
			if (LayerBlockCount > 0)
			{
				layerBlocker.SetActive(true);
				SetInteractable(false);
			}
			else
			{
				layerBlocker.SetActive(false);
				SetInteractable(true);

				if (Obstacle && Obstacle is ZipperObstacle zipper)
					zipper.SetBlocker(false);
			}
		}

		public void AddBlockerCount()
		{
			LayerBlockCount++;

			CheckBlockState();
		}

		#endregion

		#region Inputs

		public void OnPointerDown(PointerEventData eventData)
		{
			if (IsInDeck) return;

			CurrentTile = this;

			if (!Obstacle)
			{
				Highlight();

				HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.RigidImpact);
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (IsInDeck) return;

			if (!CurrentTile) return;
			if (eventData.pointerEnter && !eventData.pointerEnter.Equals(CurrentTile.gameObject))
			{
				HideHighlight();
			}
			else if (eventData.pointerEnter)
			{
				if (IsInDeck)
				{
					CurrentTile = null;
					return;
				}

				transform.DOKill();
				if (Obstacle && Obstacle.IsBlockingMovement)
				{
					Obstacle.OnTapped();
					CurrentTile = null;

					return;
				}

				MoveTileToHolder();
			}
			else
			{
				HideHighlight();
			}

			CurrentTile = null;
		}

		public void SetInteractable(bool interactable)
		{
			col.enabled = interactable;
		}

		#endregion

		public void MoveTileToHolder()
		{
			trail.emitting = true;

			AudioManager.Instance.PlayAudio(AudioName.Pop1);

			IsInDeck = true;
			SetInteractable(false);

			OnTappedToTile?.Invoke(this);
			OnTileRemoved?.Invoke(this);
		}

		public void TilePlaced()
		{
			trail.emitting = false;
			OnTilePlaced?.Invoke(this);
		}

#if UNITY_EDITOR
		public void SetupObstacle(BaseObstacle obstacle)
		{
			Obstacle = (BaseObstacle)PrefabUtility.InstantiatePrefab(obstacle, obstacleHolder);
			Obstacle.Setup(this);
		}
#endif
	}
}