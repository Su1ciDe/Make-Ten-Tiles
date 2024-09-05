using UnityEngine;

namespace Obstacles
{
	public class CageObstacle : BaseObstacle
	{
		public override bool IsBlockingMovement { get; } = true;

		public override bool OnTapped()
		{
			return !IsBlockingMovement;
		}

		public void Unlock(KeyObstacle key)
		{
			// TODO: unlock animation
			DestroyObstacle();
		}
	}
}