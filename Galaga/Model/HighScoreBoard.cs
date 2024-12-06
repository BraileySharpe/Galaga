﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace Galaga.Model;

public class HighScoreBoard
{
    #region Data members

    private const string FilenameDataContractSerialization = "HighScores.xml";

    #endregion

    #region Methods

    public async Task<List<HighScoreEntry>> GetHighScoresAsync()
    {
        try
        {
            var folder = ApplicationData.Current.LocalFolder;

            if (await folder.TryGetItemAsync(FilenameDataContractSerialization) is not StorageFile file)
            {
                return [];
            }

            using var inStream = await file.OpenStreamForReadAsync();
            var deserializer = new DataContractSerializer(typeof(List<HighScoreEntry>));
            return (List<HighScoreEntry>)deserializer.ReadObject(inStream) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task SaveHighScoresAsync(List<HighScoreEntry> highScores)
    {
        try
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(FilenameDataContractSerialization,
                CreationCollisionOption.ReplaceExisting);

            using var outStream = await file.OpenStreamForWriteAsync();
            var serializer = new DataContractSerializer(typeof(List<HighScoreEntry>));
            serializer.WriteObject(outStream, highScores);
        }
        catch (Exception ex)
        {
            throw new IOException("Error saving high scores.", ex);
        }
    }

    public async Task<List<HighScoreEntry>> AddHighScoreAsync(HighScoreEntry newEntry)
    {
        var highScores = await this.GetHighScoresAsync();

        highScores.Add(newEntry);
        highScores = highScores
            .OrderByDescending(h => h.Score)
            .ThenBy(h => h.PlayerName)
            .ThenBy(h => h.LevelCompleted)
            .Take(10)
            .ToList();

        await this.SaveHighScoresAsync(highScores);

        return highScores;
    }

    public async Task<List<HighScoreEntry>> ClearHighScoreBoardAsync()
    {
        var highScores = await this.GetHighScoresAsync();
        highScores.Clear();
        await this.SaveHighScoresAsync(highScores);
        return highScores;
    }

    #endregion
}