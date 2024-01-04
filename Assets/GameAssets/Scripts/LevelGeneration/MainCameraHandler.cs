using UnityEngine;
using RocknFall.Bases;
using RocknFall.Bases.SO;
using UnityEngine.SceneManagement;

namespace RocknFall.LevelGeneration
{
    [RequireComponent(typeof(Camera), typeof(BlinkEffect))]
    public class MainCameraHandler : MonoBehaviour
    {
        [Header("Target Movement")]
        [SerializeField] MainCamera mainCamera;
        [SerializeField] Transform yAxisMover;
        [SerializeField] Transform player;
        [SerializeField] [Range(0f, 1f)] float smoothSpeed;
        private Vector3 velocity;
        private bool hasPlayerReachBelowZero = false;

        [Header("Effects")]
        [SerializeField] FloatValue boostFireMode;
        [SerializeField] FloatValue minLevelHeight;
        [SerializeField] SceneTheme sceneTheme;
        [SerializeField] bool hasLevelColorChange;

        private Gradient BackgroundColor
        {
            get => boostFireMode.Value > 0f ? sceneTheme.mainTheme.FireModeTheme.backgroundColor : sceneTheme.mainTheme.NormalModeTheme.backgroundColor;
        }

        private BlinkEffect blinkEffect;
        private BlinkEffect.OnColorModified onColorModifiedFunction;
        private MainCamera.onBlinkEffect cameraBlinkEffectFunction;

        private MainCamera.onCameraShake cameraShakeFunction;
        private float shakeDuration;
        private float shakeMagnitude;
        private float elapsed;

        #region Initialization
        private void Awake()
        {
            // Set the camera's value
            mainCamera.Cam = GetComponent<Camera>();
        }
        private void Start()
        {
            // Get the components and assign the right values
            mainCamera.Cam.backgroundColor = sceneTheme.mainTheme.NormalModeTheme.backgroundColor.Evaluate(0f);

            blinkEffect = GetComponent<BlinkEffect>();
            blinkEffect.baseColor = mainCamera.Cam.backgroundColor;
            blinkEffect.blinkColor = sceneTheme.BackgroundBlinkColor;

            // Make the entrance effect only if the player is playing
            if (SceneManager.GetActiveScene().buildIndex == (int)SceneIndex.PLAY)
            {
                // Set the player's position at the top of the screen
                player.position = new Vector3(player.position.x, mainCamera.Cam.ScreenToWorldPoint(Screen.height * new Vector2(0f, 0.75f)).y, player.position.z);
            }

            // Subscribe to events
            cameraShakeFunction = (float duration, float magnitude) => Shake(duration, magnitude);
            mainCamera.OnCameraShake += cameraShakeFunction;

            cameraBlinkEffectFunction = (float duration) =>
            {
                blinkEffect.baseColor = mainCamera.Cam.backgroundColor;
                blinkEffect.Blink(duration);
            };
            mainCamera.OnBlinkEffect += cameraBlinkEffectFunction;
            onColorModifiedFunction = (Color modifiedColor) => { mainCamera.Cam.backgroundColor = modifiedColor; };
            blinkEffect.onColorModified += onColorModifiedFunction;
        }
        private void OnDestroy()
        {
            // Unsubscribe to events
            mainCamera.OnCameraShake -= cameraShakeFunction;
            mainCamera.OnBlinkEffect -= cameraBlinkEffectFunction;
            blinkEffect.onColorModified -= onColorModifiedFunction;
        }
        #endregion

        public void Shake(float duration, float magnitude)
        {
            shakeDuration = duration;
            shakeMagnitude = magnitude;
        }

        private void Update()
        {
            // If the camera must shake, make a countdown
            if(shakeDuration > 0f)
            {
                // As long as the time elapsed hasn't been more than the shake duration
                if (elapsed < shakeDuration)
                {
                    // Increase the elapsed time
                    elapsed += Time.deltaTime;
                }
                else
                {
                    // Else, finish the shaking effect
                    shakeDuration = 0f;
                    elapsed = 0f;
                    transform.localPosition = new Vector3(0f, 0f, transform.localPosition.z);
                    mainCamera.DeactivateShakingFunctions();
                }
            }

            // Set the color depending on the level "state"
            if (!blinkEffect.IsBlinking && hasLevelColorChange)
            {
                mainCamera.Cam.backgroundColor = GameData.GetColorByHeight(BackgroundColor, player.position.y, minLevelHeight.Value);
            }
        }

        private void FixedUpdate()
        {
            // If this is the play scene
            if(SceneManager.GetActiveScene().buildIndex == (int)SceneIndex.PLAY)
            {
                // We start moving only when the player is below the middle of the screen
                if (player.position.y > 0f && !hasPlayerReachBelowZero)
                {
                    return;
                }

                // Set that the player has at least once reached below 0
                hasPlayerReachBelowZero = true;
            }

            // Initialize the desired position
            Vector3 desiredPosition = new Vector3(yAxisMover.position.x, player.position.y, yAxisMover.position.z);

            // Calculate the smooth position and assign it as the current position
            Vector3 smoothPosition = Vector3.SmoothDamp(yAxisMover.position, desiredPosition, ref velocity, smoothSpeed);
            yAxisMover.position = smoothPosition;

            // If the camera is shaking
            if(shakeDuration > 0f)
            {
                // Get the "shaked" coordinates
                float x = Random.Range(-1f, 1f) * shakeMagnitude;
                float y = Random.Range(-1f, 1f) * shakeMagnitude;

                // Move the camera along that position
                transform.localPosition = new Vector3(x, y, transform.localPosition.z);
            }
        }
    }
}
