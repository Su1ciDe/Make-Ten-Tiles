using Fiber.Utilities;
using ScriptableObjects;
using UnityEngine;

namespace Fiber.Managers
{
	[DefaultExecutionOrder(-1)]
	public class GameManager : Singleton<GameManager>
	{
		[SerializeField] private PrefabsSO prefabsSO;
		public PrefabsSO PrefabsSO => prefabsSO;

		[SerializeField] private ColorsSO colorsSO;
		public ColorsSO ColorsSO => colorsSO;

		public const int BLAST_COUNT = 10;

		private void Awake()
		{
			Application.targetFrameRate = 60;
			Debug.unityLogger.logEnabled = Debug.isDebugBuild;
			Input.multiTouchEnabled = false;
		}
	}
}