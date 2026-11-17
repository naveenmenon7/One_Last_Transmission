
using System.IO;
using UnityEngine;

public class ArchiveSaveManager : MonoBehaviour
{
    private string SavePath =>
        Path.Combine(
            Application.persistentDataPath,
            "archive_save.json"
        );

    public void SaveArchive(ArchiveSaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData, true);

        try
        {
            File.WriteAllText(SavePath, json);
            Debug.Log($"Archive progress saved: {SavePath}");
        }
        catch (System.Exception exception)
        {
            Debug.LogError(
                $"Failed to save archive: {exception.Message}"
            );
        }
    }

    public ArchiveSaveData LoadArchive()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log($"No archive save found at: {SavePath}");
            return new ArchiveSaveData();
        }

        try
        {
            string json = File.ReadAllText(SavePath);

            ArchiveSaveData saveData =
                JsonUtility.FromJson<ArchiveSaveData>(json);

            Debug.Log("Archive progress loaded.");

            return saveData ?? new ArchiveSaveData();
        }
        catch (System.Exception exception)
        {
            Debug.LogError(
                $"Failed to load archive: {exception.Message}"
            );

            return new ArchiveSaveData();
        }
    }
}
