using UnityEngine;

namespace RocknFall.Bases.SO
{
    [CreateAssetMenu(fileName = "Sound List", menuName = "RocknFall/Custom/SoundList")]
    public class SoundList : ScriptableObject
    {
        public AudioClip[] musics;
        public AudioClip[] clips;
    }
}
