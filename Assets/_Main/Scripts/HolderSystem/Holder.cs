using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fiber.Managers;
using Fiber.Utilities;
using GridSystem;
using GridSystem.Tiles;
using UnityEngine;

namespace HolderSystem
{
	public class Holder : Singleton<Holder>
	{
		[SerializeField] private int slotCount = 7;
		[SerializeField] private int unlockLevel = 0;

		private readonly List<HolderGroup> holderGroups = new List<HolderGroup>();
		private readonly List<HolderSlot> holderSlots = new List<HolderSlot>();
		private readonly Queue<HolderGroup> holderGroupPool = new Queue<HolderGroup>();

		private readonly WaitForSeconds waitBlast = new WaitForSeconds(Tile.BLAST_DURATION);

		private CancellationTokenSource token = new CancellationTokenSource();

		private void Awake()
		{
			Setup();
		}

		private void Start()
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, -GridManager.Instance.GridCells.GetLength(0) * Tile.TILE_HEIGHT);
		}

		private void OnEnable()
		{
			Tile.OnTappedToTile += AddTileToDeck;
		}

		private void OnDisable()
		{
			Tile.OnTappedToTile -= AddTileToDeck;
			token.Dispose();
		}

		private void Setup()
		{
			var holderSlotPrefab = GameManager.Instance.PrefabsSO.HolderSlotPrefab;
			var holderGroupPrefab = GameManager.Instance.PrefabsSO.HolderGroupPrefab;
			var offset = slotCount * holderSlotPrefab.Size / 2f - holderSlotPrefab.Size / 2f;
			for (int i = 0; i < slotCount; i++)
			{
				var slot = Instantiate(holderSlotPrefab, transform);
				slot.transform.localPosition = new Vector3(i * holderSlotPrefab.Size - offset, 0, 0);
				holderSlots.Add(slot);

				if (i == slotCount - 1 && LevelManager.Instance.LevelNo < unlockLevel)
				{
					slot.Lock(unlockLevel);
					continue;
				}

				var deckGroup = Instantiate(holderGroupPrefab, transform);
				deckGroup.gameObject.SetActive(false);
				holderGroupPool.Enqueue(deckGroup);
			}
		}

		public async void AddTileToDeck(Tile tile)
		{
			var tenGroup = FindTen(tile);
			if (tenGroup)
			{
				TenBlast(tile, tenGroup);
			}
			else
			{
				if (!holderGroupPool.TryDequeue(out var newGroup)) return;

				token.Cancel();

				newGroup.gameObject.SetActive(true);
				var tileCount = GetTotalTileCount();
				newGroup.Setup(holderSlots[tileCount]);
				newGroup.AddTile(tile);
				holderGroups.Add(newGroup);

				//TODO: wait for blast
				// Check if the holder is full and lose the game
				try
				{
					await UniTask.WaitForSeconds(0.5f, cancellationToken: token.Token);
				}
				catch (OperationCanceledException e)
				{
				}

				var lockedCount = LevelManager.Instance.LevelNo < unlockLevel ? 1 : 0;
				if (GetTotalTileCount().Equals(slotCount - lockedCount))
				{
					LevelManager.Instance.Lose();
				}
			}
		}

		private void TenBlast(Tile tile, HolderGroup tenGroup)
		{
			var tileInDeck = tenGroup.Tiles[0];
			tileInDeck.IsCompleted = true;
			tile.IsCompleted = true;
			tile.transform.SetParent(tenGroup.transform);
			tile.Jump(1 * Vector3.up).OnComplete(() =>
			{
				tileInDeck.Blast();
				tile.Blast().OnComplete(() =>
				{
					holderGroups.Remove(tenGroup);
					holderGroupPool.Enqueue(tenGroup);
					tenGroup.gameObject.SetActive(false);
					RearrangeGroups();
				});
			});
		}

		public void RearrangeGroups()
		{
			int count = 0;
			for (int i = 0; i < holderGroups.Count; i++)
			{
				if (i > 0)
				{
					var previousGroup = holderGroups[i - 1];
					count += previousGroup.Tiles.Count;
				}

				holderGroups[i].MoveToSlot(holderSlots[count]);
			}
		}

		// private void OnBlast(HolderGroup holderGroup)
		// {
		// 	StartCoroutine(WaitBlast());
		// 	return;
		//
		// 	IEnumerator WaitBlast()
		// 	{
		// 		yield return waitBlast;
		//
		// 		holderGroups.Remove(holderGroup);
		// 		holderGroupPool.Enqueue(holderGroup);
		// 		holderGroup.gameObject.SetActive(false);
		// 		RearrangeGroups();
		// 	}
		// }

		public HolderGroup FindTen(Tile tile)
		{
			for (int i = 0; i < holderGroups.Count; i++)
			{
				for (var j = 0; j < holderGroups[i].Tiles.Count; j++)
				{
					if ((int)holderGroups[i].Tiles[j].Type + (int)tile.Type == GameManager.BLAST_COUNT && !holderGroups[i].Tiles[j].IsCompleted)
						return holderGroups[i];
				}
			}

			return null;
		}

		public int GetTotalTileCount()
		{
			int count = 0;
			for (int i = 0; i < holderGroups.Count; i++)
				count += holderGroups[i].Tiles.Count;
			return count;
		}
	}
}