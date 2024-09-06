using Cysharp.Threading.Tasks;
using GridSystem.Tiles;
using TriInspector;
using UnityEngine;

namespace Obstacles
{
	public class GlueObstacle : BaseObstacle
	{
		[field: SerializeField, ReadOnly, Group("Properties")] public Tile OtherAttachedTile { get; set; }
		public override bool IsBlockingMovement { get; } = true;

		public void Setup(Tile tile, Tile otherTile)
		{
			AttachedTile = tile;
			OtherAttachedTile = otherTile;

			otherTile.Obstacle = tile.Obstacle;

			transform.position = (tile.transform.position + otherTile.transform.position) / 2f;
			transform.rotation = Quaternion.LookRotation(otherTile.transform.position - tile.transform.position, Vector3.back);
		}

		public override bool OnTapped()
		{
			GlueAnimation();

			return !IsBlockingMovement;
		}

		private async void GlueAnimation()
		{
			await UniTask.WaitForSeconds(1);
			
			DestroyObstacle();
			AttachedTile.RemoveTile();
			OtherAttachedTile.RemoveTile();
		}
	}
}