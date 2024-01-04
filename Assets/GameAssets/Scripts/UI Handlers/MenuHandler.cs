using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using RocknFall.Bases.SO;
using System.Collections;

namespace RocknFall.UIHandlers
{
    public class MenuHandler : MonoBehaviour
    {
        private OpenMenu menu;

        [Header("Triggers")]
        [SerializeField] TriggerFunction triggerPlay;
        [SerializeField] TriggerFunction triggerCredits;
        [SerializeField] TriggerFunction triggerSettings;
        [SerializeField] TriggerFunction triggerShop;

        [Header("Menus")]
        [SerializeField] Animator playAnim;
        [SerializeField] Animator creditsAnim;
        [SerializeField] Animator settingsAnim;
        [SerializeField] Animator shopAnim;

        [Header("Theme values")]
        [SerializeField] Button[] themeButtons;
        [SerializeField] Text[] themeTexts;

        [Header("Error Texts")]
        [SerializeField] GameObject errorTextPrefab;
        [SerializeField] Transform errorTextsParent;
        [SerializeField] int sizeOfPool;
        [SerializeField] float fadingTime;
        [SerializeField] float fadingUpSpeed;
        private Text[] errorTexts;

        [Header("Purchase")]
        [SerializeField] Button[] themePricesButtons;
        [SerializeField] Text[] themePricesTexts;
        [SerializeField] RawImage[] coinPricesImages;

        [Header("Coins")]
        [SerializeField] IntValue coinCount;
        [SerializeField] Text coinsText;
        private IntValue.onValueChange coinsChangeFunction;

        [Header("Sounds")]
        [SerializeField] MessageSO soundMessage;

        #region Initialization
        private void Awake()
        {
            // Set that we own the main theme
            PlayerPrefs.SetInt("byeifjsoehfzybizhenjphuiofczie)îfjpooncjp^siq)àoeijuijcks0", 1);
        }
        private void Start()
        {
            // Subscribe to trigger events
            triggerPlay.OnTriggerEnterFunction += OpenPlayMenu;
            triggerCredits.OnTriggerEnterFunction += CreditsMenu;
            triggerSettings.OnTriggerEnterFunction += SettingsMenu;
            triggerShop.OnTriggerEnterFunction += ShopMenu;

            triggerPlay.OnTriggerExitFunction += GoBackToMenu;
            triggerCredits.OnTriggerExitFunction += GoBackToMenu;
            triggerSettings.OnTriggerExitFunction += GoBackToMenu;
            triggerShop.OnTriggerExitFunction += GoBackToMenu;

            // Set the right menus activated
            creditsAnim.SetBool("Activated", false);
            settingsAnim.SetBool("Activated", false);
            playAnim.SetBool("Activated", false);
            shopAnim.SetBool("Activated", false);

            // Get the coins count and subscribe to the events whenever it changes
            coinsChangeFunction = (int coinCount) => OnCoinCountChange(coinCount);
            coinCount.OnValueChange += coinsChangeFunction;
            coinCount.Value = PlayerPrefs.GetInt("efbyzidcojixjdzhuçcdposidazàjcioseidzkcsjo", 0);

            // Initialize the pool
            InitializeErrorTextsPool();

            // Set the right price texts
            for (int i = 0; i < themePricesTexts.Length; i++)
            {
                // Get whether or not they have been purchased
                bool purchased = PlayerPrefs.GetInt("byeifjsoehfzybizhenjphuiofczie)îfjpooncjp^siq)àoeijuijcks" + i, 0) != 0;

                // Set the text accordingly
                themePricesTexts[i].text = purchased ? "Already owned" : "" + GameData.themePrices[i];

                // Enable/Disable the coin image accordingly
                coinPricesImages[i].enabled = !purchased;

                // Set the buttons' interactablily depending on that
                themePricesButtons[i].interactable = !purchased;
            }

            // Set the right theme texts
            for (int i = 0; i < themeTexts.Length; i++)
            {
                // Get whether or not they have been purchased
                bool purchased = PlayerPrefs.GetInt("byeifjsoehfzybizhenjphuiofczie)îfjpooncjp^siq)àoeijuijcks" + i, 0) != 0;

                // Set the text accordingly
                themeTexts[i].text = purchased ? "Play" : "Buy it";

                // Set the buttons' interactablily depending on that
                themeButtons[i].interactable = purchased;
            }

            // Prevent the game to be in paused when it shouldn't be
            Time.timeScale = 1f;
        }
        private void OnDestroy()
        {
            // Unsubscribe to events
            triggerPlay.OnTriggerEnterFunction -= OpenPlayMenu;
            triggerCredits.OnTriggerEnterFunction -= CreditsMenu;
            triggerSettings.OnTriggerEnterFunction -= SettingsMenu;
            triggerShop.OnTriggerEnterFunction -= ShopMenu;

            triggerPlay.OnTriggerExitFunction -= GoBackToMenu;
            triggerCredits.OnTriggerExitFunction -= GoBackToMenu;
            triggerSettings.OnTriggerExitFunction -= GoBackToMenu;
            triggerShop.OnTriggerExitFunction -= GoBackToMenu;

            coinCount.OnValueChange -= coinsChangeFunction;
        }
        #endregion

        private void ClickSound()
        {
            soundMessage.SendMessage("Clip" + ((int)ClipIndex.ButtonClicked).ToString());
        }

        /// <summary>
        /// This function makes the player purchase a theme.
        /// </summary>
        /// <param name="index">The theme index.</param>
        public void PurchaseTheme(int index)
        {
            // Check that the right index has been sent
            if(index < 0 || index >= themePricesTexts.Length) { return; }

            // Check it hasn't already been purchased
            bool purchased = PlayerPrefs.GetInt("byeifjsoehfzybizhenjphuiofczie)îfjpooncjp^siq)àoeijuijcks" + index, 0) != 0;
            if (purchased)
            {
                // Send an error message
                StartCoroutine(ErrorTextAnimation(Input.mousePosition, "You can't buy something you already own !"));
                return;
            }

            // Check if the player has the money to buy it
            if (coinCount.Value >= GameData.themePrices[index])
            {
                // Play the right sound
                soundMessage.SendMessage("Clip" + ((int)ClipIndex.BuyTheme).ToString());

                // Decrease the coins count
                coinCount.Value -= GameData.themePrices[index];
                // Save the new coins count
                PlayerPrefs.SetInt("efbyzidcojixjdzhuçcdposidazàjcioseidzkcsjo", coinCount.Value);

                // Save that the theme has been purchased
                PlayerPrefs.SetInt("byeifjsoehfzybizhenjphuiofczie)îfjpooncjp^siq)àoeijuijcks" + index, 1);

                // Update the text and the image
                themePricesButtons[index].interactable = false;
                themePricesTexts[index].text = "Already owned";
                coinPricesImages[index].enabled = false;

                themeTexts[index].text = "Play";
                themeButtons[index].interactable = true;

                // Send that the player lost a certain amount of money
                StartCoroutine(ErrorTextAnimation(Input.mousePosition, "-" + GameData.themePrices[index]));
            }
            else
            {
                // Send an error message
                StartCoroutine(ErrorTextAnimation(Input.mousePosition, "You don't have enough money !"));
            }
        }

        #region Game Menus
        /// <summary>
        /// Open the play menu, so that the player can choose its theme before playing.
        /// </summary>
        public void OpenPlayMenu()
        {
            // Play the click sound
            ClickSound();

            // Set in which menu we are
            menu = menu == OpenMenu.ChooseTheme ? OpenMenu.Default : OpenMenu.ChooseTheme;

            // Set the animations
            creditsAnim.SetBool("Activated", false);
            settingsAnim.SetBool("Activated", false);
            shopAnim.SetBool("Activated", false);

            // Enable the right menu
            playAnim.SetBool("Activated", menu == OpenMenu.ChooseTheme);
            playAnim.SetBool("Exit", menu != OpenMenu.ChooseTheme);
        }

        /// <summary>
        /// Start the game.
        /// </summary>
        public void Play(int themeIndex)
        {
            // Save the theme index the player chose
            PlayerPrefs.SetInt("rghebfizhibfhqzuofijihouejcshpdqiofhuixjdshfbcuxhj", themeIndex);
            // Start the game
            StartCoroutine(PlayLevel());
        }

        private IEnumerator PlayLevel()
        {
            // Play the click sound
            ClickSound();
            // Wait a little bit so that the player can hear the click sound
            yield return new WaitForSeconds(GameData.timeToWaitForClickSound);
            // Start the level
            SceneManager.LoadScene((int)SceneIndex.PLAY);
        }

        /// <summary>
        /// Show the credits.
        /// </summary>
        public void CreditsMenu()
        {
            // Play the click sound
            ClickSound();

            // Set in which menu we are
            menu = menu == OpenMenu.Credits ? OpenMenu.Default : OpenMenu.Credits;

            // Set the animations
            creditsAnim.SetBool("Activated", menu == OpenMenu.Credits);
            creditsAnim.SetBool("Exit", menu != OpenMenu.Credits);

            settingsAnim.SetBool("Activated", false);
            playAnim.SetBool("Activated", false);
            shopAnim.SetBool("Activated", false);
        }

        /// <summary>
        /// Show the settings menu.
        /// </summary>
        public void SettingsMenu()
        {
            // Play the click sound
            ClickSound();

            // Set in which menu we are
            menu = menu == OpenMenu.Settings ? OpenMenu.Default : OpenMenu.Settings;

            // Set the animations
            settingsAnim.SetBool("Activated", menu == OpenMenu.Settings);
            settingsAnim.SetBool("Exit", menu != OpenMenu.Settings);

            creditsAnim.SetBool("Activated", false);
            playAnim.SetBool("Activated", false);
            shopAnim.SetBool("Activated", false);
        }

        /// <summary>
        /// Show the shop menu.
        /// </summary>
        public void ShopMenu()
        {
            // Play the click sound
            ClickSound();

            // Set in which menu we are
            menu = menu == OpenMenu.Shop ? OpenMenu.Default : OpenMenu.Shop;

            // Set the animations
            creditsAnim.SetBool("Activated", false);
            settingsAnim.SetBool("Activated", false);
            playAnim.SetBool("Activated", false);

            // Enable the right menu
            shopAnim.SetBool("Activated", menu == OpenMenu.Shop);
            shopAnim.SetBool("Exit", menu != OpenMenu.Shop);
        }

        /// <summary>
        /// Go back to the main menu.
        /// </summary>
        public void GoBackToMenu()
        {
            // Play the click sound
            ClickSound();

            switch (menu)
            {
                case OpenMenu.Credits:
                    creditsAnim.SetBool("Exit", true);
                    creditsAnim.SetBool("Activated", false);
                    break;
                case OpenMenu.Settings:
                    settingsAnim.SetBool("Exit", true);
                    settingsAnim.SetBool("Activated", false);
                    break;
                case OpenMenu.Shop:
                    shopAnim.SetBool("Exit", true);
                    shopAnim.SetBool("Activated", false);
                    break;
                case OpenMenu.ChooseTheme:
                    playAnim.SetBool("Exit", true);
                    playAnim.SetBool("Activated", false);
                    break;
            }

            // Set in which menu we are
            menu = OpenMenu.Default;
        }
        #endregion

        #region Redirecting Functions
        /// <summary>
        /// Redirect the player on youtube.
        /// </summary>
        public void GoOnYoutube()
        {
            // Play the click sound
            ClickSound();

            Application.OpenURL("https://www.youtube.com/channel/UC6FMYBRrE8xw7GVCs9qEFYA");
        }

        /// <summary>
        /// Redirect the player on discord.
        /// </summary>
        public void GoOnDiscord()
        {
            // Play the click sound
            ClickSound();

            Application.OpenURL("https://discord.gg/NNmk9zn");
        }

        /// <summary>
        /// Redirect the player on instagram.
        /// </summary>
        public void GoOnInstagram()
        {
            // Play the click sound
            ClickSound();

            Application.OpenURL("https://www.instagram.com/mcdown_forcommunity/");
        }

        /// <summary>
        /// Redirect the player on twitter.
        /// </summary>
        public void GoOnTwitter()
        {
            // Play the click sound
            ClickSound();

            Application.OpenURL("https://twitter.com/McDown6");
        }
        #endregion

        #region Update UI
        private void OnCoinCountChange(int count)
        {
            coinsText.text = count.ToString();
        }

        private void InitializeErrorTextsPool()
        {
            // Initialize the array size
            errorTexts = new Text[sizeOfPool];

            for (int i = 0; i < sizeOfPool; i++)
            {
                // Create it
                GameObject currentText = Instantiate(errorTextPrefab, errorTextsParent);
                currentText.transform.localPosition = Vector3.zero;
                // Assign it to the array
                errorTexts[i] = currentText.GetComponent<Text>();
                // Disable it
                errorTexts[i].enabled = false;
            }
        }

        IEnumerator ErrorTextAnimation(Vector3 position, string message)
        {
            // Get an error text
            Text ErrorText = GetAvailableErrorText();

            // Enable the error text with the right message and place it at the right position
            ErrorText.text = message;
            ErrorText.transform.position = position;
            ErrorText.enabled = true;

            // Get the time to wait
            WaitForSeconds timeToWait = new WaitForSeconds(Time.fixedDeltaTime);

            // Loop during the fading process
            for (float count = 0f; count < fadingTime; count += Time.fixedDeltaTime)
            {
                // Make the error text fade up
                ErrorText.transform.position += Vector3.up * fadingUpSpeed * Time.fixedDeltaTime;

                // Update the alpha
                ErrorText.color = new Color(ErrorText.color.r, ErrorText.color.g, ErrorText.color.b, 1f - (count / fadingTime));

                // Wait the needed amount of time
                yield return timeToWait;
            }

            // Reset the alpha correctly
            ErrorText.color = new Color(ErrorText.color.r, ErrorText.color.g, ErrorText.color.b, 1f);

            // Disable the error text
            ErrorText.enabled = false;
        }

        private Text GetAvailableErrorText()
        {
            // For every error texts
            for (int i = 0; i < sizeOfPool; i++)
            {
                // If the current one isn't enabled, it isn't used, so return it
                if (!errorTexts[i].enabled)
                {
                    return errorTexts[i];
                }
            }

            // If we reach this code, then there wasn't enough error texts to cover every problem, so signal the developper and return a default value
            #if UNITY_EDITOR
            Debug.LogError("THERE WEREN'T ENOUGH TEXT ERROR, PLEASE INCREASE THE SIZE OF THE POOL !");
            #endif
            return errorTexts[0];
        }
        #endregion
    }

    public enum OpenMenu
    {
        Default     = 0,
        Settings    = 1,
        Credits     = 2,
        Shop        = 3,
        ChooseTheme = 4,
    }
}
