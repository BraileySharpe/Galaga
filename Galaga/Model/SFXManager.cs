
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Storage;


namespace Galaga.Model
{
    /// <summary>
    ///     Manages sound effects for the game.
    /// </summary>
    public class SFXManager
    {
        private readonly Dictionary<string, MediaPlayer> soundEffets;

        /// <summary>
        ///     Constructor for the SFXManager class.
        /// </summary>
        public SFXManager()
        {
            soundEffets = new Dictionary<string, MediaPlayer>();
            LoadSounds();
        }

        private async void LoadSounds()
        {
            try
            {
                StorageFolder assetsFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                StorageFolder audioFolder = await assetsFolder.GetFolderAsync("Audio");

                await AddSoundEffect("enemy_death", audioFolder);
                await AddSoundEffect("enemy_shoot", audioFolder);
                await AddSoundEffect("player_death", audioFolder);
                await AddSoundEffect("player_shoot", audioFolder);
            }
            catch (Exception exception)
            {
                throw new Exception("Error loading sound effects", exception);
            }
        }

        private async Task AddSoundEffect(string key, StorageFolder audioFolder)
        {
            try
            {
                StorageFile file = await audioFolder.GetFileAsync(key + ".wav");
                MediaPlayer player = new MediaPlayer()
                {
                    Source = Windows.Media.Core.MediaSource.CreateFromStorageFile(file)
                };

                soundEffets.Add(key, player);
            }
            catch (Exception exception)
            {
                throw new Exception("Error loading sound effect " + key, exception);
            }

        }

        /// <summary>
        ///     Plays a sound effect.
        /// </summary>
        /// <param name="key">
        ///     Key of the sound effect to play.
        /// </param>
        public void Play(string key)
        {
            if (soundEffets.ContainsKey(key))
            {
                soundEffets[key].Play();
            }
        }

        /// <summary>
        ///     Stops a sound effect.
        /// </summary>
        /// <param name="key">
        ///     The key of the sound effect to stop.
        /// </param>
        public void Stop(string key)
        {
            if (soundEffets.TryGetValue(key, out var player))
            {
                player.Pause();
            }
        }
    }
}
