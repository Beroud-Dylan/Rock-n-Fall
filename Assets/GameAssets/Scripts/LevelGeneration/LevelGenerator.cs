using UnityEngine;
using RocknFall.Bases.SO;
using RocknFall.Platforms;

namespace RocknFall.LevelGeneration
{
    public class LevelGenerator : MonoBehaviour
    {
        [Header("Spawnable Objects")]
        [SerializeField] GameObject finalPlatform;
        [SerializeField] Transform finalPlatformParent;
        [SerializeField] SpawnableValues[] platforms;
        [SerializeField] SpawnableValues[] coins;
        [SerializeField] SpawnableValues[] boosts;

        [Header("Dependencies")]
        [SerializeField] IntValue level;
        [SerializeField] FloatValue minLevelHeight;
        [SerializeField] MainCamera mainCam;

        private void Start()
        {
            // Set the right level number
            level.Value = PlayerPrefs.GetInt("efbhusndzidjksxndisjxnjq", 1);

            // The height evolution depends on the level and should evolve sigmoidally
            float rangeHeight = 10000f;
            float shift = 4238f;
            int levelWhereIntensityChanges = 50;
            float height = (float)(rangeHeight / (1f + Mathf.Exp(-0.005f * (level.Value - levelWhereIntensityChanges)))) - shift;

            // Generate the level
            GenerateLevel(height);
        }

        /// <summary>
        /// Generate the whole level.
        /// </summary>
        /// <param name="levelLength">The Y length of the level.</param>
        private void GenerateLevel(float levelLength)
        {
            GeneratePlatforms(levelLength);
            GenerateCoins(levelLength);
            GenerateBoosts(levelLength);
        }

        /// <summary>
        /// Generate all the platforms of the current level.
        /// </summary>
        /// <param name="levelLength">The Y length of the level.</param>
        private void GeneratePlatforms(float levelLength)
        {
            // Get the range on X and the start height value
            Vector2 rangeOnX = new Vector2(mainCam.Cam.ScreenToWorldPoint(Vector2.zero).x, mainCam.Cam.ScreenToWorldPoint(new Vector2(Screen.width, 0f)).x);
            float startHeight = mainCam.Cam.ScreenToWorldPoint(Vector2.zero).y - GameData.heightOffsetBeforeAppearing;
            float yStep = 0.1f;

            // For each step, create a platform
            for (float step = 0f; step < levelLength; step += yStep)
            {
                // Get a random position and index
                Vector2 currentPosition = new Vector2(Random.Range(rangeOnX.x, rangeOnX.y), startHeight - step);
                int index = GetIndex(platforms);

                // Generate a platform
                GameObject platform = Instantiate(platforms[index].prefab, platforms[index].parent);
                platform.transform.position = currentPosition;

                // Calculate its x scale
                float scaleX = Random.Range(GameData.minPlatformSizeX, GameData.maxPlatformSizeX);

                // If it is a kill platform
                if (platform.CompareTag(GameData.KILL_PLATFORM_TAG))
                {
                    platform.GetComponent<KillPlatform>().SetUI(scaleX);
                }
                else
                {
                    // Else, get its spriteRenderer and its collider
                    SpriteRenderer platformRenderer;
                    BoxCollider2D platformCollider = platform.GetComponent<BoxCollider2D>();

                    if (platform.TryGetComponent(out Platform script))
                    {
                        platformRenderer = script.UIPart.GetComponent<SpriteRenderer>();
                    }
                    else
                    {
                        platformRenderer = platform.GetComponent<SpriteRenderer>();
                    }

                    // Calculate the scale with the ration
                    float scaledRatio = scaleX / GameData.scaleRatio;
                    // Then get the size (on x) of one texture
                    float sizePerUnitTextureX = platformRenderer.sprite.texture.width / platformRenderer.sprite.pixelsPerUnit;
                    // Get the real scale so that there is a whole number of textures
                    float realScaleOnX = scaledRatio - (scaledRatio % sizePerUnitTextureX);

                    // Scale them accordingly
                    platformRenderer.size = new Vector2(realScaleOnX, platformRenderer.size.y);
                    platformCollider.size = new Vector2(realScaleOnX, platformCollider.size.y);
                }

                // Get the step from its scale
                yStep = Random.Range(GameData.platformMinHeightOffset, GameData.platformMaxHeightOffset);
            }

            // Spawn the final platform
            GameObject finalPlat = Instantiate(finalPlatform, finalPlatformParent);
            finalPlat.transform.position = new Vector2(0f, startHeight - levelLength - yStep);

            // Set the min height's value
            minLevelHeight.Value = finalPlat.transform.position.y;
        }

        /// <summary>
        /// Generate all the coins of the current level.
        /// </summary>
        /// <param name="levelLength">The Y length of the level.</param>
        private void GenerateCoins(float levelLength)
        {
            // Get the range on X and the start height value
            Vector2 rangeOnX = new Vector2(mainCam.Cam.ScreenToWorldPoint(Vector2.zero).x, mainCam.Cam.ScreenToWorldPoint(new Vector2(Screen.width, 0f)).x);
            float startHeight = mainCam.Cam.ScreenToWorldPoint(Vector2.zero).y - GameData.heightOffsetBeforeAppearing;
            float yStep;

            // For each step, create a platform
            for (float step = 0f; step < levelLength; step += yStep)
            {
                // Get a random position and index
                Vector2 currentPosition = new Vector2(Random.Range(rangeOnX.x, rangeOnX.y), startHeight - step);
                int index = GetIndex(coins);

                // Generate a coin
                GameObject coin = Instantiate(coins[index].prefab, coins[index].parent);
                coin.transform.position = currentPosition;

                // Get the step from its scale
                yStep = Random.Range(coin.transform.lossyScale.y * 2f, coin.transform.lossyScale.y * 2f + (GameData.platformMaxHeightOffset - GameData.platformMinHeightOffset));
            }
        }

        /// <summary>
        /// Generate all the boosts of the current level.
        /// </summary>
        /// <param name="levelLength">The Y length of the level.</param>
        private void GenerateBoosts(float levelLength)
        {
            // Get the range on X and the start height value
            Vector2 rangeOnX = new Vector2(mainCam.Cam.ScreenToWorldPoint(Vector2.zero).x, mainCam.Cam.ScreenToWorldPoint(new Vector2(Screen.width, 0f)).x);
            float startHeight = mainCam.Cam.ScreenToWorldPoint(Vector2.zero).y - GameData.heightOffsetBeforeAppearing;
            float yStep = 15f;

            // For each step, create a platform
            for (float step = 0f; step < levelLength; step += yStep)
            {
                // Get a random position and index
                Vector2 currentPosition = new Vector2(Random.Range(rangeOnX.x, rangeOnX.y), startHeight - step);
                int index = GetIndex(boosts, true);

                // If there is no index, continue the loop
                if(index == -1) { continue; }

                // Generate a boost
                GameObject boost = Instantiate(boosts[index].prefab, boosts[index].parent);
                boost.transform.position = currentPosition;

                // Get the step from its scale
                yStep = Random.Range(boost.transform.lossyScale.y * 2f, boost.transform.lossyScale.y * 2f + (GameData.platformMaxHeightOffset - GameData.platformMinHeightOffset));
            }
        }

        /// <summary>
        /// Get a random index.
        /// </summary>
        private int GetIndex(SpawnableValues[] spawnableObjects, bool canReturnNull = false)
        {
            int chance = Random.Range(0, 1001);
            float maxDstToPreventNull = 25f;
            float minDst = int.MaxValue;
            int minIndex = 0;

            for (int i = 0; i < spawnableObjects.Length; i++)
            {
                float currentDst = Mathf.Abs(spawnableObjects[i].strength - chance);
                if (minDst > currentDst)
                {
                    minDst = currentDst;
                    minIndex = i;

                    if (canReturnNull && minDst > maxDstToPreventNull)
                    {
                        minIndex = -1;
                    }
                }
            }

            return minIndex;
        }
    }
}
