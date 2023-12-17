using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AudioSystem
{
    public class TrackInfo
    {
        [Header("All information related to the track")]
        public string Name = string.Empty;
        public AudioMixerGroup Group = null;
        public IEnumerator TrackFader = null;
    }

    public class AudioPoolItem
    {
        [Header("All information related to item in the pool")]
        public GameObject GameObject = null;
        public Transform Transform = null;
        public AudioSource AudioSource = null;
        public float Unimportance = float.MaxValue;
        public bool isPlaying = false;
        public IEnumerator  Coroutine = null;
        public ulong ID = 0;
    }

    public class AudioHandler : MonoBehaviour
    {
        private static AudioHandler _instance = null;
        public static AudioHandler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = (AudioHandler)FindAnyObjectByType(typeof(AudioHandler));
                return _instance;
            }
        }
        //Assign in Inspector
        [SerializeField] private AudioMixer _mixer = null;
        [SerializeField] int _maxSounds  = 10;

        // Private variables
        Dictionary<string, TrackInfo> _tracks = new Dictionary<string, TrackInfo>();
        List<AudioPoolItem> _pool = new List<AudioPoolItem>();
        Dictionary<ulong, AudioPoolItem> _activePool = new Dictionary<ulong, AudioPoolItem>();
        ulong _idGiver = 0;
        Transform _listenerPos = null;
        

        private void Awake()
        {
            // If creating multiple scenes where audio needs to keep playing
            // Uncomment this out
            //DontDestroyOnLoad(gameObject);
            if (!_mixer) return;
            AudioMixerGroup[] groups = _mixer.FindMatchingGroups(string.Empty);

            foreach (AudioMixerGroup group in groups)
            {
                TrackInfo trackInfo     = new TrackInfo();
                trackInfo.Name          = group.name;
                trackInfo.Group         = group;
                trackInfo.TrackFader    = null;
                _tracks[group.name]     = trackInfo;
            }

            //Create pool
            for (int i = 0; i < _maxSounds; i++)
            {
                GameObject audioHolder = new GameObject("Pool Item");
                AudioSource audioSource = audioHolder.AddComponent<AudioSource>();
                audioHolder.transform.parent = transform;

                // Create and configure new pool item
                AudioPoolItem poolItem = new AudioPoolItem();
                poolItem.GameObject = audioHolder;
                poolItem.AudioSource = audioSource;
                poolItem.Transform = audioHolder.transform;
                poolItem.isPlaying = false;
                audioHolder.SetActive(false);
                _pool.Add(poolItem);
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded (Scene scene, LoadSceneMode loadMode)
        {
            //Returns the first AudioListener transform in the scene
            _listenerPos = FindObjectOfType<AudioListener>().transform;
        }


        public float GetTrackVolume(string track)
        {
            TrackInfo trackInfo;
            if (_tracks.TryGetValue(track, out trackInfo))
            {
                float volume;
                _mixer.GetFloat(track, out volume);
                return volume;
            }
            return float.MinValue;
        }

        public AudioMixerGroup GetAudioGroupGromTrackName(string name)
        {
            TrackInfo trackInfo;

            if (_tracks.TryGetValue(name, out trackInfo))
            {
                return trackInfo.Group;
            }

            return null;
        }

        public void SetTrackVolume(string track, float volume, float fadeTime = 0.0f)
        {
            if (!_mixer) return;
            TrackInfo trackInfo;

            if (_tracks.TryGetValue(track, out trackInfo))
            {
                // Stop existing coroutines
                if (trackInfo.TrackFader != null)
                {
                    StopCoroutine(trackInfo.TrackFader);
                }

                if (fadeTime == 0.0f)
                {
                    _mixer.SetFloat(track, volume);
                }
                else
                {
                    trackInfo.TrackFader = SetTrackVolumeInternal(track, volume, fadeTime);
                    StartCoroutine(trackInfo.TrackFader);
                }
            }
        }

        // Internal call only
        protected IEnumerator SetTrackVolumeInternal(string track, float volume, float fadeTime)
        {
            float startVolume = 0.0f;
            float timer = 0.0f;
            _mixer.GetFloat(track, out startVolume);

            while (timer < fadeTime)
            {
                timer += Time.unscaledDeltaTime;
                _mixer.SetFloat(track, Mathf.Lerp(startVolume, volume, timer / fadeTime));
                yield return null;
            }

            // Remove floating point inaccuracy from Lerp
            _mixer.SetFloat(track, volume);
        }

        // Internal call only
        protected ulong ConfigurePoolObject(int poolIndex, string track, AudioClip clip, Vector3 position, float volume, float spatialBlend, float unimportance)
        {
            // If poolIndex is out of range, return 0 and abort request
            if (poolIndex < 0 || poolIndex >= _pool.Count) return 0;

            // Get pool item
            AudioPoolItem poolItem = _pool[poolIndex];

            // Generate ID to identify the track
            _idGiver++;

            AudioSource source = poolItem.AudioSource;
            source.clip = clip;
            source.volume = volume;
            source.spatialBlend = spatialBlend;

            //Assign to requested audio group/track
            source.outputAudioMixerGroup = _tracks[track].Group;

            // Position source at requested position
            source.transform.position = position;

            poolItem.isPlaying = true;
            poolItem.Unimportance = unimportance;
            poolItem.ID = _idGiver;
            poolItem.GameObject.SetActive(true);

            source.Play();

            poolItem.Coroutine = StopSoundDelay(_idGiver, source.clip.length);
            StartCoroutine(poolItem.Coroutine);

            //Add this to the pool of sounds
            _activePool[_idGiver] = poolItem;

            // Return the id to the caller
            return _idGiver;
        }

        protected IEnumerator StopSoundDelay (ulong id, float duration)
        {
            yield return new WaitForSeconds(duration);
            AudioPoolItem activeSound;

            if (_activePool.TryGetValue (id, out activeSound))
            {
                activeSound.AudioSource.Stop();
                activeSound.AudioSource.clip = null;
                activeSound.GameObject.SetActive(false);
                _activePool.Remove (id);

                // Free up the pool
                activeSound.isPlaying = false;
            }

        }

        public void StopSoundWithoutDelay (ulong id)
        {
            AudioPoolItem activeSound;

            if (_activePool.TryGetValue(id, out activeSound))
            {
                // Stop the coroutine that is waiting to stop the sound
                StopCoroutine(activeSound.Coroutine);

                // Clear the sound manually
                activeSound.AudioSource.Stop();
                activeSound.AudioSource.clip = null;
                activeSound.GameObject.SetActive(false);
                _activePool.Remove(id);

                // Free up the pool
                activeSound.isPlaying = false;
            }
        }
        public ulong PlayOneShotSound (string track, AudioClip clip, Vector3 position, float volume, float spatialBlend, int priority = 128)
        {
            // Does nothing if track or clip is null or track volume is 0
            if (!_tracks.ContainsKey(track) || clip == null || volume.Equals(0.0f)) return 0;

            // Replace the highest unimportance first
            float unimportance = (_listenerPos.position - position).sqrMagnitude / Mathf.Max(1, priority);

            int leastImportantIndex = -1;
            float leastImportanceValue = float.MaxValue;

            // Find available audio source
            for (int i = 0; i < _pool.Count; i++)
            {
                AudioPoolItem poolItem = _pool[i];

                // If source is available
                if (!poolItem.isPlaying)
                {
                    return ConfigurePoolObject(i, track, clip, position, volume, spatialBlend, unimportance);
                } else
                {
                    // We have a pool item thhat is less important than the one we are going to play

                    if (poolItem.Unimportance > leastImportanceValue)
                    {
                        // Candidate to be replaced
                        leastImportanceValue = poolItem.Unimportance;
                        leastImportantIndex = i;
                    }

                }
            }

            // If all audiosources are playing,
            // check if existing request are less important than current request
            if (leastImportanceValue > unimportance)
            {
                return ConfigurePoolObject(leastImportantIndex, track, clip, position, volume, spatialBlend, unimportance);
            }

            // Could not be played due to high unimportance of the request
            return 0;
        }

        public IEnumerator PlayOneShotDelay (string track, AudioClip clip, Vector3 position, float volume, float spatialBlend, float duration, int priority = 128)
        {
            yield return new WaitForSeconds (duration);

            PlayOneShotSound(track, clip, position, volume, spatialBlend, priority);
        }

    }
}
