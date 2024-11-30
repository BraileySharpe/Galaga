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

        private const int InitialPoolSize = 5;

        private const double Volume = 0.20;

        private readonly Dictionary<string, List<MediaPlayer>> soundEffectPools;
        private readonly Dictionary<string, StorageFile> soundFiles;
        private readonly TaskCompletionSource<bool> preloadTaskCompletionSource;

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructor for the SFXManager class.
        /// </summary>
        public SFXManager()
        {
            this.soundEffectPools = new Dictionary<string, List<MediaPlayer>>();
            this.soundFiles = new Dictionary<string, StorageFile>();
            this.preloadTaskCompletionSource = new TaskCompletionSource<bool>();
            this.loadSounds();
        }

        #endregion

        #region Methods

        private async void loadSounds()
        {
            try
            {
                var assetsFolder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
                var audioFolder = await assetsFolder.GetFolderAsync("Audio");

                await this.addSoundEffect("enemy_death", audioFolder);
                await this.addSoundEffect("enemy_shoot", audioFolder);
                await this.addSoundEffect("player_death", audioFolder);
                await this.addSoundEffect("player_shoot", audioFolder);
                await this.addSoundEffect("gameover_lose", audioFolder);
                await this.addSoundEffect("gameover_win", audioFolder);

                this.preloadTaskCompletionSource.SetResult(true);
            }
            catch (Exception exception)
            {
                this.preloadTaskCompletionSource.SetException(new Exception("Error loading sound effects", exception));
            }
        }

        private async Task addSoundEffect(string key, StorageFolder audioFolder)
        {
            try
            {
                var file = await audioFolder.GetFileAsync(key + ".wav");
                this.soundFiles.Add(key, file);

                var pool = new List<MediaPlayer>();
                for (var i = 0; i < InitialPoolSize; i++)
                {
                    var player = new MediaPlayer
                    {
                        Source = MediaSource.CreateFromStorageFile(file),

                        Volume = Volume
                    };

                    pool.Add(player);
                }

                this.soundEffectPools.Add(key, pool);
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
            await this.preloadTaskCompletionSource.Task;
        }

        /// <summary>
        ///     Plays a sound effect.
        /// </summary>
        /// <param name="key">Key of the sound effect to play.</param>
        public void Play(string key)
        {
            if (this.soundEffectPools.TryGetValue(key, out var pool))
            {
                var availablePlayer = pool.Find(p =>
                    p.PlaybackSession.PlaybackState == MediaPlaybackState.None ||
                    p.PlaybackSession.PlaybackState == MediaPlaybackState.Paused);

                if (availablePlayer == null)
                {
                    var newPlayer = this.createNewThread(key);
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
            if (this.soundFiles.TryGetValue(key, out var file))
            {
                var player = new MediaPlayer
                {
                    Source = MediaSource.CreateFromStorageFile(file),
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
            if (this.soundEffectPools.TryGetValue(key, out var pool))
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