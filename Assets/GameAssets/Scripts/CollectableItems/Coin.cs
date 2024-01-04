using UnityEngine;
using RocknFall.Bases.SO;
using UnityEngine.UI;
using System.Collections;

namespace RocknFall.CollectableItems
{
    public class Coin : CollectableItem
    {
        [Header("Coin Values")]
        [SerializeField] IntValue coinsCount;
        [SerializeField] int coinValue;

        [Header("Coin Text")]
        [SerializeField] Text coinText;
        [SerializeField] float fadingUpSpeed;
        [SerializeField] float fadingSizeSpeed;

        private onCollisionWithPlayer onCollisionFunction;

        private new void Start()
        {
            base.Start();
            coinText.enabled = false;

            // Subscribe to events
            onCollisionFunction = () => AddCoins();
            base.OnCollisionWithPlayer += onCollisionFunction;
        }
        private new void OnDestroy()
        {
            base.OnDestroy();
            base.OnCollisionWithPlayer -= onCollisionFunction;
        }

        private void AddCoins()
        {
            // Increase the number of coins and destroy this
            coinsCount.Value += Mathf.RoundToInt(coinValue);
            // Play the coin sound
            soundMessage.SendMessage("Clip" + ((int)ClipIndex.ObtentionCoin).ToString());
            // Animate the coin
            StartCoroutine(AnimateCoinText());
        }

        private IEnumerator AnimateCoinText()
        {
            // Enable the text
            coinText.enabled = true;

            // Get the time to wait
            WaitForSeconds timeToWait = new WaitForSeconds(Time.fixedDeltaTime);
            float maxTime = 1f;

            // Loop during the fading process
            for (float count = 0f; count < maxTime; count += Time.fixedDeltaTime)
            {
                // Make the text fade up
                coinText.transform.position += Vector3.up * fadingUpSpeed * Time.fixedDeltaTime;

                // Make the text increase in size
                coinText.transform.localScale += Vector3.one * fadingSizeSpeed * Time.fixedDeltaTime;

                // Update the alpha
                coinText.color = new Color(coinText.color.r, coinText.color.g, coinText.color.b, 1f - count);

                // Wait the needed amount of time
                yield return timeToWait;
            }
        }
    }
}
