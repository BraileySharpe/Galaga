
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
    public class SfxManager
    {
        #region Data members

        private readonly Dictionary<GlobalEnums.AudioFiles, StorageFile> soundFiles;
        private readonly Dictionary<GlobalEnums.AudioFiles, MediaPlayer> activePlayers = new Dictionary<GlobalEnums.AudioFiles, MediaPlayer>();
        private readonly TaskCompletionSource<bool> preloadTaskCompletionSource = new TaskCompletionSource<bool>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructor for the SFXManager class.
        /// </summary>
        public SfxManager()
        {
            this.soundFiles = new Dictionary<GlobalEnums.AudioFiles, StorageFile>();
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

                foreach (GlobalEnums.AudioFiles file in Enum.GetValues(typeof(GlobalEnums.AudioFiles)))
                {
                    try
                    {
                        await this.addSoundFile(file, audioFolder);
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("Error loading sound effects", exception);
                    }
                }

                preloadTaskCompletionSource.SetResult(true);
            }
            catch (Exception exception)
            {
                preloadTaskCompletionSource.SetException(exception);
            }
        }

        private async Task addSoundFile(GlobalEnums.AudioFiles key, StorageFolder audioFolder)
        {
            try
            {
                var fileName = $"{key.ToString().ToLower()}.wav";
                var file = await audioFolder.GetFileAsync(fileName);
                this.soundFiles.Add(key, file);
            }
            catch (Exception exception)
            {
                throw new Exception("Error loading sound file " + key, exception);
            }
        }

        /// <summary>
        ///     Wait until all sounds are preloaded
        /// </summary>
        public async Task WaitForPreloadingAsync()
        {
            await this.preloadTaskCompletionSource.Task;
        }

        /// <summary>
        ///     Plays a sound effect.
        /// </summary>
        /// <param name="key">Key of the sound effect to play.</param>
        public void Play(GlobalEnums.AudioFiles key)
        {
            if (this.soundFiles.TryGetValue(key, out var file))
            {
                if (this.activePlayers.TryGetValue(key, out var existingPlayer))
                {
                    existingPlayer.Pause();
                    existingPlayer.Dispose();
                    this.activePlayers.Remove(key);
                }

                var mediaPlayer = new MediaPlayer
                {
                    Source = MediaSource.CreateFromStorageFile(file),
                    Volume = 0.15
                };

                mediaPlayer.MediaEnded += (sender, args) => 
                { 
                    mediaPlayer.Dispose();
                    this.activePlayers.Remove(key);
                };

                this.activePlayers.Add(key, mediaPlayer);

                mediaPlayer.Play();
            }
            else
            {
                throw new ArgumentException($"Sound effect '{key}' not found.");
            }
        }

        /// <summary>
        ///     Stops a sound effect.
        /// </summary>
        /// <param name="key">
        ///     The key of the sound effect to stop.
        /// </param>
        public void Stop(GlobalEnums.AudioFiles key)
        {
            if (this.activePlayers.TryGetValue(key, out var player))
            {
                player.Pause();
                player.Dispose();
                this.activePlayers.Remove(key);
            }
        }

        #endregion
    }
}