using System.Collections.Generic;
using DG.Tweening;
using GridSystem.Tiles;
using UnityEngine;

namespace HolderSystem
{
	public class HolderGroup : MonoBehaviour
	{
		public HolderSlot CurrentSlot { get; private set; }

		public List<Tile> Tiles { get; } = new List<Tile>();

		private const float MOVE_SPEED = 2.5f;

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
			tile.Jump(new Vector3(Tiles.Count * CurrentSlot.Size, 0)).OnComplete(tile.OnTilePlaced);
			Tiles.Add(tile);
		}
	}
}