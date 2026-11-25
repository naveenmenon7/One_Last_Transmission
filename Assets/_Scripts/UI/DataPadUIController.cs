using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DataPadUIController : MonoBehaviour
{
    [Header("Archive System")]
    [SerializeField] private ArchiveManager _archiveManager;

    [Header("Dialogue System")]
    [SerializeField] private DialogueManager _dialogueManager;

    [Header("Main Screen")]
    [SerializeField] private GameObject _mainScreen;

    [Header("Archive Buttons")]
    [SerializeField] private Button _audioRecordingsButton;
    [SerializeField] private Button _personalMessagesButton;
    [SerializeField] private Button _transmissionsButton;

    [Header("Category Screen")]
    [SerializeField] private GameObject _categoryScreen;
    [SerializeField] private TMP_Text _categoryTitle;
    [SerializeField] private Button _backButton;

    [Header("Category Lists")]
    [SerializeField] private GameObject _recordingListPanel;
    [SerializeField] private GameObject _personalMessagesListPanel;
    [SerializeField] private GameObject _transmissionsListPanel;

    [Header("Recording Entries")]
    [SerializeField] private Button[] _recordingButtons;
    [SerializeField] private Button[] _messageButtons;
    [SerializeField] private Button[] _transmissionButtons;

    [Header("Entry Text References")]
    [SerializeField] private TMP_Text[] _recordingTitleTexts;
    [SerializeField] private TMP_Text[] _recordingDescriptionTexts;

    [SerializeField] private TMP_Text[] _messageTitleTexts;
    [SerializeField] private TMP_Text[] _messageDescriptionTexts;

    [SerializeField] private TMP_Text[] _transmissionTitleTexts;
    [SerializeField] private TMP_Text[] _transmissionDescriptionTexts;

    [Header("Recording Details")]
    [SerializeField] private GameObject _recordingDetailsPanel;
    [SerializeField] private TMP_Text _detailsTitle;
    [SerializeField] private TMP_Text _recordingMetadata;
    [SerializeField] private TMP_Text _transcriptText;
    [SerializeField] private Button _detailsBackButton;

    private GameObject _currentListPanel;
    private List<ArchiveEntry> _currentEntries;

    private ArchiveEntry _currentEntry;

    private UnityAction[] _recordingActions;
    private UnityAction[] _messageActions;
    private UnityAction[] _transmissionActions;

    private void Awake()
    {
        _audioRecordingsButton.onClick.AddListener(OpenAudioRecordings);
        _personalMessagesButton.onClick.AddListener(OpenPersonalMessages);
        _transmissionsButton.onClick.AddListener(OpenTransmissions);

        _backButton.onClick.AddListener(ReturnToMainScreen);
        _detailsBackButton.onClick.AddListener(ReturnToList);

        _recordingActions = RegisterEntryButtons(_recordingButtons);
        _messageActions = RegisterEntryButtons(_messageButtons);
        _transmissionActions = RegisterEntryButtons(_transmissionButtons);
    }

    private void Start()
    {
        _mainScreen.SetActive(true);
        _categoryScreen.SetActive(false);

        HideAllLists();
        _recordingDetailsPanel.SetActive(false);
    }

    private UnityAction[] RegisterEntryButtons(Button[] buttons)
    {
        UnityAction[] actions = new UnityAction[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;

            actions[index] = () => OpenEntryAtIndex(index);
            buttons[index].onClick.AddListener(actions[index]);
        }

        return actions;
    }

    private void OpenAudioRecordings()
    {
        OpenCategory(
            "AUDIO RECORDINGS",
            ArchiveCategory.AudioRecording,
            _recordingListPanel,
            _recordingButtons,
            _recordingTitleTexts,
            _recordingDescriptionTexts
        );
    }

    private void OpenPersonalMessages()
    {
        OpenCategory(
            "PERSONAL MESSAGES",
            ArchiveCategory.PersonalMessage,
            _personalMessagesListPanel,
            _messageButtons,
            _messageTitleTexts,
            _messageDescriptionTexts
        );
    }

    private void OpenTransmissions()
    {
        OpenCategory(
            "TRANSMISSIONS",
            ArchiveCategory.Transmission,
            _transmissionsListPanel,
            _transmissionButtons,
            _transmissionTitleTexts,
            _transmissionDescriptionTexts
        );
    }

    private void OpenCategory(
        string categoryName,
        ArchiveCategory category,
        GameObject listPanel,
        Button[] buttons,
        TMP_Text[] titleTexts,
        TMP_Text[] descriptionTexts)
    {
        _mainScreen.SetActive(false);
        _categoryScreen.SetActive(true);

        _categoryTitle.text = categoryName;

        HideAllLists();

        _currentListPanel = listPanel;
        _currentListPanel.SetActive(true);

        _recordingDetailsPanel.SetActive(false);

        _currentEntry = null;

        _currentEntries = _archiveManager.GetEntriesByCategory(category);

        RefreshCategoryEntries(
            buttons,
            titleTexts,
            descriptionTexts
        );
    }

    private void RefreshCategoryEntries(
        Button[] buttons,
        TMP_Text[] titleTexts,
        TMP_Text[] descriptionTexts)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i >= _currentEntries.Count)
            {
                buttons[i].interactable = false;
                titleTexts[i].text = "NO DATA";
                descriptionTexts[i].text = "NO ARCHIVE ENTRY";
                continue;
            }

            ArchiveEntry entry = _currentEntries[i];

            bool isUnlocked =
                _archiveManager.IsEntryUnlocked(entry.EntryID);

            buttons[i].interactable = isUnlocked;

            if (isUnlocked)
            {
                titleTexts[i].text =
                    $"{entry.EntryID} // {entry.EntryTitle}";

                descriptionTexts[i].text =
                    "RECOVERED // AVAILABLE FOR REVIEW";
            }
            else
            {
                titleTexts[i].text = "LOCKED";

                descriptionTexts[i].text =
                    "UNDISCOVERED ARCHIVE DATA";
            }
        }
    }

    private void OpenEntryAtIndex(int index)
    {
        if (_currentEntries == null ||
            index < 0 ||
            index >= _currentEntries.Count)
        {
            return;
        }

        ArchiveEntry entry = _currentEntries[index];

        if (!_archiveManager.IsEntryUnlocked(entry.EntryID))
            return;

        _currentEntry = entry;

        _currentListPanel.SetActive(false);
        _recordingDetailsPanel.SetActive(true);

        _detailsTitle.text =
            $"{entry.EntryID} // {entry.EntryTitle}";

        _recordingMetadata.text = entry.Metadata;
        _transcriptText.text = entry.DialogueText;

        // Selecting an audio recording automatically plays its dialogue.
        if (entry.Category == ArchiveCategory.AudioRecording)
        {
            if (_dialogueManager != null)
            {
                Debug.Log($"Playing recording: {entry.EntryID}");

                _dialogueManager.PlayDialogue(entry);
            }
            else
            {
                Debug.LogWarning(
                    "DataPadUIController: DialogueManager is not assigned."
                );
            }
        }
    }

    private void ReturnToList()
    {
        _recordingDetailsPanel.SetActive(false);
        _currentListPanel.SetActive(true);

        _currentEntry = null;
    }

    private void ReturnToMainScreen()
    {
        _categoryScreen.SetActive(false);
        _mainScreen.SetActive(true);

        _currentEntry = null;
    }

    private void HideAllLists()
    {
        _recordingListPanel.SetActive(false);
        _personalMessagesListPanel.SetActive(false);
        _transmissionsListPanel.SetActive(false);
    }

    private void RemoveEntryListeners(
        Button[] buttons,
        UnityAction[] actions)
    {
        if (buttons == null || actions == null)
            return;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null && i < actions.Length)
            {
                buttons[i].onClick.RemoveListener(actions[i]);
            }
        }
    }

    private void OnDestroy()
    {
        _audioRecordingsButton.onClick.RemoveListener(OpenAudioRecordings);
        _personalMessagesButton.onClick.RemoveListener(OpenPersonalMessages);
        _transmissionsButton.onClick.RemoveListener(OpenTransmissions);

        _backButton.onClick.RemoveListener(ReturnToMainScreen);
        _detailsBackButton.onClick.RemoveListener(ReturnToList);

        RemoveEntryListeners(_recordingButtons, _recordingActions);
        RemoveEntryListeners(_messageButtons, _messageActions);
        RemoveEntryListeners(_transmissionButtons, _transmissionActions);
    }
}