using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _subtitlePanel;
    [SerializeField] private CanvasGroup _subtitleCanvasGroup;
    [SerializeField] private TMP_Text _speakerNameText;
    [SerializeField] private TMP_Text _subtitleText;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;

    [Header("Dialogue Settings")]
    [SerializeField] private float _charactersPerSecond = 35f;
    [SerializeField] private float _minimumSentenceDisplayDuration = 3f;
    [SerializeField] private float _readingWordsPerSecond = 3.5f;

    [Header("Fade Settings")]
    [SerializeField] private float _fadeInDuration = 0.5f;
    [SerializeField] private float _fadeOutDuration = 0.7f;

    private Coroutine _dialogueCoroutine;

    public void PlayDialogue(ArchiveEntry entry)
    {
        if (entry == null)
        {
            Debug.LogWarning("DialogueManager: Archive entry is null.");
            return;
        }

        if (_dialogueCoroutine != null)
        {
            StopCoroutine(_dialogueCoroutine);
        }

        _dialogueCoroutine = StartCoroutine(
            PlayDialogueRoutine(entry)
        );
    }

    private IEnumerator PlayDialogueRoutine(ArchiveEntry entry)
    {
        _subtitlePanel.SetActive(true);
        _subtitleCanvasGroup.alpha = 0f;

        _speakerNameText.text = entry.SpeakerName;
        _subtitleText.text = string.Empty;

        if (_audioSource != null && entry.VoiceClip != null)
        {
            _audioSource.Stop();
            _audioSource.clip = entry.VoiceClip;
            _audioSource.Play();
        }

        // Fade in the subtitle panel.
        yield return StartCoroutine(
            FadePanel(0f, 1f, _fadeInDuration)
        );

        string[] sentences = Regex.Split(
            entry.DialogueText.Trim(),
            @"(?<=[.!?])\s+|\r?\n+"
        );

        foreach (string sentence in sentences)
        {
            string currentSentence = sentence.Trim();

            if (string.IsNullOrWhiteSpace(currentSentence))
                continue;

            _subtitleText.text = string.Empty;

            foreach (char character in currentSentence)
            {
                _subtitleText.text += character;

                yield return new WaitForSeconds(
                    1f / _charactersPerSecond
                );
            }

            int wordCount = currentSentence.Split(
                new char[] { ' ' },
                System.StringSplitOptions.RemoveEmptyEntries
            ).Length;

            float readingDuration = Mathf.Max(
                _minimumSentenceDisplayDuration,
                wordCount / _readingWordsPerSecond
            );

            yield return new WaitForSeconds(readingDuration);
        }

        // Fade out after the final sentence.
        yield return StartCoroutine(
            FadePanel(1f, 0f, _fadeOutDuration)
        );

        if (_audioSource != null)
        {
            _audioSource.Stop();
        }

        _subtitlePanel.SetActive(false);
        _dialogueCoroutine = null;
    }

    private IEnumerator FadePanel(
        float startAlpha,
        float targetAlpha,
        float duration)
    {
        if (duration <= 0f)
        {
            _subtitleCanvasGroup.alpha = targetAlpha;
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / duration
            );

            _subtitleCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                progress
            );

            yield return null;
        }

        _subtitleCanvasGroup.alpha = targetAlpha;
    }
}