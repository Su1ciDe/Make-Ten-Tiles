using DG.Tweening;
using Fiber.Managers;
using TMPro;
using TriInspector;
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
		[field: SerializeField, ReadOnly, Group("Properties")] public CellType Type { get; private set; }
		[field: SerializeField, ReadOnly, Group("Properties")] public GridCell CurrentCell { get; private set; }
		[field: SerializeField, ReadOnly, Group("Properties")] public int LayerBlockCount { get; private set; }

		[Title("References")]
		[SerializeField] private GameObject blockImage;
		[SerializeField] private new Renderer renderer;
		[SerializeField] private Collider col;
		[SerializeField] private TextMeshPro txtAmount;

		public static float JUMP_DURATION = .35F;
		public static float BLAST_DURATION = .35F;
		private const float HIGHLIGHT_DURATION = .15F;
		private const float JUMP_POWER = 5;

		public static event UnityAction<Tile> OnTappedToTile;
		public event UnityAction<Tile> OnTileRemoved;

		public void Setup(CellType cellType, GridCell cell)
		{
			Type = cellType;
			txtAmount.SetText(((int)cellType).ToString());
			CurrentCell = cell;
			CurrentCell.CurrentTile = this;

			renderer.material = GameManager.Instance.ColorsSO.Materials[Type];
		}

		public Tween Jump(Vector3 pos)
		{
			transform.localScale = Vector3.one;
			return transform.DOLocalJump(pos, transform.position.y / 2f + JUMP_POWER, 1, JUMP_DURATION);
		}

		public void Blast()
		{
			transform.DOScale(1.3f, BLAST_DURATION).SetEase(Ease.OutSine);
			// tileRenderer.DOFade(0, BLAST_DURATION).SetEase(Ease.InSine);
			// tileFaceRenderer.DOFade(0, BLAST_DURATION).SetEase(Ease.InSine);
			transform.DOShakeRotation(BLAST_DURATION, 4 * Vector3.forward, 25, 2, false, ShakeRandomnessMode.Harmonic).SetEase(Ease.InQuart).OnComplete(() => Destroy(gameObject));
		}

		private void Highlight()
		{
			transform.DOKill();
			transform.DOLocalMoveZ(0.1f, HIGHLIGHT_DURATION).SetRelative().SetEase(Ease.OutBack);
			transform.DOScale(1.1f, HIGHLIGHT_DURATION).SetEase(Ease.OutBack);
		}

		private void HideHighlight()
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

		private void OnBlockerRemoved(Tile blockerTile)
		{
			blockerTile.OnTileRemoved -= OnBlockerRemoved;

			LayerBlockCount--;
			CheckBlockState();
		}

		public void CheckBlockState()
		{
			if (LayerBlockCount > 0)
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
			LayerBlockCount++;

			CheckBlockState();
		}

		#endregion

		#region Inputs

		public void OnPointerDown(PointerEventData eventData)
		{
			CurrentTile = this;

			Highlight();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (!CurrentTile) return;
			if (eventData.pointerEnter && !eventData.pointerEnter.Equals(CurrentTile.gameObject))
			{
				HideHighlight();
			}
			else if (eventData.pointerEnter)
			{
				transform.DOKill();

				if (IsInDeck) return;
				IsInDeck = true;
				col.enabled = false;

				OnTappedToTile?.Invoke(this);
				OnTileRemoved?.Invoke(this);

				CurrentTile = null;
			}
			else
			{
				HideHighlight();
			}
		}

		#endregion
	}
}