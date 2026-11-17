
using System.Collections.Generic;
using UnityEngine;

public class ArchiveManager : MonoBehaviour
{
    [Header("Save System")]
    [SerializeField] private ArchiveSaveManager _saveManager;

    [Header("Archive Entries")]
    [SerializeField] private ArchiveEntry[] _archiveEntries;

    private readonly HashSet<string> _unlockedEntryIDs = new();

    public IReadOnlyCollection<string> UnlockedEntryIDs => _unlockedEntryIDs;

    private void Awake()
    {
        if (_archiveEntries == null || _archiveEntries.Length == 0)
        {
            Debug.LogWarning("Archive Manager: No archive entries assigned.");
            return;
        }

        ValidateArchiveEntries();
        LoadUnlockedEntries();
    }

    public bool UnlockEntry(string discoveryID)
    {
        if (string.IsNullOrWhiteSpace(discoveryID))
        {
            Debug.LogWarning("Archive Manager: Invalid discovery ID.");
            return false;
        }

        foreach (ArchiveEntry entry in _archiveEntries)
        {
            if (entry == null || entry.DiscoveryID != discoveryID)
                continue;

            bool wasUnlocked = _unlockedEntryIDs.Add(entry.EntryID);

            if (wasUnlocked)
            {
                Debug.Log($"Archive unlocked: {entry.EntryID}");
                SaveUnlockedEntries();
            }

            return wasUnlocked;
        }

        Debug.LogWarning($"Archive Manager: No entry found for discovery ID: {discoveryID}");
        return false;
    }

    public bool IsEntryUnlocked(string entryID)
    {
        return _unlockedEntryIDs.Contains(entryID);
    }

    public ArchiveEntry GetEntry(string entryID)
    {
        foreach (ArchiveEntry entry in _archiveEntries)
        {
            if (entry != null && entry.EntryID == entryID)
                return entry;
        }

        return null;
    }

    private void ValidateArchiveEntries()
    {
        HashSet<string> registeredIDs = new();

        foreach (ArchiveEntry entry in _archiveEntries)
        {
            if (entry == null)
            {
                Debug.LogWarning("Archive Manager: An empty entry reference was found.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(entry.EntryID))
            {
                Debug.LogWarning($"Archive Manager: Entry '{entry.name}' has no Entry ID.");
                continue;
            }

            if (!registeredIDs.Add(entry.EntryID))
            {
                Debug.LogWarning($"Archive Manager: Duplicate Entry ID found: {entry.EntryID}");
            }
        }
    }

    [ContextMenu("TEST/Unlock REC_001")]
    private void TestUnlockEntry()
    {
        UnlockEntry("DISCOVERY_REC_001");
    }

    public List<ArchiveEntry> GetEntriesByCategory(ArchiveCategory category)
    {
        List<ArchiveEntry> matchingEntries = new();

        foreach (ArchiveEntry entry in _archiveEntries)
        {
            if (entry == null)
                continue;

            if (entry.Category == category)
            {
                matchingEntries.Add(entry);
            }
        }

        return matchingEntries;
    }

    private void LoadUnlockedEntries()
    {
        if (_saveManager == null)
        {
            Debug.LogWarning("Archive Manager: Save Manager is not assigned.");
            return;
        }

        ArchiveSaveData saveData = _saveManager.LoadArchive();

        foreach (string entryID in saveData.UnlockedEntryIDs)
        {
            if (GetEntry(entryID) != null)
            {
                _unlockedEntryIDs.Add(entryID);
            }
            else
            {
                Debug.LogWarning(
                    $"Archive Manager: Saved entry not found: {entryID}"
                );
            }
        }

        Debug.Log($"Restored {_unlockedEntryIDs.Count} archive entries.");
    }

    private void SaveUnlockedEntries()
    {
        if (_saveManager == null)
            return;

        ArchiveSaveData saveData = new ArchiveSaveData();

        foreach (string entryID in _unlockedEntryIDs)
        {
            saveData.UnlockedEntryIDs.Add(entryID);
        }

        _saveManager.SaveArchive(saveData);
    }
}
