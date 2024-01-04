using UnityEngine;
using RocknFall.Bases.Interfaces;
using RocknFall.Bases.SO;
using RocknFall.Entities;
using System.Collections.Generic;

namespace RocknFall.Platforms
{
    public class KillPlatform : Platform, IKiller
    {
        [Header("Kill Platform")]
        [SerializeField] BoolValue isPlayerDead;
        [SerializeField] BoolValue hasBubbleProtection;
        [SerializeField] GameObject UIPartPrefab;

        #region Initialization
        private onCollisionWithEntity OnCollisionFunction;

        private void Awake()
        {
            // We subscribe to the necessary events
            OnCollisionFunction = (GameObject entity, Vector2 contactPoint) => BeforeKilling(entity, contactPoint);
            OnCollisionWithEntity += OnCollisionFunction;
        }
        private new void OnDestroy()
        {
            base.OnDestroy();

            // Unsubscribe to events
            OnCollisionWithEntity -= OnCollisionFunction;
        }

        public void SetUI(float scaleX)
        {
            // Set values
            float maxOffset = (scaleX / 2f) / GameData.scaleRatio;
            List<GameObject> newPlatforms = new List<GameObject>();

            // Create several ones next to each other
            float lastOffset = 0f;
            for (float xOffset = -maxOffset; xOffset < maxOffset; xOffset += UIPartPrefab.transform.lossyScale.x)
            {
                GameObject newPlatform = Instantiate(UIPartPrefab, base.UIPart.transform);
                newPlatform.transform.position = transform.position + Vector3.right * xOffset;

                newPlatforms.Add(newPlatform);
                lastOffset = xOffset;
            }

            // Modify its collider size
            BoxCollider2D m_collider = GetComponent<BoxCollider2D>();
            m_collider.offset = new Vector2(lastOffset - maxOffset, m_collider.offset.y);
            m_collider.size = new Vector2(UIPartPrefab.transform.lossyScale.x * newPlatforms.Count, UIPartPrefab.transform.lossyScale.y);
        }
        #endregion

        public void BeforeKilling(GameObject entity, Vector2 contactPoint)
        {
            // Check if the entity is the player and if it isn't in fire mode
            if(entity.TryGetComponent(out Player player) && !player.IsOnFireMode && !isPlayerDead.Value)
            {
                // If it is protected by the bubble, remove its protection only
                if (hasBubbleProtection.Value)
                {
                    // Play the bubble break sound
                    soundMessage.SendMessage("Clip" + ((int)ClipIndex.BubbleExplode).ToString());
                    // Set that the player is no longer protected by the bubbles
                    hasBubbleProtection.Value = false;
                    return;
                }

                // If so, then kill it
                Kill(player, contactPoint);
            }
        }

        public void Kill(IKillable entity, Vector2 impactPoint, float time = 0f)
        {
            // Play the player death sound
            soundMessage.SendMessage("Clip" + ((int)ClipIndex.PlayerDeath).ToString());

            // Kill the entity
            entity.Die(impactPoint, time);
        }
    }
}
