using UnityEngine;

namespace RocknFall.Bases.SO
{
    [CreateAssetMenu(fileName = "Main Camera", menuName = "RocknFall/MainCamera")]
    public class MainCamera : ScriptableObject
    {
        private Camera _cam;
        public Camera Cam
        {
            get => _cam;
            set
            {
                // Modify the value
                this._cam = value;

                // Trigger the event related to the value
                OnValueChange?.Invoke(this._cam);
            }
        }
        public delegate void onValueChange(Camera value);
        public event onValueChange OnValueChange;

        public delegate void onCameraShake(float duration, float magnitude);
        public event onCameraShake OnCameraShake;
        public delegate void onCameraStopShaking();
        public event onCameraStopShaking OnCameraStopShaking;

        public delegate void onBlinkEffect(float duration);
        public event onBlinkEffect OnBlinkEffect;

        public void ActivateShakingFunctions(float duration, float magnitude)
        {
            // Activate the shaking effect
            OnCameraShake?.Invoke(duration, magnitude);
        }
        public void DeactivateShakingFunctions()
        {
            // Stop the shaking effect
            OnCameraStopShaking?.Invoke();
        }

        public void ActivateBackgroundBlinkEffect(float duration = -1f)
        {
            // Activate the blinking effect
            OnBlinkEffect?.Invoke(duration);
        }

        private void OnDestroy()
        {
            // Delete the events
            OnValueChange = null;
            OnCameraShake = null;
            OnCameraStopShaking = null;
        }
    }
}
