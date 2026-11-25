using UnityEngine;

public class ArchiveDiscovery : MonoBehaviour, IInteractable
{
    [Header("Discovery Settings")]
    [SerializeField] private ArchiveManager _archiveManager;
    [SerializeField] private string _discoveryID;
    [SerializeField] private string _interactionPrompt = "E  —  INVESTIGATE";

    [Header("Dialogue")]
    [SerializeField] private DialogueManager _dialogueManager;

    public void Interact()
    {
        if (_archiveManager == null)
        {
            Debug.LogWarning(
                $"Archive Discovery: No ArchiveManager assigned on {gameObject.name}."
            );
            return;
        }

        bool unlocked = _archiveManager.UnlockEntry(_discoveryID);

        if (unlocked)
        {
            Debug.Log($"Discovered archive entry: {_discoveryID}");

            ArchiveEntry entry =
                _archiveManager.GetEntryByDiscoveryID(_discoveryID);

            Debug.Log($"Archive Entry Found: {entry != null}");

            Debug.Log($"Dialogue Manager Assigned: {_dialogueManager != null}");

            if (entry != null && _dialogueManager != null)
            {
                Debug.Log("Starting dialogue...");
                _dialogueManager.PlayDialogue(entry);
            }
            else
            {
                Debug.LogWarning(
                    $"Archive Discovery: Missing ArchiveEntry or DialogueManager on {gameObject.name}."
                );
            }
        }
        else
        {
            Debug.Log($"Archive entry already discovered or unavailable: {_discoveryID}");
        }
    }

    public string GetInteractionPrompt()
    {
        return _interactionPrompt;
    }
}