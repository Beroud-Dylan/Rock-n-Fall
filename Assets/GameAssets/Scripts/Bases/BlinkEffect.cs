using UnityEngine;

namespace RocknFall.Bases
{
    public class BlinkEffect : MonoBehaviour
    {
        [HideInInspector] public Color blinkColor;
        [HideInInspector] public Color baseColor;
        private Color previousColor;

        [SerializeField] float blinkTime;
        private float blinkCooldown;
        private bool isBlinking;
        public bool IsBlinking { get => isBlinking; }

        public delegate void OnColorModified(Color modifiedColor);
        public event OnColorModified onColorModified;

        #region Initialization
        private void Start()
        {
            // Set the right color
            onColorModified?.Invoke(baseColor);
        }
        private void OnDestroy()
        {
            onColorModified = null;
        }
        #endregion

        /// <summary>
        /// Make anything have a color that will blink from a base color to a blinked color.
        /// </summary>
        /// <param name="time">The duration of the blink effect.</param>
        public void Blink(float time = -1f)
        {
            isBlinking = true;
            blinkTime = time != -1f ? time : blinkTime;
        }        

        private void Update()
        {
            // If it is in blinking
            if (isBlinking)
            {
                // If there is still time left
                if (blinkCooldown < blinkTime)
                {
                    // Increase the countdown
                    blinkCooldown += Time.deltaTime;

                    // Make the blink effect
                    Color currentColor = previousColor == baseColor ? blinkColor : baseColor;
                    previousColor = currentColor;
                    onColorModified?.Invoke(currentColor);
                }
                else
                {
                    // Else, just reset the values
                    isBlinking = false;
                    blinkCooldown = 0f;
                    onColorModified?.Invoke(baseColor);
                }
            }
        }
    }
}
