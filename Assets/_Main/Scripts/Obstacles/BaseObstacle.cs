using GridSystem.Tiles;
using TriInspector;
using UnityEngine;

namespace Obstacles
{
	[DeclareFoldoutGroup("Properties")]
	public abstract class BaseObstacle : MonoBehaviour
	{
		[field: SerializeField, ReadOnly, Group("Properties")] public Tile AttachedTile { get; set; }
		public abstract bool IsBlockingMovement { get; }

		public abstract bool OnTapped();

		public virtual void Setup(Tile tile)
		{
			AttachedTile = tile;
		}

		public virtual void DestroyObstacle()
		{
			Destroy(gameObject);
		}
	}
}