using UnityEditor;
using UnityEngine;

namespace GridSystem
{
	[InitializeOnLoad]
	public class LevelEditor : Editor
	{
		private static int currentLayerIndex;
		private static int layerCount;

		static LevelEditor()
		{
			SceneView.duringSceneGui -= OnDuringSceneGui;
			SceneView.duringSceneGui += OnDuringSceneGui;

			if (!GridManager.Instance) return;
			layerCount = currentLayerIndex = GridManager.Instance.GridCells.GetLength(0);
		}

		private static void ResetGrid()
		{
			layerCount = currentLayerIndex = GridManager.Instance.GridCells.GetLength(0);
			for (int i = 0; i < GridManager.Instance.GridCells.GetLength(0); i++)
			{
				var layer = GridManager.Instance.GetLayerTransform(i);
				if (SceneVisibilityManager.instance.IsHidden(layer.gameObject, true))
					SceneVisibilityManager.instance.ToggleVisibility(layer.gameObject, true);
			}
		}

		private static void OnDuringSceneGui(SceneView obj)
		{
			if (!GridManager.Instance) return;

			Handles.BeginGUI();
			{
				GUILayout.BeginArea(new Rect(10, 10, 100, 100));
				{
					if (GUILayout.Button("▲", GUILayout.Width(100), GUILayout.Height(35)))
					{
						if (!currentLayerIndex.Equals(layerCount))
						{
							var layer = GridManager.Instance.GetLayerTransform(currentLayerIndex);
							SceneVisibilityManager.instance.ToggleVisibility(layer.gameObject, true);
							currentLayerIndex = Mathf.Clamp(currentLayerIndex + 1, 0, layerCount);
						}
					}

					if (GUILayout.Button("▼", GUILayout.Width(100), GUILayout.Height(35)))
					{
						if (!currentLayerIndex.Equals(0))
						{
							currentLayerIndex = Mathf.Clamp(currentLayerIndex - 1, 0, layerCount - 1);
							var layer = GridManager.Instance.GetLayerTransform(currentLayerIndex);
							SceneVisibilityManager.instance.ToggleVisibility(layer.gameObject, true);
						}
					}

					if (GUILayout.Button("Reset"))
					{
						ResetGrid();
					}
				}
				GUILayout.EndArea();
			}
			Handles.EndGUI();
		}
	}
}