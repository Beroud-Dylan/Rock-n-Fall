using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using RocknFall.Bases;
using RocknFall.Bases.SO;
using RocknFall.Bases.Interfaces;

namespace RocknFall.Entities
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(BlinkEffect))]
    public class Player : MonoBehaviour, IKillable, IKiller
    {
        [Header("Juicy Elements")]
        [SerializeField] GameObject groundParticlesPrefab;
        private BlinkEffect blinkEffect;
        private BlinkEffect.OnColorModified onColorModifiedFunction;
        private Gradient CurrentColorGradient
        {
            get => boostFireMode.Value > 0f ? sceneTheme.mainTheme.FireModeTheme.playerColor : sceneTheme.mainTheme.NormalModeTheme.playerColor;
        }

        [SerializeField] GameObject bubbleProtectionUI;
        [SerializeField] GameObject bubbleDestroyParticles;
        private bool destroyOnce;
        private SpriteRenderer bubbleProtectionRenderer;

        [Header("Stats")]
        [SerializeField] float speed;
        [SerializeField] float bounceForce;
        [SerializeField] float stretchingSpeed;
        private Vector3 baseScale;
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;

        private float _playerOffsetX;
        private float _playerStartPosX;
        private float _startPosX;
        private bool _notStartPos;

        private bool _isOnFireMode;
        public bool IsOnFireMode
        {
            get => _isOnFireMode || boostFireMode.Value > 0f;
        }

        [Header("Dependencies")]
        [SerializeField] BoolValue isDead;
        [SerializeField] FloatValue boostFireMode;
        private Vector2 previousVelocity;
        [SerializeField] FloatValue minVelocity;
        [SerializeField] MainCamera mainCamera;
        [SerializeField] BoolValue hasBubbleProtection;
        [SerializeField] MessageSO soundMessage;
        [SerializeField] SceneTheme sceneTheme;

        public delegate void onPlayerDeath();
        public event onPlayerDeath OnPlayerDeath;

        private void Start()
        {
            // Initialize the values
            _playerOffsetX = 0;

            // Set the right start values for the SO
            isDead.Value = false;
            boostFireMode.Value = 0f;
            minVelocity.Value = -GameData.fireModeThreshold;            
            hasBubbleProtection.Value = false;

            // Get the components
            rb = GetComponent<Rigidbody2D>();
            bubbleProtectionRenderer = bubbleProtectionUI.GetComponent<SpriteRenderer>();

            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = CurrentColorGradient.Evaluate(0f);

            blinkEffect = GetComponent<BlinkEffect>();
            blinkEffect.baseColor = CurrentColorGradient.Evaluate(0f);
            blinkEffect.blinkColor = sceneTheme.PlayerBlinkColor;

            baseScale = transform.localScale;

            // Subscribe to events
            onColorModifiedFunction = (Color modifiedColor) => 
            {
                blinkEffect.baseColor = GameData.GetColorBySpeed(CurrentColorGradient, Mathf.Abs(rb.velocity.y));
                spriteRenderer.color = modifiedColor;
            };
            blinkEffect.onColorModified += onColorModifiedFunction;
        }
        private void OnDestroy()
        {
            // Unsubscribe to events
            blinkEffect.onColorModified -= onColorModifiedFunction;

            // Delete the events
            OnPlayerDeath = null;
        }

        private void Update()
        {
            // Set the color depending on speed
            if (!blinkEffect.IsBlinking)
            {
                spriteRenderer.color = GameData.GetColorBySpeed(CurrentColorGradient, Mathf.Abs(rb.velocity.y));
            }

            // If the player is protected by a bubble 
            if (hasBubbleProtection.Value)
            {
                // Set the bubble ui part visible and set the right color to it
                bubbleProtectionUI.SetActive(true);
                bubbleProtectionRenderer.color = spriteRenderer.color;
                destroyOnce = false;
            }
            else
            {
                if (!destroyOnce)
                {
                    // Start the particles
                    bubbleDestroyParticles.SetActive(false);
                    bubbleDestroyParticles.SetActive(true);

                    // Deactivate the UI
                    bubbleProtectionUI.SetActive(false);
                    destroyOnce = true;
                }
            }

            // If the player is dead
            if (isDead.Value)
            {
                return;
            }

            // Determinate if we are clicking on screen (so on computer if the mouse is clicked, and on mobile if there is at least 1 finger on screen)
            bool isClicking = SystemInfo.deviceType == DeviceType.Desktop ? Input.GetKey(KeyCode.Mouse0) : SystemInfo.deviceType == DeviceType.Handheld ? Input.touchCount > 0 : false;

            // If we click on screen
            if (isClicking)
            {
                // If it is the first position
                if (!_notStartPos)
                {
                    // Set the start position of the player
                    _playerStartPosX = transform.position.x;
                    // Get the start position of what has clicked (so on computer, the mouse's position, and on mobile devices, the first finger's position)
                    _startPosX = SystemInfo.deviceType == DeviceType.Desktop ? mainCamera.Cam.ScreenToWorldPoint(Input.mousePosition).x : mainCamera.Cam.ScreenToWorldPoint(Input.touches[0].position).x;

                    // Says that it is no longer the first position
                    _notStartPos = true;
                    return;
                }

                // Get the player offset depending on the device we're using
                _playerOffsetX = SystemInfo.deviceType == DeviceType.Desktop ? mainCamera.Cam.ScreenToWorldPoint(Input.mousePosition).x - _startPosX : mainCamera.Cam.ScreenToWorldPoint(Input.touches[0].position).x - _startPosX;
            }
            else
            {
                // Else, reset the bool, since it can't be the first pos
                _notStartPos = false;
            }

            // If the vertical velocity gets really high, we enter fire mode
            if (Mathf.Abs(rb.velocity.y) >= GameData.fireModeThreshold)
            {
                _isOnFireMode = true;

                // Clamp the rb velocity
                float yVelocity = Mathf.Clamp(rb.velocity.y, minVelocity.Value, Mathf.Abs(minVelocity.Value));
                rb.velocity = new Vector2(rb.velocity.x, yVelocity);
            }
            else
            {
                _isOnFireMode = false;

                // If the player is no longer falling
                if(rb.velocity.y >= 0f)
                {
                    // Reset its max down velocity
                    minVelocity.Value = -GameData.fireModeThreshold;
                }
            }

            // Set the previous velocity
            previousVelocity = rb.velocity;
        }

        private void FixedUpdate()
        {
            // Stretch the player depending on its velocity
            Vector2 stretchingForce = Vector2.zero;
            // If the player moves more on the horizontal axis than the vertical one
            if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(rb.velocity.y))
            {
                // Use the horizontal axis as the reference
                stretchingForce.y = 1f - ((Mathf.Abs(rb.velocity.x) + 0.1f) / speed) * GameData.stretchFactor;
                stretchingForce.x = 1f / stretchingForce.y;
            }
            else
            {
                // Use the vertical axis as the reference
                stretchingForce.x = 1f - ((Mathf.Abs(rb.velocity.y) + 0.1f) / GameData.maxVelocity) * GameData.stretchFactor;
                stretchingForce.y = 1f / stretchingForce.x;
            }
            // Set the new player's scale
            transform.localScale = Vector2.Lerp(transform.localScale, stretchingForce * baseScale, stretchingSpeed * Time.fixedDeltaTime);

            // If the player is dead, it shouldn't be able to move
            if (isDead.Value)
            {
                return;
            }

            // Decrease the fire mode boost
            if (boostFireMode.Value > 0f)
            {
                boostFireMode.Value -= Time.fixedDeltaTime;

                // Get the y velocity
                float yVelocity = rb.velocity.y;
                // If the player doesn't go at the minimum speed in fire mode boos
                if (Mathf.Abs(rb.velocity.y) > 0f && Mathf.Abs(rb.velocity.y) < GameData.fireModeThreshold)
                {
                    // Set the velocity to be the one the player should be when in fire mode
                    yVelocity = rb.velocity.y > 0f ? GameData.fireModeThreshold : -GameData.fireModeThreshold;
                }

                // Set the y velocity
                rb.velocity = new Vector2(rb.velocity.x, yVelocity);
            }
            else
            {
                // Reset the fire mode boost's value
                boostFireMode.Value = 0f;
            }

            // Move the player toward an offset position
            Vector3 direction = _playerOffsetX * Vector2.right;
            transform.position = Vector2.MoveTowards(transform.position, new Vector3(_playerStartPosX, transform.position.y, transform.position.z) + direction, speed * Time.fixedDeltaTime);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Vector2 contactPoint = GameData.GetCollisionPointFrom(other.contacts);
            bool canStillApplyFallParticles = true;
            bool playNormalBounceSound = true;

            // If we collide with the background, make a special sound
            if (other.gameObject.CompareTag(GameData.BACKGROUND_TAG))
            { 
                soundMessage.SendMessage("Clip" + ((int)ClipIndex.NormalBounceBack).ToString());
            }

            // As long as the player with anything that isn't a bounce platform
            if (!other.gameObject.CompareTag(GameData.BOUNCE_PLATFORM_TAG))
            {
                // Make the camera shake a little bit
                float duration = GameData.baseShakeDuration * previousVelocity.magnitude;
                float magnitude = GameData.baseShakeMagnitude * previousVelocity.magnitude;
                mainCamera.ActivateShakingFunctions(duration, magnitude);
            }
            else
            {
                // If this is the bounce platform, make a special sound
                soundMessage.SendMessage("Clip" + ((int)ClipIndex.BouncePlatform).ToString());
            }

            // If the player collides with a platform
            if (GameData.IsPlatform(other.gameObject.tag) && !isDead.Value)
            {
                // If we were in fire mode and in the play scene
                if (IsOnFireMode && SceneManager.GetActiveScene().buildIndex == (int)SceneIndex.PLAY && other.gameObject.TryGetComponent(out IKillable killable))
                {
                    // Kill the platform
                    Kill(killable, contactPoint);
                    // Set that we can no longer add ground particle effects, since there is no more ground
                    canStillApplyFallParticles = false;
                    playNormalBounceSound = false;

                    // Set that the player enters "blink" mode
                    blinkEffect.Blink();
                    mainCamera.ActivateBackgroundBlinkEffect();
                }

                // Make the player bounce, if we aren't in unlimited fire mode
                if (boostFireMode.Value <= 0f)
                {
                    // Apply the bounce force
                    rb.velocity = new Vector2(rb.velocity.x, bounceForce);

                    // Make the bounce sound if needed
                    if (playNormalBounceSound)
                    {
                        soundMessage.SendMessage("Clip" + ((int)ClipIndex.NormalBounceBack).ToString());
                    }
                }
                else
                {
                    // Add the velocity back
                    rb.velocity = new Vector2(rb.velocity.x, previousVelocity.y);
                }                
            }

            // If the player is on the ground and didn't destroy the platform
            if (canStillApplyFallParticles)
            {
                // Get the particles informations
                Color color = other.gameObject.GetComponent<SpriteRenderer>() != null ? other.gameObject.GetComponent<SpriteRenderer>().color : spriteRenderer.color;
                // Apply ground particles
                StartCoroutine(SpawnParticles(groundParticlesPrefab, contactPoint, ((Vector2)transform.position - contactPoint).normalized, color));
            }
        }

        IEnumerator SpawnParticles(GameObject prefab, Vector2 position, Vector2 direction, Color color)
        {
            // Spawn the particles
            GameObject particles = Instantiate(prefab, position, Quaternion.identity);
            // Calculate its rotation
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            particles.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

            // Set its properties
            particles.GetComponent<ParticleSystem>().startColor = color;
            particles.GetComponent<ParticleSystem>().Play();

            // Destroy it when not needed
            yield return new WaitForSeconds(GameData.particlesTimeWait);
            Destroy(particles);
        }

        public void Kill(IKillable killable, Vector2 impactPoint, float time = 0f)
        {
            // Kill what's killable
            killable.Die(impactPoint, time);
        }

        public void Die(Vector2 impactPoint, float time = 0f, bool canPlaySoundOnDeath = true)
        {
            // Set that the player is dead
            isDead.Value = true;

            // Activate the functions whenever the player dies
            OnPlayerDeath?.Invoke();
        }
    }
}
