using UnityEngine;
using RocknFall.Bases.SO;
using RocknFall.Bases.Interfaces;
using System.Collections;

namespace RocknFall.Platforms
{
    [RequireComponent(typeof(Collider2D))]
    public class Platform : MonoBehaviour, IKillable
    {
        [Header("Platform")]
        public GameObject UIPart;
        [SerializeField] GameObject particles;
        [SerializeField] protected MessageSO soundMessage;

        protected void OnDestroy()
        {
            // Delete all the events
            OnCollisionWithEntity = null;
            OnCollisionEntityExit = null;
        }

        public void Die(Vector2 impactPoint, float time = 0f, bool canPlayOnDeath = true)
        {
            if (canPlayOnDeath)
            {
                // Play a death sound
                soundMessage.SendMessage("Clip" + ((int)ClipIndex.PlatformBreak).ToString());
            }

            // Start the particle effect
            StartCoroutine(Death(impactPoint, time));
        }

        IEnumerator Death(Vector2 impactPoint, float time)
        {
            // Deactivate what's not needed anymore
            UIPart.SetActive(false);
            GetComponent<Collider2D>().enabled = false;

            // Activate the particles
            particles.transform.position = impactPoint;
            particles.SetActive(true);

            // Wait a certain time before dying
            yield return new WaitForSeconds(GameData.particlesTimeWait + time);

            // Destroy this gameObject
            Destroy(gameObject);
        }

        #region Events
        public delegate void onCollisionWithEntity(GameObject entity, Vector2 contactPoint);
        public event onCollisionWithEntity OnCollisionWithEntity;

        public delegate void onCollisionEntityExit(GameObject entity);
        public event onCollisionEntityExit OnCollisionEntityExit;

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            // If the object we are colliding with is killable (so is an entity)
            if (collision.gameObject.TryGetComponent(out IKillable killable))
            {
                // Trigger the event
                OnCollisionWithEntity?.Invoke(collision.gameObject, Vector2.zero);
            }
        }
        protected void OnCollisionEnter2D(Collision2D collision)
        {
            // If the object we are colliding with is killable (so is an entity)
            if (collision.gameObject.TryGetComponent(out IKillable killable))
            {
                // Get the contact point 
                Vector2 contactPoint = GameData.GetCollisionPointFrom(collision.contacts);

                // Trigger the event
                OnCollisionWithEntity?.Invoke(collision.gameObject, contactPoint);
            }
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            // If the object we are no longer colliding with is killable (so is an entity)
            if (collision.gameObject.TryGetComponent(out IKillable killable))
            {
                // Trigger the event
                OnCollisionEntityExit?.Invoke(collision.gameObject);
            }
        }
        protected void OnCollisionExit2D(Collision2D collision)
        {
            // If the object we are no longer colliding with is killable (so is an entity)
            if (collision.gameObject.TryGetComponent(out IKillable killable))
            {
                // Trigger the event
                OnCollisionEntityExit?.Invoke(collision.gameObject);
            }
        }
        #endregion
    }
}
