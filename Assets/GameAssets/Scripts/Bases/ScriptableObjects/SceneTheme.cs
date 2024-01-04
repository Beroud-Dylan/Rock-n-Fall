using UnityEngine;

namespace RocknFall.Bases.SO
{
    [CreateAssetMenu(fileName = "Scene Theme Name", menuName = "RocknFall/Custom/SceneTheme")]
    public class SceneTheme : ScriptableObject
    {
        [HideInInspector] public Theme mainTheme;

        [Header("Static properties")]
        [SerializeField] Color backgroundBlinkColor;
        public Color BackgroundBlinkColor { get => backgroundBlinkColor; }

        public Color PlayerBlinkColor { get => playerBlinkColor; }
        [SerializeField] Color playerBlinkColor;

        public Color WallColor { get => wallColor; }
        [SerializeField] Color wallColor;

        [Header("List of possible themes")]
        [SerializeField] Theme[] themes;
        public Theme[] Themes { get => themes; }
    }
}
