using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using RocknFall.Bases.SO;
using System.Collections;

namespace RocknFall.UIHandlers
{
    public class GameplayMenuHandler : MonoBehaviour
    {
        [Header("Menus")]
        [SerializeField] GameObject gameplayMenu;
        [SerializeField] GameObject pauseMenu;
        [SerializeField] GameObject deathMenu;

        [Header("Buttons")]
        [SerializeField] GameObject nextLevelButton;
        [SerializeField] GameObject restartButton;

        [Header("Coins Text")]
        [SerializeField] Text coinsCountText;
        [SerializeField] Text subCoinsText;
        [SerializeField] Color baseSubCoinsColor;
        [SerializeField] Color lotSubCoinsColor;
        [SerializeField] float fadingUpSpeed;
        private int lastCoinsCount;
        private Coroutine currentCoinsCoroutine;

        [Header("Boosts Text")]
        [SerializeField] MessageSO messageBoosts;
        [SerializeField] Text BoostsText;
        private MessageSO.onNewMessage onNewMessageBoost;
        private Coroutine currentBoostsCoroutine;

        [Header("Dynamic Result Texts")]
        [SerializeField] Text pauseScreenLevelText;
        [SerializeField] Text pauseScreenCoinsText;
        [SerializeField] Text deathScreenLevelText;
        [SerializeField] Text deathScreenCoinsText;

        [Header("Dependencies")]
        [SerializeField] BoolValue isPlayerDead;
        private BoolValue.onValueChange playerDeadFunction;

        [SerializeField] BoolValue isLevelFinished;
        private BoolValue.onValueChange levelFinishedFunction;

        [SerializeField] IntValue level;
        [SerializeField] IntValue coinsCount;
        private int globalCoinsCount;
        private IntValue.onValueChange coinsChangeFunction;
        private bool _paused = false;

        [SerializeField] MessageSO soundMessage;

        private void Start()
        {
            // Set the right menus at the start of the game
            gameplayMenu.SetActive(true);
            pauseMenu.SetActive(false);
            deathMenu.SetActive(false);

            // Set the right values for the SO
            isLevelFinished.Value = false;

            // Set the coins text
            coinsCount.Value = 0;
            lastCoinsCount = 0;
            coinsCountText.text = coinsCount.Value.ToString();
            subCoinsText.enabled = false;
            globalCoinsCount = PlayerPrefs.GetInt("efbyzidcojixjdzhuçcdposidazàjcioseidzkcsjo", 0);

            // Disable the boost text
            onNewMessageBoost = (string message) => ModifyBoostsText(message);
            messageBoosts.OnNewMessage += onNewMessageBoost;
            BoostsText.gameObject.SetActive(false);

            // Set that the game should be running
            Time.timeScale = 1f;

            // Subscribe to events
            coinsChangeFunction = (int coins) => ModifyCoinsText(coins);
            playerDeadFunction = (bool isPlayerDead) => { if (isPlayerDead) { RenderDeathMenu(true); } };
            levelFinishedFunction = (bool isLevelFinished) => { if (isLevelFinished) { RenderDeathMenu(false); } };

            coinsCount.OnValueChange += coinsChangeFunction;
            isPlayerDead.OnValueChange += playerDeadFunction;
            isLevelFinished.OnValueChange += levelFinishedFunction;
        }
        private void OnDestroy()
        {
            // Unsubscribe to events
            coinsCount.OnValueChange -= coinsChangeFunction;
            isPlayerDead.OnValueChange -= playerDeadFunction;
            isLevelFinished.OnValueChange -= levelFinishedFunction;
            messageBoosts.OnNewMessage -= onNewMessageBoost;
        }

        #region Coins Manager
        /// <summary>
        /// This function is called whevener the coins count is modified. It renders it on screen.
        /// </summary>
        /// <param name="coins">The current coins count achieved.</param>
        private void ModifyCoinsText(int coins)
        {
            // Render the text
            coinsCountText.text = coins.ToString();

            // Animate the coin text
            int coinsDifference = coins - lastCoinsCount;
            lastCoinsCount = coins;

            if (currentCoinsCoroutine != null)
            {
                StopCoroutine(currentCoinsCoroutine);
            }
            currentCoinsCoroutine = StartCoroutine(AnimateSubCoinText(coinsDifference));

            // Save the new number of coins
            PlayerPrefs.SetInt("efbyzidcojixjdzhuçcdposidazàjcioseidzkcsjo", globalCoinsCount + coins);
        }
        private IEnumerator AnimateSubCoinText(int coinsDifference)
        {
            // Enable the text
            subCoinsText.enabled = true;
            subCoinsText.transform.localPosition = new Vector3(Random.Range(0f, 50f), -25f, 0f);
            subCoinsText.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(-10f, 0f));
            subCoinsText.color = coinsDifference >= 5 ? lotSubCoinsColor : baseSubCoinsColor;
            subCoinsText.text = "+" + coinsDifference;

            // Get the time to wait
            WaitForSeconds timeToWait = new WaitForSeconds(Time.fixedDeltaTime);
            float maxTime = 1f;

            // Loop during the fading process
            for (float count = 0f; count < maxTime; count += Time.fixedDeltaTime)
            {
                // Make the text fade up
                subCoinsText.transform.position += subCoinsText.transform.up * fadingUpSpeed * Time.fixedDeltaTime;

                // Update the alpha
                subCoinsText.color = new Color(subCoinsText.color.r, subCoinsText.color.g, subCoinsText.color.b, 1f - count);

                // Wait the needed amount of time
                yield return timeToWait;
            }

            // Reset the alpha
            subCoinsText.color = new Color(subCoinsText.color.r, subCoinsText.color.g, subCoinsText.color.b, 1f);
            subCoinsText.enabled = false;
        }
        #endregion

        #region Boosts Manager
        /// <summary>
        /// This function is called whevener the player gets a new boost. It renders the text related to it.
        /// </summary>
        /// <param name="boostText">The boost text to show.</param>
        private void ModifyBoostsText(string boostText)
        {
            if (currentBoostsCoroutine != null)
            {
                StopCoroutine(currentBoostsCoroutine);
                BoostsText.gameObject.SetActive(false);
            }
            currentBoostsCoroutine = StartCoroutine(ModifyBoostsTextCor(boostText));
        }
        private IEnumerator ModifyBoostsTextCor(string boostText)
        {
            BoostsText.gameObject.SetActive(true);
            BoostsText.text = boostText;

            yield return new WaitForSeconds(1f);

            BoostsText.gameObject.SetActive(false);
        }
        #endregion

        /// <summary>
        /// This function is called when the game is finished (so either the player has died or has reached the end of the level).
        /// </summary>
        /// <param name="isPlayerDead">Whether or not the player has died or this function is triggered because he has reached the end of the level.</param>
        public void RenderDeathMenu(bool isPlayerDead)
        {
            // Render the right menus
            deathMenu.SetActive(true);
            gameplayMenu.SetActive(false);

            // Render the right buttons
            nextLevelButton.SetActive(!isPlayerDead);
            restartButton.SetActive(isPlayerDead);

            // Set the right texts
            deathScreenLevelText.text = level.Value.ToString();
            deathScreenCoinsText.text = coinsCount.Value.ToString();
        }

        #region Level Manager
        /// <summary>
        /// Finish the level and start the new one.
        /// </summary>
        public void FinishLevel()
        {
            // Save the level count
            PlayerPrefs.SetInt("efbhusndzidjksxndisjxnjq", level.Value + 1);
            // Restart the scene with tha value saved
            Restart();
        }

        /// <summary>
        /// Pause/Unpause the game.
        /// </summary>
        public void Pause()
        {
            PlayClickSound();

            // Set whether or not the game is paused
            _paused = !_paused;
            Time.timeScale = _paused ? 0f : 1f;

            // Render the menus accordingly
            gameplayMenu.SetActive(!_paused);
            pauseMenu.SetActive(_paused);

            // Set the right texts
            if (_paused)
            {
                pauseScreenLevelText.text = level.Value.ToString();
                pauseScreenCoinsText.text = coinsCount.Value.ToString();
            }
        }

        /// <summary>
        /// Reload the current scene.
        /// </summary>
        public void Restart()
        {
            StartCoroutine(RestartLevel());
        }

        private IEnumerator RestartLevel()
        {
            // Reset the time scale in case the game as paused
            Time.timeScale = 1f;

            // Play the click sound
            PlayClickSound();

            // Wait a little bit so that the player can hear the click sound
            yield return new WaitForSeconds(GameData.timeToWaitForClickSound);
            // Restart the level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Stop the game and go back to the menu scene.
        /// </summary>
        public void GoToMenu()
        {
            StartCoroutine(ReturnToMenu());
        }

        private IEnumerator ReturnToMenu()
        {
            // Reset the time scale in case the game as paused
            Time.timeScale = 1f;

            // Play the click sound
            PlayClickSound();
            // Wait a little bit so that the player can hear the click sound
            yield return new WaitForSeconds(GameData.timeToWaitForClickSound);
            // Return to the menu
            SceneManager.LoadScene((int)SceneIndex.MENU);
        }

        /// <summary>
        /// Play the click sound.
        /// </summary>
        private void PlayClickSound()
        {
            soundMessage.SendMessage("Clip" + ((int)ClipIndex.ButtonClicked).ToString());
        }
        #endregion
    }
}
