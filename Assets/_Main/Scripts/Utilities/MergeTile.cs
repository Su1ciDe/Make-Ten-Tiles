using DG.Tweening;
using Fiber.AudioSystem;
using Fiber.Managers;
using Fiber.Utilities;
using GridSystem;
using HolderSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

namespace Utilities
{
	public class MergeTile : MonoBehaviour
	{
		[SerializeField] private TextMeshPro txtTen;
		[SerializeField] private Transform tileHolder;
		[SerializeField] private Renderer tileRenderer;
		[Space]
		[SerializeField] private PositionConstraint positionConstraint;

		private Vector3 textPosition;

		private const string POOL_TAG = "MergeTile";
		private const float ANIMATION_DURATION = .5f;

		private void Awake()
		{
			txtTen.SetText(GameManager.BLAST_COUNT.ToString());
			textPosition = txtTen.transform.localPosition;
		}

		private void OnDisable()
		{
			ResetTile();
		}

		private void OnDestroy()
		{
			tileHolder.DOKill();
			txtTen.DOKill();
			txtTen.transform.DOKill();
		}

		public void Blast(TileType tileType, HolderGroup holderGroup)
		{
			AddConstraint(holderGroup.transform);

			var mats = tileRenderer.materials;
			for (var i = 0; i < mats.Length; i++)
				mats[i] = GameManager.Instance.ColorsSO.Materials[tileType];
			tileRenderer.materials = mats;

			txtTen.transform.localScale = Vector3.zero;
			txtTen.transform.DOScale(1, .5f).SetEase(Ease.OutBack);
			tileHolder.DOScale(0, .25f).SetDelay(ANIMATION_DURATION).SetEase(Ease.InCirc).OnComplete(() =>
			{
				AudioManager.Instance.PlayAudio(AudioName.Plop1);

				tileHolder.gameObject.SetActive(false);
				txtTen.color = mats[0].color;
				txtTen.DOFade(0, 1).SetEase(Ease.OutCubic);
				txtTen.transform.DOMoveY(3, 1).SetRelative(true).SetEase(Ease.OutCubic).OnComplete(() => ObjectPooler.Instance.Release(gameObject, POOL_TAG));
			});
		}

		private void ResetTile()
		{
			txtTen.color = Color.white;
			txtTen.transform.localPosition = textPosition;
			txtTen.transform.localScale = Vector3.one;

			tileHolder.gameObject.SetActive(true);
			tileHolder.localScale = Vector3.one;

			positionConstraint.RemoveSource(0);
		}

		private void AddConstraint(Transform t)
		{
			var constraint = new ConstraintSource { weight = 1, sourceTransform = t };
			positionConstraint.AddSource(constraint);
			positionConstraint.constraintActive = true;
		}
	}
}