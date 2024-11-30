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

        private readonly Dictionary<string, StorageFile> soundFiles;

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructor for the SFXManager class.
        /// </summary>
        public SFXManager()
        {
            this.soundFiles = new Dictionary<string, StorageFile>();
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

                await this.addSoundFile("enemy_death", audioFolder);
                await this.addSoundFile("enemy_shoot", audioFolder);
                await this.addSoundFile("player_death", audioFolder);
                await this.addSoundFile("player_shoot", audioFolder);
            }
            catch (Exception exception)
            {
                throw new Exception("Error loading sound effects", exception);
            }
        }

        private async Task addSoundFile(string key, StorageFolder audioFolder)
        {
            try
            {
                var file = await audioFolder.GetFileAsync(key + ".wav");
                this.soundFiles.Add(key, file);
            }
            catch (Exception exception)
            {
                throw new Exception("Error loading sound file " + key, exception);
            }
        }

        /// <summary>
        ///     Plays a sound effect.
        /// </summary>
        /// <param name="key">Key of the sound effect to play.</param>
        public void Play(string key)
        {
            if (this.soundFiles.TryGetValue(key, out var file))
            {
                var mediaPlayer = new MediaPlayer
                {
                    Source = MediaSource.CreateFromStorageFile(file)
                };

                mediaPlayer.MediaEnded += (sender, args) => { mediaPlayer.Dispose(); };

                mediaPlayer.Play();
            }
            else
            {
                throw new ArgumentException($"Sound effect '{key}' not found.");
            }
        }

        #endregion
    }
}