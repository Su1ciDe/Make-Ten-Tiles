using System.Collections;
using Fiber.UI;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay.Player;
using GridSystem;
using GridSystem.Tiles;
using UnityEngine;

namespace Managers
{
	public class TutorialManager : MonoBehaviour
	{
		private TutorialUI tutorialUI => TutorialUI.Instance;

		private void OnEnable()
		{
			LevelManager.OnLevelStart += OnLevelStarted;
			LevelManager.OnLevelUnload += OnLevelUnloaded;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelStart -= OnLevelStarted;
			LevelManager.OnLevelUnload -= OnLevelUnloaded;
		}

		private void OnDestroy()
		{
			Unsub();
		}

		private void OnLevelUnloaded()
		{
			Unsub();
		}

		private void OnLevelStarted()
		{
			if (LoadingPanelController.Instance && LoadingPanelController.Instance.IsActive)
			{
				StartCoroutine(WaitLoadingScreen());
			}
			else
			{
				LevelStart();
			}
		}

		private void Unsub()
		{
			StopAllCoroutines();

			if (TutorialUI.Instance)
			{
				tutorialUI.HideFocus();
				tutorialUI.HideHand();
				tutorialUI.HideText();
				tutorialUI.HideFakeButton();
			}

			Tile.OnTilePlaced -= OnTilePlacedLevel1_1;
			Tile.OnTilePlaced -= OnTilePlacedLevel1_2;
		}

		private IEnumerator WaitLoadingScreen()
		{
			yield return new WaitUntilAction(ref LoadingPanelController.Instance.OnLoadingFinished);

			LevelStart();
		}

		private void LevelStart()
		{
			if (LevelManager.Instance.LevelNo.Equals(1))
			{
				StartCoroutine(Level1Tutorial());
			}
		}

		#region Level 1

		private IEnumerator Level1Tutorial()
		{
			Player.Instance.CanInput = false;

			yield return new WaitForSeconds(1);

			var selectedCell = GridManager.Instance.GridCells[0, 0, 0];
			var pos = selectedCell.transform.position;

			tutorialUI.ShowText("Tap two tiles that add up to 10 to clear!");
			tutorialUI.ShowFocus(pos, Helper.MainCamera);
			tutorialUI.ShowTap(pos, Helper.MainCamera);
			tutorialUI.SetupFakeButton(() => selectedCell.CurrentTile.MoveTileToHolder(), pos, Helper.MainCamera);

			Tile.OnTilePlaced += OnTilePlacedLevel1_1;
		}

		private void OnTilePlacedLevel1_1(Tile tile)
		{
			Tile.OnTilePlaced -= OnTilePlacedLevel1_1;

			tutorialUI.HideFocus();
			tutorialUI.HideHand();
			tutorialUI.HideFakeButton();

			var selectedCell = GridManager.Instance.GridCells[0, 2, 0];
			var pos = selectedCell.transform.position;
			tutorialUI.ShowTap(pos, Helper.MainCamera);
			tutorialUI.SetupFakeButton(() => selectedCell.CurrentTile.MoveTileToHolder(), pos, Helper.MainCamera);

			Tile.OnTilePlaced += OnTilePlacedLevel1_2;
		}

		private void OnTilePlacedLevel1_2(Tile tile)
		{
			Tile.OnTilePlaced -= OnTilePlacedLevel1_2;

			tutorialUI.HideText();
			tutorialUI.HideFocus();
			tutorialUI.HideHand();
			tutorialUI.HideFakeButton();

			Player.Instance.CanInput = true;
		}

		#endregion
	}
}