using UnityEngine;

namespace RocknFall.Bases.SO
{
	[CreateAssetMenu(fileName = "Theme Name", menuName = "RocknFall/Custom/Theme")]
	public class Theme : ScriptableObject
	{
		[Header("Mode Themes")]
		[SerializeField] ModeTheme normalModeTheme;
		public ModeTheme NormalModeTheme { get => normalModeTheme; }

		public ModeTheme FireModeTheme { get => fireModeTheme; }
		[SerializeField] ModeTheme fireModeTheme;
	}

	[System.Serializable]
	public struct ModeTheme
	{
		[Header("Environment")]
		public Gradient backgroundColor;

		[Header("Entities")]
		public Gradient playerColor;
		public GameObject fireParticlesPrefab;
	}
}
