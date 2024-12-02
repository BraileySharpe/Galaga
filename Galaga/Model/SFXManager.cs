using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
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

        private const int InitialPoolSize = 10;

        private const double Volume = 0.18;

        private readonly Dictionary<string, List<MediaPlayer>> soundEffectPools;
        private readonly Dictionary<string, StorageFile> soundFiles;
        private readonly TaskCompletionSource<bool> preloadTaskCompletionSource;

        //****TESTING****//
        private readonly Stopwatch stopwatch;
        private static string logFilePath;
        //****TESTING****//

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructor for the SFXManager class.
        /// </summary>
        public SfxManager()
        {
            this.initilizeLogPathing();
            Debug.WriteLine("SFXManager created");
            this.stopwatch = Stopwatch.StartNew();
            Debug.WriteLine("Stopwatch started");
            this.soundEffectPools = new Dictionary<string, List<MediaPlayer>>();
            Debug.WriteLine($"Sound effect pools created. Size: {this.soundEffectPools.Count} | {this.stopwatch.ElapsedMilliseconds} ms");
            this.soundFiles = new Dictionary<string, StorageFile>();
            Debug.WriteLine($"Sound files created. Size: {this.soundFiles.Count} | {this.stopwatch.ElapsedMilliseconds} ms");
            this.preloadTaskCompletionSource = new TaskCompletionSource<bool>();
            Debug.WriteLine($"Preload task completion source created | {this.stopwatch.ElapsedMilliseconds} ms");
            this.loadSounds();
            Debug.WriteLine($"Sounds loaded | {this.stopwatch.ElapsedMilliseconds} ms");
        }

        #endregion

        #region Methods

        private async void initilizeLogPathing()
        {
            try
            {
                var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                var logFile = await folder.GetFileAsync("SFXManager.txt");
                logFilePath = logFile.Path;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing log file: {ex.Message}");
            }
        }

        private async void loadSounds()
        {
            Debug.WriteLine($"Loading sounds | {this.stopwatch.ElapsedMilliseconds} ms");
            try
            {
                Debug.WriteLine($"Getting assets folder | {this.stopwatch.ElapsedMilliseconds} ms");
                var assetsFolder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
                Debug.WriteLine($"Assets folder found | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Getting audio folder | {this.stopwatch.ElapsedMilliseconds} ms");
                var audioFolder = await assetsFolder.GetFolderAsync("Audio");
                Debug.WriteLine($"Audio folder found | {this.stopwatch.ElapsedMilliseconds} ms");

                Debug.WriteLine($"Adding sound effects | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Adding enemy_death sound | {this.stopwatch.ElapsedMilliseconds} ms");
                await this.addSoundEffect("enemy_death", audioFolder);
                Debug.WriteLine($"enemy_death sound added | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Adding enemy_shoot sound | {this.stopwatch.ElapsedMilliseconds} ms");
                await this.addSoundEffect("enemy_shoot", audioFolder);
                Debug.WriteLine($"enemy_shoot sound added | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Adding player_death sound | {this.stopwatch.ElapsedMilliseconds} ms");
                await this.addSoundEffect("player_death", audioFolder);
                Debug.WriteLine($"player_death sound added | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Adding player_shoot sound | {this.stopwatch.ElapsedMilliseconds} ms");
                await this.addSoundEffect("player_shoot", audioFolder);
                Debug.WriteLine($"player_shoot sound added | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Adding gameover_lose sound | {this.stopwatch.ElapsedMilliseconds} ms");
                await this.addSoundEffect("gameover_lose", audioFolder);
                Debug.WriteLine($"gameover_lose sound added | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Adding gameover_win sound | {this.stopwatch.ElapsedMilliseconds} ms");
                await this.addSoundEffect("gameover_win", audioFolder);
                Debug.WriteLine($"gameover_win sound added | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Adding bonusenemy_sound sound | {this.stopwatch.ElapsedMilliseconds} ms");
                await this.addSoundEffect("bonusenemy_sound", audioFolder);
                Debug.WriteLine($"bonusenemy_sound sound added | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Adding powerup_activate sound | {this.stopwatch.ElapsedMilliseconds} ms");
                await this.addSoundEffect("powerup_activate", audioFolder);
                Debug.WriteLine($"powerup_activate sound added | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Adding powerup_deactivate sound | {this.stopwatch.ElapsedMilliseconds} ms");
                await this.addSoundEffect("powerup_deactivate", audioFolder);
                Debug.WriteLine($"powerup_deactivate sound added | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Adding shieldhit sound | {this.stopwatch.ElapsedMilliseconds} ms");
                await this.addSoundEffect("shieldhit", audioFolder);
                Debug.WriteLine($"shieldhit sound added | {this.stopwatch.ElapsedMilliseconds} ms");

                Debug.WriteLine($"All sound effects added | {this.stopwatch.ElapsedMilliseconds} ms");

                Debug.WriteLine($"Setting preload task completion source result | {this.stopwatch.ElapsedMilliseconds} ms");
                this.preloadTaskCompletionSource.SetResult(true);
                Debug.WriteLine($"Preload task completion source result set | {this.stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error loading sound effects | {this.stopwatch.ElapsedMilliseconds} ms");
                this.preloadTaskCompletionSource.SetException(new Exception("Error loading sound effects", exception));
            }
        }

        private async Task addSoundEffect(string key, StorageFolder audioFolder)
        {
            Debug.WriteLine($"Adding sound effect {key} | {this.stopwatch.ElapsedMilliseconds} ms");
            try
            {
                Debug.WriteLine($"Getting sound file {key}.wav | {this.stopwatch.ElapsedMilliseconds} ms");
                var file = await audioFolder.GetFileAsync(key + ".wav");
                Debug.WriteLine($"Sound file {key}.wav found | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Adding sound file {key} to sound files | {this.stopwatch.ElapsedMilliseconds} ms");
                this.soundFiles.Add(key, file);
                Debug.WriteLine($"Sound file {key} added to sound files | {this.stopwatch.ElapsedMilliseconds} ms");

                Debug.WriteLine($"Creating sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                var pool = new List<MediaPlayer>();
                Debug.WriteLine($"Sound effect pool for {key} created | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Adding {InitialPoolSize} players to sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                for (var i = 0; i < InitialPoolSize; i++)
                {
                    Debug.WriteLine($"Creating player {i} for sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                    var player = new MediaPlayer
                    {
                        Source = MediaSource.CreateFromStorageFile(file),

                        Volume = Volume
                    };
                    Debug.WriteLine($"Player {i} created for sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");

                    Debug.WriteLine($"Adding player {i} to sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                    pool.Add(player);
                    Debug.WriteLine($"Player {i} added to sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                }
                Debug.WriteLine($"All players added to sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Adding sound effect pool for {key} to sound effect pools | {this.stopwatch.ElapsedMilliseconds} ms");
                this.soundEffectPools.Add(key, pool);
                Debug.WriteLine($"Sound effect pool for {key} added to sound effect pools | {this.stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error loading sound file {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                throw new Exception("Error loading sound file " + key, exception);
            }
        }

        /// <summary>
        ///     Wait untill all sounds are preloaded
        /// </summary>
        public async Task WaitForPreloadingAsync()
        {
            Debug.WriteLine($"Waiting for preloading | {this.stopwatch.ElapsedMilliseconds} ms");
            await this.preloadTaskCompletionSource.Task;
            Debug.WriteLine($"Preloading completed | {this.stopwatch.ElapsedMilliseconds} ms");
        }

        /// <summary>
        ///     Plays a sound effect.
        /// </summary>
        /// <param name="key">Key of the sound effect to play.</param>
        public void Play(string key)
        {
            Debug.WriteLine($"Playing sound effect {key} | {this.stopwatch.ElapsedMilliseconds} ms");
            if (this.soundEffectPools.TryGetValue(key, out var pool))
            {
                Debug.WriteLine($"Sound effect pool for {key} found | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Finding available player in sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                var availablePlayer = pool.Find(p =>
                    p.PlaybackSession.PlaybackState == MediaPlaybackState.None ||
                    p.PlaybackSession.PlaybackState == MediaPlaybackState.Paused);
                Debug.WriteLine($"Available player found in sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Available player state: {availablePlayer.PlaybackSession.PlaybackState} | {this.stopwatch.ElapsedMilliseconds} ms");
                if (availablePlayer == null)
                {
                    Debug.WriteLine($"No available player found in sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                    Debug.WriteLine($"Creating new player for sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                    var newPlayer = this.createNewThread(key);
                    Debug.WriteLine($"New player created for sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                    Debug.WriteLine($"Adding new player to sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                    pool.Add(newPlayer);
                    Debug.WriteLine($"New player added to sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                }
                Debug.WriteLine($"Playing sound effect {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                availablePlayer?.Play();
                Debug.WriteLine($"Sound effect {key} played | {this.stopwatch.ElapsedMilliseconds} ms");
            }
            else
            {
                Debug.WriteLine($"Sound effect pool for {key} not found | {this.stopwatch.ElapsedMilliseconds} ms");
                throw new ArgumentException($"Sound effect not found: {key}");
            }
        }

        private MediaPlayer createNewThread(string key)
        {
            Debug.WriteLine($"Creating new player for sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
            if (this.soundFiles.TryGetValue(key, out var file))
            {
                Debug.WriteLine($"Sound file {key} found | {this.stopwatch.ElapsedMilliseconds} ms");
                Debug.WriteLine($"Creating new player for sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                var player = new MediaPlayer
                {
                    Source = MediaSource.CreateFromStorageFile(file),
                    Volume = 0.25
                };
                Debug.WriteLine($"New player created for sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");

                Debug.WriteLine($"Returning new player for sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                return player;
            }
            Debug.WriteLine($"Sound file {key} not found | {this.stopwatch.ElapsedMilliseconds} ms");
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
            Debug.WriteLine($"Stopping sound effect {key} | {this.stopwatch.ElapsedMilliseconds} ms");
            if (this.soundEffectPools.TryGetValue(key, out var pool))
            {
                Debug.WriteLine($"Sound effect pool for {key} found | {this.stopwatch.ElapsedMilliseconds} ms");
                foreach (var player in pool)
                {
                    Debug.WriteLine($"Stopping player in sound effect pool for {key} | {this.stopwatch.ElapsedMilliseconds} ms");
                    if (player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                    {
                        Debug.WriteLine($"Player in sound effect pool for {key} is playing | {this.stopwatch.ElapsedMilliseconds} ms");
                        player.Pause();
                        Debug.WriteLine($"Player in sound effect pool for {key} paused | {this.stopwatch.ElapsedMilliseconds} ms");
                    }
                }
            }
        }

        #endregion
    }
}