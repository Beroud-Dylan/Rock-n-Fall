using UnityEngine;

namespace RocknFall.Bases.Interfaces
{
    public interface IKillable
    {
        /// <summary>
        /// This fonction will be triggered anytime the entity dies.
        /// </summary>
        /// <param name="impactPoint">The impact point it there is any.</param>
        /// <param name="time">The time to wait before the entity really dies.</param>
        /// /// <param name="canPlaySoundOnDeath">Does the entity have the right to play a sound on death ?</param>
        public void Die(Vector2 impactPoint, float time = 0f, bool canPlaySoundOnDeath = true);
    }
}
