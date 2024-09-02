using DG.Tweening;
using Fiber.Utilities;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fiber.UI
{
	public class LoadingPanelController : SingletonPersistent<LoadingPanelController>
	{
		[Title("General Variables")]
		[SerializeField] private float minLoadingDuration = 4f;
		[SerializeField] private float maxLoadingDuration = 5f;
		[SerializeField] private Ease loadingEase;

		[Title("References")]
		[SerializeField] private Image imgFillBar;
		[SerializeField] private GameObject loadingPanelParent;
		[Space]
		[SerializeField] private Image imgBackground;
		[SerializeField] private Image imgLoadingScreen;
		[SerializeField] private Image imgLoadingScreenTitle;

		public event UnityAction OnLoadingFinished;

		private void Start()
		{
			imgFillBar.fillAmount = 0f;
			loadingPanelParent.SetActive(true);

			float _duration = Random.Range(minLoadingDuration, maxLoadingDuration);

			imgFillBar.DOFillAmount(1f, _duration).SetEase(loadingEase).SetLink(gameObject).SetTarget(gameObject).OnComplete(() =>
			{
				loadingPanelParent.SetActive(false);
				OnLoadingFinished?.Invoke();
			});
		}

		public void SetLoadingScreen(Sprite background, Sprite loadingScreen, Sprite loadingScreenTitle)
		{
			imgBackground.sprite = background;
			imgLoadingScreen.sprite = loadingScreen;
			imgLoadingScreenTitle.sprite = loadingScreenTitle;
		}
	}
}