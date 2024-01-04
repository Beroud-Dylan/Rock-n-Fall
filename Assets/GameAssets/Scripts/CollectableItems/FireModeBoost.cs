using UnityEngine;
using RocknFall.Bases.SO;

namespace RocknFall.CollectableItems
{
    public class FireModeBoost : CollectableItem
    {
        [Header("Fire Mode Boost Values")]
        [SerializeField] MessageSO messageLauncher;
        [SerializeField] FloatValue playerBoostFireMode;
        [SerializeField] float timeDuration;
        private onCollisionWithPlayer onCollisionFunction;

        private new void Start()
        {
            base.Start();

            onCollisionFunction = () => AddFireModeTime();
            base.OnCollisionWithPlayer += onCollisionFunction;
        }
        private new void OnDestroy()
        {
            base.OnDestroy();
            base.OnCollisionWithPlayer -= onCollisionFunction;
        }

        private void AddFireModeTime()
        {
            // Add the fire mode time to the player and destroy this
            playerBoostFireMode.Value += timeDuration;
            messageLauncher.SendMessage("FIRE MODE");
            soundMessage.SendMessage("Clip" + ((int)ClipIndex.ObtentionBoost).ToString());
        }
    }
}
