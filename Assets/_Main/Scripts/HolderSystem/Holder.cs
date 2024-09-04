using System.Collections;
using System.Collections.Generic;
using Fiber.Managers;
using Fiber.Utilities;
using GridSystem.Tiles;
using UnityEngine;

namespace HolderSystem
{
	public class Holder : Singleton<Holder>
	{
		[SerializeField] private int slotCount = 7;

		private List<HolderGroup> holderGroups = new List<HolderGroup>();
		private List<HolderSlot> holderSlots = new List<HolderSlot>();
		private Queue<HolderGroup> holderGroupPool = new Queue<HolderGroup>();

		private readonly WaitForSeconds waitBlast = new WaitForSeconds(Tile.BLAST_DURATION);

		private void Awake()
		{
			Setup();
		}

		private void OnEnable()
		{
			Tile.OnTappedToTile += AddTileToDeck;
			HolderGroup.OnBlast += OnBlast;
		}

		private void OnDisable()
		{
			Tile.OnTappedToTile -= AddTileToDeck;
			HolderGroup.OnBlast -= OnBlast;
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

				var deckGroup = Instantiate(holderGroupPrefab, transform);
				deckGroup.gameObject.SetActive(false);
				holderGroupPool.Enqueue(deckGroup);
			}
		}

		public void AddTileToDeck(Tile tile)
		{
			var tenGroup = FindTen(tile);
			if (tenGroup)
			{
				TenBlast(tile, tenGroup);
			}
			else
			{
				if (!holderGroupPool.TryDequeue(out var newGroup)) return;

				newGroup.gameObject.SetActive(true);
				var tileCount = GetTotalTileCount();
				Debug.Log(tileCount);
				newGroup.Setup(holderSlots[tileCount]);
				newGroup.AddTile(tile);
				holderGroups.Add(newGroup);

				// Check if the holder is full and lose the game
				if (GetTotalTileCount().Equals(slotCount))
				{
					LevelManager.Instance.Lose();
				}
			}

			RearrangeGroups();
		}

		private void TenBlast(Tile tile, HolderGroup tenGroup)
		{
			tile.Jump(tenGroup.transform.position + 1 * Vector3.up);
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

		private void OnBlast(HolderGroup holderGroup)
		{
			StartCoroutine(WaitBlast());
			return;

			IEnumerator WaitBlast()
			{
				yield return waitBlast;

				holderGroups.Remove(holderGroup);
				holderGroupPool.Enqueue(holderGroup);
				holderGroup.gameObject.SetActive(false);
				RearrangeGroups();
			}
		}

		public HolderGroup FindTen(Tile tile)
		{
			for (int i = 0; i < holderGroups.Count; i++)
			{
				for (var j = 0; j < holderGroups[i].Tiles.Count; j++)
				{
					if ((int)holderGroups[i].Tiles[j].Type + (int)tile.Type == GameManager.BLAST_COUNT)
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