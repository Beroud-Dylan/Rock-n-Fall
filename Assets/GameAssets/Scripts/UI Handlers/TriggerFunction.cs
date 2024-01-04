using UnityEngine;

namespace RocknFall.UIHandlers
{
    [RequireComponent(typeof(Collider2D))]
    public class TriggerFunction : MonoBehaviour
    {
        public delegate void onTriggerEnterFunction();
        public event onTriggerEnterFunction OnTriggerEnterFunction;

        public delegate void onTriggerExitFunction();
        public event onTriggerExitFunction OnTriggerExitFunction;

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            // If this is the player
            if (collision.CompareTag(GameData.PLAYER_TAG))
            {
                // Trigger the event
                OnTriggerEnterFunction?.Invoke();
            }
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            // If this was the player
            if (collision.CompareTag(GameData.PLAYER_TAG))
            {
                // Trigger the event
                OnTriggerExitFunction?.Invoke();
            }
        }

        protected void OnDestroy()
        {
            // Delete the events
            OnTriggerEnterFunction = null;
        }
    }
}
