using UnityEngine;
using RocknFall.Bases.SO;

namespace RocknFall.Platforms
{
    public class CloudPlatform : Platform
    {
        [SerializeField] FloatValue minPlayerVelocity;
        [SerializeField] float speedBoost;

        #region Initialization
        private onCollisionWithEntity OnCollisionFunction;

        private void Awake()
        {
            // We subscribe to the necessary events
            OnCollisionFunction = (GameObject entity, Vector2 contactPoint) => ActivateFireMode(entity);
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
        /// This function is called whenever an entity collides with this platform. It will make the entity go to fire mode !
        /// </summary>
        /// <param name="entity">The entity that will enter fire mode.</param>
        public void ActivateFireMode(GameObject entity)
        {
            // If the entity has a rigidbody
            if (entity.TryGetComponent(out Rigidbody2D rb))
            {
                // Modify the speed limit
                minPlayerVelocity.Value = Mathf.Clamp(minPlayerVelocity.Value - speedBoost, -GameData.maxVelocity, 0f);

                // Change its velocity so that it enters fire mode or go beyond
                float yVelocity = rb.velocity.y < -GameData.fireModeThreshold ? rb.velocity.y - speedBoost : -GameData.fireModeThreshold;
                rb.velocity = new Vector2(rb.velocity.x, yVelocity);

                // Play the cloud boost sound
                soundMessage.SendMessage("Clip" + ((int)ClipIndex.BoostClouds).ToString());

                // Destroy the cloud after its use
                base.Die(transform.position, 0.25f, false);
            }
        }
    }
}
