using System.Collections;
using Fiber.UI;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay.Player;
using GridSystem;
using GridSystem.Tiles;
using HolderSystem;
using Obstacles;
using UnityEngine;

namespace Managers
{
	public class TutorialManager : MonoBehaviour
	{
		private TutorialUI tutorialUI => TutorialUI.Instance;

		[SerializeField] private int iceObstacleTutorialLevel;
		[SerializeField] private int cageObstacleTutorialLevel;
		[SerializeField] private int zipperObstacleTutorialLevel;

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
				tutorialUI.HideTapToSkip();
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

			if (LevelManager.Instance.LevelNo.Equals(2))
			{
				Level2Tutorial();
			}

			if (LevelManager.Instance.LevelNo.Equals(zipperObstacleTutorialLevel))
			{
				StartCoroutine(ZipperObstacleTutorial());
			}

			if (LevelManager.Instance.LevelNo.Equals(iceObstacleTutorialLevel))
			{
				StartCoroutine(IceObstacleTutorial());
			}
		}

		#region Level 1

		private IEnumerator Level1Tutorial()
		{
			Player.Instance.CanInput = false;

			yield return new WaitForSeconds(0.5f);

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

		#region Level 2

		private int level2TileCount;

		private void Level2Tutorial()
		{
			level2TileCount = 2;
			Player.Instance.CanInput = false;

			Tile.OnTilePlaced += OnTilePlacedLevel2;
		}

		private void OnTilePlacedLevel2(Tile tile)
		{
			level2TileCount--;

			if (level2TileCount > 0) return;

			Tile.OnTilePlaced -= OnTilePlacedLevel2;
			Player.Instance.CanInput = false;

			tutorialUI.ShowText("Don't let the holder get full!", new Vector3(0, -1600));
			tutorialUI.ShowFocus(Holder.Instance.transform.position, Helper.MainCamera, false, 0, 2);
			tutorialUI.ShowTapToSkip(Level2TutorialComplete, true, 1);
		}

		private void Level2TutorialComplete()
		{
			tutorialUI.HideText();
			tutorialUI.HideFocus();

			Player.Instance.CanInput = true;
		}

		#endregion

		#region IceTutorial

		private IEnumerator IceObstacleTutorial()
		{
			Player.Instance.CanInput = false;

			yield return new WaitForSeconds(0.5f);

			var iceObstacle = GridManager.Instance.FindObstacle<IceObstacle>();
			var pos = iceObstacle.AttachedTile.transform.position;

			tutorialUI.ShowFocus(pos, Helper.MainCamera);
			tutorialUI.ShowText("Break the ICE by moving any 2 tiles!", new Vector3(0, -1150));
			tutorialUI.ShowTapToSkip(IceObstacleTutorialComplete, true, 1);
		}

		private void IceObstacleTutorialComplete()
		{
			tutorialUI.HideFocus();
			tutorialUI.HideText();

			Player.Instance.CanInput = true;
		}

		#endregion

		#region ZipperTutorial

		private IEnumerator ZipperObstacleTutorial()
		{
			Player.Instance.CanInput = false;

			yield return new WaitForSeconds(0.5f);

			var zipperObstacle = GridManager.Instance.FindObstacle<ZipperObstacle>();
			var pos = zipperObstacle.AttachedTile.transform.position;

			tutorialUI.ShowFocus(pos, Helper.MainCamera, false, 0, 1.2f);
			tutorialUI.ShowText("Tiles attached with ZIPPER move together!", new Vector3(0, -1100));
			tutorialUI.ShowTap(pos, Helper.MainCamera);
			tutorialUI.SetupFakeButton(() => zipperObstacle.OnTapped(), pos, Helper.MainCamera);

			Tile.OnTappedToTile += OnZipperTileTapped;
		}

		private void OnZipperTileTapped(Tile tile)
		{
			Tile.OnTappedToTile -= OnZipperTileTapped;

			tutorialUI.HideFocus();
			tutorialUI.HideText();
			tutorialUI.HideHand();
			tutorialUI.HideFakeButton();

			Player.Instance.CanInput = true;
		}

		#endregion
	}
}