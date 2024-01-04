using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using RocknFall.Bases.SO;

namespace RocknFall.Entities
{
    public class ParticlesGenerator : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] FloatValue fireBoostTime;
        [SerializeField] Transform target;
        private Rigidbody2D targetRb;

        [SerializeField] float movementThreshold;
        private Vector2 previousPosition;
        private bool hasTargetBeenBelowZero = false;

        [Header("Particles")]
        [SerializeField] SceneTheme sceneTheme;
        [SerializeField] SpawnableValues[] spawnableParticles;
        private GameObject[][] particles;

        private void Start()
        {
            // Initialize the right prefabs
            for (int i = 0; i < 2; i++) { spawnableParticles[i].prefab = i == 0 ? sceneTheme.mainTheme.NormalModeTheme.fireParticlesPrefab : sceneTheme.mainTheme.FireModeTheme.fireParticlesPrefab; }

            // Initialize the pool
            particles = new GameObject[spawnableParticles.Length][];
            for (int i = 0; i < spawnableParticles.Length; i++)
            {
                particles[i] = new GameObject[spawnableParticles[i].strength];

                for (int j = 0; j < particles[i].Length; j++)
                {
                    particles[i][j] = Instantiate(spawnableParticles[i].prefab, spawnableParticles[i].parent);
                    particles[i][j].SetActive(false);
                }
            }

            // Get the components
            targetRb = target.GetComponent<Rigidbody2D>();

            // Set the previous position
            previousPosition = target.position;
        }

        private void Update()
        {
            // If this is the play scene
            if (SceneManager.GetActiveScene().buildIndex == (int)SceneIndex.PLAY)
            {
                // If the player is in the "start animation" part
                if (target.position.y > 0f && !hasTargetBeenBelowZero)
                {
                    return;
                }

                // Set that the target has at least once been below zero
                hasTargetBeenBelowZero = true;
            }

            // If the target has moved enough
            if (((Vector2)target.position - previousPosition).sqrMagnitude > movementThreshold)
            {
                // Place a fire mode particle
                PlaceFireModeParticlesAt(target.position, targetRb.velocity.y);

                // Set the previous position
                previousPosition = target.position;
            }
        }

        #region Place Particles
        void PlaceFireModeParticlesAt(Vector2 position, float downVelocity)
        {
            // Get an available particle
            int index = fireBoostTime.Value > 0f ? (int)ParticlesPerIndices.FireMode : (int)ParticlesPerIndices.Fire;
            GameObject currentParticle = GetParticleAt(index);

            // Place it and enable it
            currentParticle.transform.position = position;
            currentParticle.SetActive(true);

            // Modify its particle abilities
            currentParticle.transform.localScale = new Vector3(1f, (downVelocity < 0f ? 1f : -1f), 1f);
            ParticleSystem particle = currentParticle.GetComponent<ParticleSystem>();
            particle.emission.SetBurst(0, new ParticleSystem.Burst(0f, (short)(Mathf.Abs(downVelocity / GameData.maxVelocity) * 15f)));

            // Keep track of the particle and disable it when it has finished
            StartCoroutine(DisableParticleWhenNeeded(currentParticle, particle));
        }
        #endregion

        #region Particles Handler
        GameObject GetParticleAt(int index)
        {
            for (int i = 0; i < particles[index].Length; i++)
            {
                // If the current particle is available
                if (!particles[index][i].activeSelf)
                {
                    return particles[index][i];
                }
            }

            #if UNITY_EDITOR
            Debug.LogError("THERE WAS NOT ENOUGH PARTICLES FOR 'index = " + index + "' !");
            #endif
            return particles[index][0];
        }

        IEnumerator DisableParticleWhenNeeded(GameObject particleObject, ParticleSystem particle)
        {
            yield return new WaitForSeconds(particle.main.startLifetime.constantMax);
            particleObject.SetActive(false);
        }
        #endregion
    }

    enum ParticlesPerIndices
    {
        Fire     = 0,
        FireMode = 1,
    }
}
