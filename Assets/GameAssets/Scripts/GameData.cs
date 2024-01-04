using UnityEngine;

namespace RocknFall
{
    public static class GameData
    {
        // TAGS
        public const string PLAYER_TAG = "Player";
        public const string PLATFORM_TAG = "Platform";
        public const string KILL_PLATFORM_TAG = "KillPlatform";
        public const string BOUNCE_PLATFORM_TAG = "BouncePlatform";
        public const string BACKGROUND_TAG = "Background";

        // THEMES PRICES
        public static int[] themePrices = new int[4] { 0, 500, 2000, 10000 };

        // VALUES
        public const float startGameSpeed = 1f;
        public const float maxPlatformTimeWait = 4f;

        public const float maxVelocity = 20f;
        public const float fireModeThreshold = 10f;
        public const float stretchFactor = 0.25f;

        public const float minPlatformSizeX = 3f;
        public const float maxPlatformSizeX = 11f;

        public const float scaleRatio = 3.125f;
        public const float particlesTimeWait = 1f;

        public const float baseShakeDuration = 0.025f;
        public const float baseShakeMagnitude = 0.025f;

        public const float timeToWaitForClickSound = 0.05f;

        // OFFSETS
        public const float heightOffsetBeforeDisappearing = 1f;
        public const float heightOffsetBeforeAppearing = 1f;
        
        public const float platformMinHeightOffset = 2f;
        public const float platformMaxHeightOffset = 4f;

        /// <summary>
        /// Get the contact point from the average of all the contact points.
        /// </summary>
        /// <param name="contacts">All the contact points.</param>
        /// <returns></returns>
        public static Vector2 GetCollisionPointFrom(ContactPoint2D[] contacts)
        {
            Vector2 position = Vector2.zero;
            for (int i = 0; i < contacts.Length; i++)
            {
                position += contacts[i].point;
            }
            position /= contacts.Length;

            return position;
        }

        /// <summary>
        /// Get the color of an object depending on its <paramref name="speed"/>.
        /// </summary>
        /// <param name="gradient">The gradient color of the object.</param>
        /// <param name="speed">The current speed of the object.</param>
        public static Color GetColorBySpeed(Gradient gradient, float speed)
        {
            return gradient.Evaluate(speed / fireModeThreshold);
        }
        /// <summary>
        /// Get the color of an object depending on its <paramref name="height"/> relative to the <paramref name="minHeight"/>.
        /// </summary>
        /// <param name="gradient">The gradient color of the object.</param>
        /// <param name="height">The current height of the object.</param>
        /// <param name="minHeight">The minimum heigth that the object can fall.</param>
        public static Color GetColorByHeight(Gradient gradient, float height, float minHeight)
        {
            return gradient.Evaluate(height / minHeight);
        }

        /// <summary>
        /// Check if the given <paramref name="tag"/> is from a platform.
        /// </summary>
        /// <param name="tag">The tag of the object.</param>
        public static bool IsPlatform(string tag)
        {
            return tag == PLATFORM_TAG || tag == KILL_PLATFORM_TAG;
        }
    }

    [System.Serializable]
    public struct SpawnableValues
    {
        public GameObject prefab;
        public Transform parent;
        [Range(0, 1000)] public int strength;
    }

    public enum SceneIndex : int
    {
        MENU = 0,
        PLAY = 1
    }    
}
