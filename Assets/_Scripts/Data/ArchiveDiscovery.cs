
using UnityEngine;

public class ArchiveDiscovery : MonoBehaviour, IInteractable
{
    [Header("Discovery Settings")]
    [SerializeField] private ArchiveManager _archiveManager;
    [SerializeField] private string _discoveryID;
    [SerializeField] private string _interactionPrompt = "E  —  INVESTIGATE";

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
