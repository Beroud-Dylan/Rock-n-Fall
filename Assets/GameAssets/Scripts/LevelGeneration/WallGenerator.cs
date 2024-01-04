using UnityEngine;
using RocknFall.Bases.SO;

namespace RocknFall.LevelGeneration
{
    public class WallGenerator : MonoBehaviour
    {
        [SerializeField] GameObject wallPrefab;
        [SerializeField] Transform wallParent;
        private GameObject[] walls;

        [SerializeField] MainCamera mainCamera;
        [SerializeField] SceneTheme sceneTheme;
        private MainCamera.onValueChange onCameraChangeFunction;
        private MainCamera.onCameraShake onCameraShakeFunction;

        #region Initialization
        private void Start()
        {
            // Subscribe to events
            onCameraChangeFunction = (Camera cam) => OnCameraChange(cam);
            mainCamera.OnValueChange += onCameraChangeFunction;
            onCameraShakeFunction = (float duration, float magnitude) => OnCameraShake();
            mainCamera.OnCameraShake += onCameraShakeFunction;
            mainCamera.OnCameraStopShaking += OnCameraStopShaking;

            // Set the game's screen size
            OnCameraChange(mainCamera.Cam);
        }

        private void OnDestroy()
        {
            // Unsubscribe to events
            mainCamera.OnValueChange -= onCameraChangeFunction;
            mainCamera.OnCameraShake -= onCameraShakeFunction;
            mainCamera.OnCameraStopShaking -= OnCameraStopShaking;
        }
        #endregion

        /// <summary>
        /// Whenever the camera changes in game.
        /// </summary>
        /// <param name="cam">The new camera.</param>
        private void OnCameraChange(Camera cam)
        {
            // Get the screen positions of the walls
            Vector2 rightWallScreenPos = new Vector2(Screen.width, 0f);
            Vector2 leftWallScreenPos = new Vector2(0f, 0f);
            // Get their scale
            float heightScale = cam.ScreenToWorldPoint(Screen.height * Vector2.up).y * 2f * GameData.scaleRatio;

            // Spawn the walls at their position
            GameObject rightWall = Instantiate(wallPrefab, new Vector2(cam.ScreenToWorldPoint(rightWallScreenPos).x + (wallPrefab.transform.localScale.x / (2f * GameData.scaleRatio)), 0f), Quaternion.identity, wallParent);
            GameObject leftWall = Instantiate(wallPrefab, new Vector2(cam.ScreenToWorldPoint(leftWallScreenPos).x - (wallPrefab.transform.localScale.x / (2f * GameData.scaleRatio)), 0f), Quaternion.identity, wallParent);

            // Set their new scale adapted to the screen size
            rightWall.transform.localScale = new Vector3(wallPrefab.transform.localScale.x, heightScale, wallPrefab.transform.localScale.z);
            leftWall.transform.localScale = new Vector3(wallPrefab.transform.localScale.x, heightScale, wallPrefab.transform.localScale.z);

            // Set the right color
            rightWall.GetComponent<SpriteRenderer>().color = sceneTheme.WallColor;
            leftWall.GetComponent<SpriteRenderer>().color = sceneTheme.WallColor;

            // Add them in the array
            walls = new GameObject[2];
            walls[0] = rightWall;
            walls[1] = leftWall;
        }

        /// <summary>
        /// Whenever the camera shakes in game.
        /// </summary>
        private void OnCameraShake()
        {
            // Deactivate the sprite renderers of the walls (to prevent seeing them weirdly)
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        /// <summary>
        /// Whenever the camera stops shaking in game.
        /// </summary>
        private void OnCameraStopShaking()
        {
            // Reactivate the sprite renderers of the walls (to make them viewable again)
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
}
