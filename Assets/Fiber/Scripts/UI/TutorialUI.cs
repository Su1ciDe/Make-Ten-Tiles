using TMPro;
using DG.Tweening;
using Fiber.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fiber.UI
{
	public class TutorialUI : Singleton<TutorialUI>
	{
		[SerializeField] private Image hand;
		[Space]
		[SerializeField] private Button fakeButton;
		[Space]
		[SerializeField] private RectTransform message;
		[SerializeField] private TMP_Text messageText;
		[Space]
		[SerializeField] private Image focus;
		[SerializeField] private Image blocker;
		[Space]
		[SerializeField] private GameObject tapToSkipPanel;
		[SerializeField] private TMP_Text txtTapToSkip;
		[SerializeField] private Button btnTapToSkip;

		private Vector3 messagePosition;

		private const float HAND_MOVE_TIME = .7f;
		private const float HAND_TAP_TIME = .25f;

		private const float FOCUSING_TIME = .5f;

		public event UnityAction OnFakeButtonClicked;

		private void Awake()
		{
			messagePosition = message.transform.position;
			btnTapToSkip.onClick.AddListener(TapToSkip);
		}

		private void OnDestroy()
		{
			hand.DOKill();
			message.transform.DOKill();
			focus.DOKill();
		}

		public void ShowSwipe(Vector3 from, Vector3 to)
		{
			var seq = DOTween.Sequence();
			seq.AppendCallback(() =>
			{
				hand.rectTransform.position = from;
				hand.gameObject.SetActive(true);
			});
			seq.AppendInterval(.5f);
			seq.Append(hand.rectTransform.DOScale(.75f, HAND_TAP_TIME).SetEase(Ease.OutExpo));
			seq.Append(hand.rectTransform.DOMove(to, HAND_MOVE_TIME).SetEase(Ease.InSine));
			seq.AppendInterval(.5f);
			seq.Append(hand.rectTransform.DOScale(1, HAND_TAP_TIME).SetEase(Ease.OutExpo));
			seq.AppendInterval(.5f);
			seq.AppendCallback(() => hand.gameObject.SetActive(false));
			seq.AppendInterval(.5f);
			seq.SetUpdate(true);
			seq.SetTarget(hand);
			seq.SetLoops(-1, LoopType.Restart);
			seq.OnKill(() =>
			{
				hand.rectTransform.localScale = Vector3.one;
				hand.gameObject.SetActive(false);
			});
		}

		public void ShowSwipe(Vector3 from, Vector3 to, Camera cam)
		{
			var _from = cam.WorldToScreenPoint(from);
			var _to = cam.WorldToScreenPoint(to);

			ShowSwipe(_from, _to);
		}

		public void ShowSwipe(Vector3 from, Vector3 to, Camera fromCamera, Camera toCamera)
		{
			var _from = fromCamera.WorldToScreenPoint(from);
			var _to = toCamera.WorldToScreenPoint(to);

			ShowSwipe(_from, _to);
		}

		public void ShowTap(Vector3 position, Camera cam = null)
		{
			var pos = position;
			if (cam) pos = cam.WorldToScreenPoint(position);

			var seq = DOTween.Sequence();
			seq.AppendCallback(() =>
			{
				hand.rectTransform.position = pos;
				hand.gameObject.SetActive(true);
			});
			seq.AppendInterval(.5f);
			seq.Append(hand.rectTransform.DOScale(.75f, HAND_TAP_TIME).SetEase(Ease.InOutExpo));
			seq.Append(hand.rectTransform.DOScale(1, HAND_TAP_TIME).SetEase(Ease.InOutExpo));
			seq.AppendInterval(.5f);
			seq.SetUpdate(true);
			seq.SetTarget(hand);
			seq.SetLoops(-1, LoopType.Restart);
			seq.OnKill(() =>
			{
				hand.rectTransform.localScale = Vector3.one;
				hand.gameObject.SetActive(false);
			});
		}

		public void HideHand()
		{
			hand.DOKill();
		}

		public void ShowText(string msg, float showDuration = 0, bool isAnimated = false)
		{
			message.transform.DOComplete();
			messageText.SetText(msg);
			message.gameObject.SetActive(true);
			if (isAnimated)
			{
				message.transform.DOScale(1.25f, .25f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo).OnKill(() => message.transform.localScale = Vector3.one).SetTarget(messageText)
					.SetUpdate(true);
			}

			if (!showDuration.Equals(0))
				DOVirtual.DelayedCall(showDuration, HideText).SetTarget(messageText).SetUpdate(true);
		}

		public void ShowText(string msg, Vector3 position, Camera cam = null, float showDuration = 0, bool isAnimated = false)
		{
			var pos = position;
			if (cam)
			{
				pos = cam.WorldToScreenPoint(position);
				message.transform.position = pos;
			}
			else
			{
				message.anchoredPosition = pos;
			}

			ShowText(msg, showDuration, isAnimated);
		}

		public void HideText()
		{
			message.transform.DOKill();
			message.gameObject.SetActive(false);

			message.transform.position = messagePosition;
		}

		public bool IsShowingText => message.gameObject.activeSelf;

		public void ShowFocus(Vector3 position, Camera cam = null, bool repeated = false, float delay = 0, float scale = 1)
		{
			if (cam) position = cam.WorldToScreenPoint(position);
			focus.transform.localScale = scale * Vector3.one;
			focus.gameObject.SetActive(true);
			var seq = DOTween.Sequence();
			seq.AppendInterval(delay);
			seq.AppendCallback(() => focus.transform.position = position);
			seq.Append(focus.DOFade(1, FOCUSING_TIME));
			seq.Join(focus.transform.DOScale(scale + 1, FOCUSING_TIME).From().SetEase(Ease.InCubic));
			seq.AppendInterval(1.5f);
			seq.Append(focus.DOFade(0, FOCUSING_TIME));
			seq.Join(focus.transform.DOScale(scale + 1, FOCUSING_TIME).SetEase(Ease.OutCubic));
			seq.SetTarget(focus);
			seq.SetUpdate(true);
			seq.OnKill(() =>
			{
				var tempColor = focus.color;
				tempColor.a = 0;
				focus.color = tempColor;
				focus.transform.localScale = Vector3.one;
				focus.gameObject.SetActive(false);
			});
			if (repeated)
				seq.SetLoops(-1, LoopType.Restart);
			seq.OnComplete(() => focus.transform.localScale = Vector3.one);
		}

		public void HideFocus()
		{
			focus.DOKill();
			focus.transform.localScale = Vector3.one;
			focus.gameObject.SetActive(false);
		}

		public void SetupFakeButton(UnityAction action, Vector3 position, Camera cam = null)
		{
			SetBlocker(true);
			if (cam) position = cam.WorldToScreenPoint(position);

			fakeButton.transform.position = position;
			fakeButton.gameObject.SetActive(true);
			fakeButton.onClick.AddListener(() =>
			{
				HideFakeButton();
				OnFakeButtonClicked?.Invoke();
			});
			fakeButton.onClick.AddListener(action);
		}

		public void HideFakeButton()
		{
			fakeButton.onClick.RemoveAllListeners();
			fakeButton.gameObject.SetActive(false);
		}

		public void SetBlocker(bool block)
		{
			blocker.gameObject.SetActive(block);
		}

		public void ShowTapToSkip(UnityAction action, bool showText = true, float delay = 0)
		{
			tapToSkipPanel.SetActive(true);
			btnTapToSkip.onClick.AddListener(action);
			btnTapToSkip.gameObject.SetActive(false);
			txtTapToSkip.gameObject.SetActive(false);
			DOVirtual.DelayedCall(delay, () =>
			{
				btnTapToSkip.gameObject.SetActive(true);
				txtTapToSkip.gameObject.SetActive(showText);
			}).SetUpdate(true).SetTarget(tapToSkipPanel);
		}

		public void HideTapToSkip()
		{
			tapToSkipPanel.SetActive(false);
			btnTapToSkip.gameObject.SetActive(false);
			txtTapToSkip.gameObject.SetActive(false);
		}

		private void TapToSkip()
		{
			DOTween.Kill(tapToSkipPanel);

			HideHand();
			btnTapToSkip.onClick.RemoveAllListeners();
			btnTapToSkip.onClick.AddListener(TapToSkip);

			txtTapToSkip.transform.DOKill();
			tapToSkipPanel.SetActive(false);
			txtTapToSkip.gameObject.SetActive(false);
		}
	}
}