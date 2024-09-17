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
		[SerializeField] private int zipperObstacleTutorialLevel;
		[SerializeField] private int cageObstacleTutorialLevel;

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
			Tile.OnTilePlaced -= OnTilePlacedLevel1_3;
			Tile.OnTilePlaced -= OnTilePlacedLevel1_4;
			Tile.OnTappedToTile -= OnTileTappedLevel3;
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
				StartCoroutine(Level2Tutorial());
			}

			if (LevelManager.Instance.LevelNo.Equals(3))
			{
				Level3Tutorial();
			}

			if (LevelManager.Instance.LevelNo.Equals(zipperObstacleTutorialLevel))
			{
				StartCoroutine(ZipperObstacleTutorial());
			}

			if (LevelManager.Instance.LevelNo.Equals(iceObstacleTutorialLevel))
			{
				StartCoroutine(IceObstacleTutorial());
			}

			if (LevelManager.Instance.LevelNo.Equals(cageObstacleTutorialLevel))
			{
				StartCoroutine(CageTutorial());
			}
		}

		#region Level 1

		private IEnumerator Level1Tutorial()
		{
			Player.Instance.CanInput = false;
			tutorialUI.SetBlocker(true);

			yield return new WaitForSeconds(0.5f);
			Player.Instance.CanInput = false;

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

			var selectedCell = GridManager.Instance.GridCells[0, 0, 2];
			var pos = selectedCell.transform.position;
			tutorialUI.ShowTap(pos, Helper.MainCamera);
			tutorialUI.SetupFakeButton(() => selectedCell.CurrentTile.MoveTileToHolder(), pos, Helper.MainCamera);

			Tile.OnTilePlaced += OnTilePlacedLevel1_3;
		}

		private void OnTilePlacedLevel1_3(Tile tile)
		{
			Tile.OnTilePlaced -= OnTilePlacedLevel1_3;

			tutorialUI.HideText();
			tutorialUI.HideFocus();
			tutorialUI.HideHand();
			tutorialUI.HideFakeButton();

			var selectedCell = GridManager.Instance.GridCells[0, 2, 2];
			var pos = selectedCell.transform.position;
			tutorialUI.ShowTap(pos, Helper.MainCamera);
			tutorialUI.SetupFakeButton(() => selectedCell.CurrentTile.MoveTileToHolder(), pos, Helper.MainCamera);

			Tile.OnTilePlaced += OnTilePlacedLevel1_4;
		}

		private void OnTilePlacedLevel1_4(Tile tile)
		{
			Tile.OnTilePlaced -= OnTilePlacedLevel1_4;

			tutorialUI.HideText();
			tutorialUI.HideFocus();
			tutorialUI.HideHand();
			tutorialUI.HideFakeButton();
			tutorialUI.SetBlocker(false);

			Player.Instance.CanInput = true;
		}

		#endregion

		#region Level 2

		private IEnumerator Level2Tutorial()
		{
			tutorialUI.SetBlocker(true);
			Player.Instance.CanInput = false;

			yield return new WaitForSeconds(0.5f);

			var selectedCell = GridManager.Instance.GridCells[0, 0, 0];
			var pos = selectedCell.transform.position;
			tutorialUI.ShowTap(pos, Helper.MainCamera);
			tutorialUI.SetupFakeButton(() => selectedCell.CurrentTile.MoveTileToHolder(), pos, Helper.MainCamera);

			Tile.OnTilePlaced += OnTilePlacedLevel2;
		}

		private void OnTilePlacedLevel2(Tile tile)
		{
			Tile.OnTilePlaced -= OnTilePlacedLevel2;
			
			tutorialUI.HideHand();

			tutorialUI.ShowText("Don't let the holder get full!", new Vector3(0, -1600));
			tutorialUI.ShowFocus(Holder.Instance.transform.position, Helper.MainCamera, false, 0, 2);
			tutorialUI.ShowTapToSkip(Level2TutorialComplete, true, 1);
		}

		private void Level2TutorialComplete()
		{
			tutorialUI.HideText();
			tutorialUI.HideFocus();

			tutorialUI.SetBlocker(false);
			Player.Instance.CanInput = true;
		}

		#endregion

		#region Level 3

		private void Level3Tutorial()
		{
			Tile.OnTappedToTile += OnTileTappedLevel3;
		}

		private void OnTileTappedLevel3(Tile tile)
		{
			Tile.OnTappedToTile -= OnTileTappedLevel3;

			Player.Instance.CanInput = false;

			tutorialUI.ShowFocus(tile.transform.position, Helper.MainCamera);
			tutorialUI.ShowText("To collect the tiles at the bottom, you need to clear the tiles above them first.", new Vector3(0, -1600));
			tutorialUI.ShowTapToSkip(Level3TutorialComplete, true, 1);
		}

		private void Level3TutorialComplete()
		{
			tutorialUI.HideText();
			tutorialUI.HideFocus();

			Player.Instance.CanInput = true;
		}

		#endregion

		#region IceTutorial

		private IEnumerator IceObstacleTutorial()
		{
			tutorialUI.SetBlocker(true);

			Player.Instance.CanInput = false;

			yield return new WaitForSeconds(0.5f);

			var iceObstacle = GridManager.Instance.FindObstacle<IceObstacle>();
			var pos = iceObstacle.AttachedTile.transform.position;

			tutorialUI.ShowFocus(pos, Helper.MainCamera);
			tutorialUI.ShowText("Break the ICE by moving any 2 tiles!", new Vector3(0, -700));
			tutorialUI.ShowTapToSkip(IceObstacleTutorialComplete, true, 1);
		}

		private void IceObstacleTutorialComplete()
		{
			tutorialUI.HideFocus();
			tutorialUI.HideText();

			Player.Instance.CanInput = true;
			tutorialUI.SetBlocker(false);
		}

		#endregion

		#region ZipperTutorial

		private IEnumerator ZipperObstacleTutorial()
		{
			tutorialUI.SetBlocker(true);

			Player.Instance.CanInput = false;

			yield return new WaitForSeconds(0.5f);

			var zipperObstacle = GridManager.Instance.FindObstacle<ZipperObstacle>();
			var pos = zipperObstacle.AttachedTile.transform.position;

			tutorialUI.ShowFocus(pos, Helper.MainCamera, false, 0, 1.2f);
			tutorialUI.ShowText("Tiles attached with ZIPPER move together!", new Vector3(0, -1700));
			tutorialUI.ShowTap(pos, Helper.MainCamera);
			tutorialUI.SetupFakeButton(() => zipperObstacle.OnTapped(), pos, Helper.MainCamera);

			Tile.OnTilePlaced += OnZipperTileTapped;
		}

		private void OnZipperTileTapped(Tile tile)
		{
			Tile.OnTilePlaced -= OnZipperTileTapped;

			tutorialUI.HideFocus();
			tutorialUI.HideText();
			tutorialUI.HideHand();
			tutorialUI.HideFakeButton();

			Player.Instance.CanInput = true;
			tutorialUI.SetBlocker(false);
		}

		#endregion

		#region CageTutorial

		private IEnumerator CageTutorial()
		{
			tutorialUI.SetBlocker(true);
			yield return new WaitForSeconds(0.5f);

			Player.Instance.CanInput = false;

			var cageObstacle = GridManager.Instance.FindObstacle<CageObstacle>();
			var keyObstacle = GridManager.Instance.FindObstacle<KeyObstacle>();

			tutorialUI.ShowFocus(cageObstacle.transform.position, Helper.MainCamera);
			tutorialUI.ShowText("Break the CAGE by collecting the KEY!", new Vector3(0, -2000));

			yield return new WaitForSeconds(2);

			tutorialUI.HideFocus();
			tutorialUI.ShowFocus(keyObstacle.transform.position, Helper.MainCamera);
			tutorialUI.ShowTap(keyObstacle.transform.position, Helper.MainCamera);
			tutorialUI.SetupFakeButton(() =>
			{
				keyObstacle.AttachedTile.MoveTileToHolder();
				OnKeyObstacleTapped();
			}, keyObstacle.transform.position, Helper.MainCamera);
		}

		private void OnKeyObstacleTapped()
		{
			tutorialUI.HideFocus();
			tutorialUI.HideText();
			tutorialUI.HideHand();
			tutorialUI.HideFakeButton();

			Player.Instance.CanInput = true;
			tutorialUI.SetBlocker(false);
		}

		#endregion
	}
}