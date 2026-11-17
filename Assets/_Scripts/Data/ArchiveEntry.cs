
using UnityEngine;

public enum ArchiveCategory
{
    AudioRecording,
    PersonalMessage,
    Transmission
}

[CreateAssetMenu(
    fileName = "NewArchiveEntry",
    menuName = "One Last Transmission/Archive Entry"
)]
public class ArchiveEntry : ScriptableObject
{
    [Header("Identification")]
    [SerializeField] private string _entryID;
    [SerializeField] private string _entryTitle;
    [SerializeField] private ArchiveCategory _category;

    [Header("Discovery")]
    [SerializeField] private string _discoveryID;

    [Header("Content")]
    [TextArea(3, 8)]
    [SerializeField] private string _metadata;

    [TextArea(5, 15)]
    [SerializeField] private string _dialogueText;

    [Header("Audio")]
    [SerializeField] private AudioClip _voiceClip;
    public string EntryID => _entryID;
    public string EntryTitle => _entryTitle;
    public ArchiveCategory Category => _category;
    public string DiscoveryID => _discoveryID;
    public string Metadata => _metadata;
    public string DialogueText => _dialogueText;
    public AudioClip VoiceClip => _voiceClip;
    
}
