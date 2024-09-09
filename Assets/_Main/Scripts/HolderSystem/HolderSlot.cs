using TMPro;
using UnityEngine;

namespace HolderSystem
{
	public class HolderSlot : MonoBehaviour
	{
		public bool IsLocked { get; set; }
		public float Size => size;
		[SerializeField] private float size;
		[SerializeField] private GameObject lockIcon;
		[SerializeField] private TextMeshPro txtUnlockLevel;

		private int unlockLevel;

		public void Lock(int _unlockLevel)
		{
			IsLocked = true;
			unlockLevel = _unlockLevel;
			lockIcon.SetActive(true);
			txtUnlockLevel.SetText("LVL " + unlockLevel.ToString());
		}

		public void Unlock()
		{
			IsLocked = false;
			lockIcon.SetActive(false);
		}
	}
}