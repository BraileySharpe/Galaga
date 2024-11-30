using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace Galaga.Model
{
    /// <summary>
    ///     Manages sound effects for the game.
    /// </summary>
    public class SFXManager
    {
        #region Data members

        private readonly Dictionary<string, List<MediaPlayer>> soundEffectPools;
        private readonly Dictionary<string, StorageFile> soundFiles;
        private TaskCompletionSource<bool> preloadTaskCompletionSource;

        private const int InitialPoolSize = 5;

        private const double Volume = 0.25;

        #endregion

        #region Constructors


        /// <summary>
        ///     Constructor for the SFXManager class.
        /// </summary>
        public SFXManager()
        {

            soundEffectPools = new Dictionary<string, List<MediaPlayer>>();
            soundFiles = new Dictionary<string, StorageFile>();
            preloadTaskCompletionSource = new TaskCompletionSource<bool>();
            LoadSounds();
        }

        #endregion

        #region Methods

        private async void loadSounds()
        {
            try
            {
                var assetsFolder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
                var audioFolder = await assetsFolder.GetFolderAsync("Audio");


                await AddSoundEffect("enemy_death", audioFolder);
                await AddSoundEffect("enemy_shoot", audioFolder);
                await AddSoundEffect("player_death", audioFolder);
                await AddSoundEffect("player_shoot", audioFolder);

                preloadTaskCompletionSource.SetResult(true);

            }
            catch (Exception exception)
            {
                preloadTaskCompletionSource.SetException(new Exception("Error loading sound effects", exception));
            }
        }

        private async Task addSoundFile(string key, StorageFolder audioFolder)
        {
            try
            {
                StorageFile file = await audioFolder.GetFileAsync(key + ".wav");
                soundFiles.Add(key, file);

                List<MediaPlayer> pool = new List<MediaPlayer>();
                for (int i = 0; i < InitialPoolSize; i++)
                {
                    MediaPlayer player = new MediaPlayer()
                    {
                        Source = Windows.Media.Core.MediaSource.CreateFromStorageFile(file),

                        Volume = Volume
                    };

                    pool.Add(player);
                }

                soundEffectPools.Add(key, pool);
            }
            catch (Exception exception)
            {
                throw new Exception("Error loading sound file " + key, exception);
            }
        }

        /// <summary>
        ///     Wait untill all sounds are preloaded
        /// </summary>
        public async Task WaitForPreloadingAsync()
        {
            await preloadTaskCompletionSource.Task;
        }

        /// <summary>
        ///     Plays a sound effect.
        /// </summary>
        /// <param name="key">Key of the sound effect to play.</param>
        public void Play(string key)
        {

            if (soundEffectPools.TryGetValue(key, out var pool))
            {
                MediaPlayer availablePlayer = pool.Find(p => p.CurrentState == MediaPlayerState.Closed
                                                    || p.CurrentState == MediaPlayerState.Paused);

                if (availablePlayer == null)
                {
                    MediaPlayer newPlayer = this.createNewThread(key);
                    pool.Add(newPlayer);
                }

                availablePlayer?.Play();
            }
            else
            {
                throw new ArgumentException($"Sound effect not found: {key}");
            }
        }

        private MediaPlayer createNewThread(string key)
        {
            if (soundFiles.TryGetValue(key, out var file))
            {
                MediaPlayer player = new MediaPlayer()
                {
                    Source = Windows.Media.Core.MediaSource.CreateFromStorageFile(file),
                    Volume = 0.25
                };

                return player;
            }

            throw new ArgumentException($"Invalid key: {key}");
        }

        /// <summary>
        ///     Stops a sound effect.
        /// </summary>
        /// <param name="key">
        ///     The key of the sound effect to stop.
        /// </param>
        public void Stop(string key)
        {
            if (soundEffectPools.TryGetValue(key, out var pool))
            {
                foreach (var player in pool)
                {
                    if (player.CurrentState == MediaPlayerState.Playing)
                    {
                        player.Pause();
                    }
                }
            }
        }

        #endregion
    }
}