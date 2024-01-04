using UnityEngine;

namespace RocknFall.Bases.Interfaces
{
    public interface IKiller
    {
        /// <summary>
        /// This function will be triggered every time this entity kills another one.
        /// </summary>
        /// <param name="killable">The entity that is killed.</param>
        /// <param name="impactPoint">The impact point if there is any.</param>
        /// <param name="timeBeforeKillingActivates">The time to wait before the killing effect activates.</param>
        public void Kill(IKillable killable, Vector2 impactPoint, float timeBeforeKillingActivates = 0f);
    }
}
