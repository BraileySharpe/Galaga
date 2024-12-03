using Galaga.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

public class HighScoreBoard
{
    private const string FilenameDataContractSerialization = "HighScores.xml";

    public async Task<List<HighScoreEntry>> GetHighScoresAsync()
    {
        try
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.TryGetItemAsync(FilenameDataContractSerialization) as StorageFile;

            if (file == null)
            {
                return new List<HighScoreEntry>();
            }

            using var inStream = await file.OpenStreamForReadAsync();
            var deserializer = new DataContractSerializer(typeof(List<HighScoreEntry>));
            return (List<HighScoreEntry>?)deserializer.ReadObject(inStream) ?? new List<HighScoreEntry>();
        }
        catch
        {
            return new List<HighScoreEntry>();
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
}
