using UnityEngine;
using RocknFall.Bases.SO;

namespace RocknFall.CollectableItems
{
    public class Bubble : CollectableItem
    {
        [Header("Bubble Values")]
        [SerializeField] MessageSO messageLauncher;
        [SerializeField] BoolValue hasBubbleProtection;
        private onCollisionWithPlayer onCollisionFunction;

        private new void Start()
        {
            base.Start();

            onCollisionFunction = () => AddBubbleProtection();
            base.OnCollisionWithPlayer += onCollisionFunction;
        }
        private new void OnDestroy()
        {
            base.OnDestroy();
            base.OnCollisionWithPlayer -= onCollisionFunction;
        }

        private void AddBubbleProtection()
        {
            // Increase the number of coins and destroy this
            hasBubbleProtection.Value = true;
            messageLauncher.SendMessage("BUBBLE\nPROTECTION");
            soundMessage.SendMessage("Clip" + ((int)ClipIndex.ObtentionBoost).ToString());
        }
    }
}
