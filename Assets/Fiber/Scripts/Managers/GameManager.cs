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
		
		private void Awake()
		{
			Application.targetFrameRate = 60;
			Debug.unityLogger.logEnabled = Debug.isDebugBuild;
		}
	}
}