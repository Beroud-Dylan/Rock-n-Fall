using UnityEngine;
using RocknFall.Bases.SO;

namespace RocknFall.Platforms
{
    public class FinalPlatform : Platform
    {
        [SerializeField] BoolValue isLevelFinished;
        [SerializeField] BoolValue isPlayerDead;

        #region Initialization
        private onCollisionWithEntity OnCollisionFunction;

        private void Awake()
        {
            // We subscribe to the necessary events
            OnCollisionFunction = (GameObject entity, Vector2 contactPoint) => OnLevelCompleted();
            OnCollisionWithEntity += OnCollisionFunction;
        }
        private new void OnDestroy()
        {
            base.OnDestroy();

            // Unsubscribe to events
            OnCollisionWithEntity -= OnCollisionFunction;
        }
        #endregion

        /// <summary>
        /// This function is called whenever the player touches the last platform and end the level.
        /// </summary>
        private void OnLevelCompleted()
        {
            // Set that the level is finished depending on whether or not the player is still alive
            isLevelFinished.Value = !isPlayerDead.Value;
        }
    }
}
