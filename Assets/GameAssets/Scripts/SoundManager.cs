using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using RocknFall.Bases.SO;

namespace RocknFall
{
    public class SoundManager : MonoBehaviour
    {
        [Header("UI For Menu")]
        [SerializeField] Slider musicSlider;
        [SerializeField] Slider soundSlider;

        [Header("Values")]
        [SerializeField] int numberOfAudioSources;
        private AudioSource[][] audioSources;
        private float musicVolume;
        private float soundVolume;

        [Header("Dependencies")]
        [SerializeField] SoundList dataBase;
        [SerializeField] MessageSO soundPlayMessage;
        private MessageSO.onNewMessage newMessageFunction;

        private void Start()
        {
            // Load the values
            musicVolume = PlayerPrefs.GetFloat("hubdbhdcizjdxjjncnkqoji", 1f);
            soundVolume = PlayerPrefs.GetFloat("eéfrdezdcazqeqdzeqsAZDSQD", 1f);

            // Initialize the audio sources
            audioSources = new AudioSource[2][];

            for (int i = 0; i < 2; i++)
            {
                audioSources[i] = new AudioSource[numberOfAudioSources];
                string startName = i == 1 ? "MusicMaker_" : "SoundMaker_";

                for (int j = 0; j < numberOfAudioSources; j++)
                {
                    GameObject audioMaker = new GameObject(startName + i);
                    audioMaker.transform.SetParent(transform);
                    audioSources[i][j] = audioMaker.AddComponent<AudioSource>();

                    // If this is for the music
                    if(i == 1)
                    {
                        audioSources[i][j].volume = musicVolume;
                        audioSources[i][j].loop = true;
                    }
                    // Else, if it's for sounds
                    else
                    {
                        audioSources[i][j].volume = soundVolume;
                        audioSources[i][j].loop = false;
                    }

                    // Deactivate the gameObject
                    audioMaker.gameObject.SetActive(false);
                }
            }

            // If this is the menu scene
            if (SceneManager.GetActiveScene().buildIndex == (int)SceneIndex.MENU)
            {
                // Update the slider values
                musicSlider.value = musicVolume;
                soundSlider.value = soundVolume;
            }

            // Subscribe to events
            newMessageFunction = (string message) => ReceiveMessage(message);
            soundPlayMessage.OnNewMessage += newMessageFunction;
        }
        private void OnDestroy()
        {
            // Unsubscribe to events
            soundPlayMessage.OnNewMessage -= newMessageFunction;
        }

        /// <summary>
        /// This function is called whenever a sound message is received. It plays the right sound
        /// </summary>
        /// <param name="soundMessage">The sound message given.</param>
        private void ReceiveMessage(string soundMessage)
        {
            if (soundMessage.StartsWith("Clip"))
            {
                int index = int.Parse(soundMessage.Remove(0, 4));
                PlaySoundOnce(index);
            }
            else if (soundMessage.StartsWith("Music"))
            {
                int index = int.Parse(soundMessage.Remove(0, 5));
                PlayMusic(index);
            }
            else
            {
                #if UNITY_EDITOR
                Debug.LogError("THE MESSAGE GIVEN WAS WRONG : '" + soundMessage + "' !");
                #endif
            }
        }

        /// <summary>
        /// Whenever the volume of the sounds is modified, it will modify the volume of the playing sounds and save it.
        /// </summary>
        public void OnSoundValueChange()
        {
            // Set the right volume and save the value
            soundVolume = soundSlider.value;
            PlayerPrefs.SetFloat("eéfrdezdcazqeqdzeqsAZDSQD", soundVolume);

            // Reset the volumes of every audio sources
            for (int i = 0; i < numberOfAudioSources; i++)
            {
                audioSources[0][i].volume = soundVolume;
            }
        }
        /// <summary>
        /// Whenever the volume of the music is modified, it will modify the volume of the playing musics and save it.
        /// </summary>
        public void OnMusicValueChange()
        {
            // Set the right volume and save the value
            musicVolume = musicSlider.value;
            PlayerPrefs.SetFloat("hubdbhdcizjdxjjncnkqoji", musicVolume);

            // Reset the volumes of every audio sources
            for (int i = 0; i < numberOfAudioSources; i++)
            {
                audioSources[1][i].volume = musicVolume;
            }
        }

        public void PlaySoundOnce(int index)
        {
            for (int i = 0; i < numberOfAudioSources; i++)
            {
                AudioSource source = audioSources[0][i];

                // If one audio maker is available, use it
                if (!source.gameObject.activeSelf)
                {
                    source.gameObject.SetActive(true);
                    source.clip = dataBase.clips[index];
                    source.Play();

                    StartCoroutine(DisableLater(source));
                    break;
                }
            }
        }

        IEnumerator DisableLater(AudioSource source)
        {
            // Since sometimes, we can't detect when a clip has finished to play, use a length that will never be too long
            yield return new WaitUntil(() => !source.isPlaying);

            // Disable the sound source
            source.gameObject.SetActive(false);
        }

        public void PlayMusic(int index)
        {
            for (int i = 0; i < numberOfAudioSources; i++)
            {
                AudioSource source = audioSources[1][i];

                // If one audio maker is available, use it
                if (!source.gameObject.activeSelf)
                {
                    source.gameObject.SetActive(true);
                    source.clip = dataBase.musics[index];
                    source.Play();
                    break;
                }
            }
        }
    }

    public enum ClipIndex
    {
        BubbleExplode    = 0,
        ButtonClicked    = 1,
        ObtentionCoin    = 2,
        BoostClouds      = 3,
        BouncePlatform   = 4,
        PlatformBreak    = 5,
        ObtentionBoost   = 6,
        NormalBounceBack = 7,
        PlayerDeath      = 8,
        BuyTheme         = 9
    }
}
