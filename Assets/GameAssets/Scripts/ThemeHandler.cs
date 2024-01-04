using UnityEngine;
using UnityEngine.SceneManagement;
using RocknFall.Bases.SO;

namespace RocknFall
{
    public class ThemeHandler : MonoBehaviour
    {
        [SerializeField] SceneTheme sceneTheme;

        private void Awake()
        {
            // Get the theme index
            int choseIndex = PlayerPrefs.GetInt("rghebfizhibfhqzuofijihouejcshpdqiofhuixjdshfbcuxhj", 0);

            // If this is the main menu, use the main theme
            if (SceneManager.GetActiveScene().buildIndex == (int)SceneIndex.MENU)
            {
                choseIndex = 0;
            }

            // Assign the right scene theme
            sceneTheme.mainTheme = sceneTheme.Themes[choseIndex];
        }
    }
}
