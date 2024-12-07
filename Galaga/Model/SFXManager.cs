using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace Galaga.Model;

/// <summary>
///     Manages sound effects for the game.
/// </summary>
public class SfxManager
{
    #region Data members

    private const string AssetsFolder = "Assets";
    private const string AudioFolder = "Audio";
    private const string SoundEffectExtension = ".wav";
    private const double Volume = 0.13;

    private readonly Dictionary<GlobalEnums.AudioFiles, StorageFile> soundFiles;
    private readonly Dictionary<GlobalEnums.AudioFiles, MediaPlayer> activePlayers = new();

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
            var assetsFolder = await Package.Current.InstalledLocation.GetFolderAsync(AssetsFolder);
            var audioFolder = await assetsFolder.GetFolderAsync(AudioFolder);

            foreach (GlobalEnums.AudioFiles file in Enum.GetValues(typeof(GlobalEnums.AudioFiles)))
            {
                try
                {
                    await this.addSoundFile(file, audioFolder);
                }
                catch (Exception exception)
                {
                    throw new Exception("Error adding sound effect", exception);
                }
            }
        }
        catch (Exception exception)
        {
            throw new Exception($"Error loading sound effects {exception}");
        }
    }

    private async Task addSoundFile(GlobalEnums.AudioFiles key, StorageFolder audioFolder)
    {
        try
        {
            var fileName = $"{key.ToString().ToLower()}{SoundEffectExtension}";
            var file = await audioFolder.GetFileAsync(fileName);
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
    public void Play(GlobalEnums.AudioFiles key)
    {
        if (!this.soundFiles.TryGetValue(key, out var file))
        {
            throw new ArgumentException($"Sound effect '{key}' not found.");
        }

        if (this.activePlayers.TryGetValue(key, out var existingPlayer))
        {
            if (existingPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                var mediaPlayer = new MediaPlayer
                {
                    Source = MediaSource.CreateFromStorageFile(file),
                    Volume = Volume
                };

                this.activePlayers[key] = mediaPlayer;
                mediaPlayer.Play();
            }
            else
            {
                existingPlayer.Play();
            }
        }
        else
        {
            var mediaPlayer = new MediaPlayer
            {
                Source = MediaSource.CreateFromStorageFile(file),
                Volume = Volume
            };

            this.activePlayers.Add(key, mediaPlayer);
            mediaPlayer.Play();
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