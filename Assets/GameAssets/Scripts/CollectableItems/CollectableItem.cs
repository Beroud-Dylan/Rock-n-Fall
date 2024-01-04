using UnityEngine;
using RocknFall.Bases.SO;
using System.Collections;

namespace RocknFall.CollectableItems
{
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class CollectableItem : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] BoolValue isPlayerDead;
        [SerializeField] protected MessageSO soundMessage;

        [Header("Particles")]
        [SerializeField] GameObject particles;
        private Collider2D itemCollider2D;
        private SpriteRenderer spriteRenderer;

        public delegate void onCollisionWithPlayer();
        public event onCollisionWithPlayer OnCollisionWithPlayer;

        protected void Start()
        {
            // Get the components
            itemCollider2D = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Disable the particles
            particles.SetActive(false);
        }
        protected void OnDestroy()
        {
            // Delete the events
            OnCollisionWithPlayer = null;
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            // If the object the collectable item is colliding with is the player
            if (collision.gameObject.CompareTag(GameData.PLAYER_TAG) && !isPlayerDead.Value)
            {
                // Trigger the event
                OnCollisionWithPlayer?.Invoke();
                // Then delete itself
                StartCoroutine(PlayParticleAndDie());
            }
        }
        protected void OnTriggerStay2D(Collider2D collision)
        {
            // Destroy itself
            Destroy(gameObject);
        }

        IEnumerator PlayParticleAndDie()
        {
            // Disable the useless parts
            spriteRenderer.enabled = false;
            itemCollider2D.enabled = false;

            // Enable the particles
            particles.SetActive(true);

            // Wait 1 second
            yield return new WaitForSeconds(1f);

            // Destroy itself
            Destroy(gameObject);
        }
    }
}