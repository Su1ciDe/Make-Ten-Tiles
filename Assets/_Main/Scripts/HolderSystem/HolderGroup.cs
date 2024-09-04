using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GridSystem.Tiles;
using UnityEngine;
using UnityEngine.Events;

namespace HolderSystem
{
	public class HolderGroup : MonoBehaviour
	{
		public HolderSlot CurrentSlot { get; private set; }

		public List<Tile> Tiles { get; private set; } = new List<Tile>();

		private const float MOVE_SPEED = 2.5f;
		private const int BLAST_AMOUNT = 3;

		public static event UnityAction<HolderGroup> OnBlast;

		public void Setup(HolderSlot holderSlot)
		{
			Tiles.Clear();

			CurrentSlot = holderSlot;

			transform.position = CurrentSlot.transform.position;
		}

		public void MoveToSlot(HolderSlot holderSlot)
		{
			if (holderSlot.Equals(CurrentSlot)) return;

			transform.DOMove(holderSlot.transform.position, MOVE_SPEED).SetSpeedBased(true).SetEase(Ease.OutQuint);

			CurrentSlot = holderSlot;
		}

		public void AddTile(Tile tile)
		{
			tile.transform.SetParent(transform);
			tile.Jump(new Vector3(Tiles.Count * CurrentSlot.Size, 0));
			Tiles.Add(tile);

			if (Tiles.Count.Equals(BLAST_AMOUNT))
			{
				StartCoroutine(Blast());
			}
		}

		private readonly WaitForSeconds jumpWait = new WaitForSeconds(Tile.JUMP_DURATION);

		private IEnumerator Blast()
		{
			yield return jumpWait;

			for (int i = 0; i < Tiles.Count; i++)
			{
				Tiles[i].Blast();
			}

			OnBlast?.Invoke(this);
		}
	}
}