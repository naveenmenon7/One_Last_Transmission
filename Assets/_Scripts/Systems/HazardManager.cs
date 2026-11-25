using System.Collections;
using TMPro;
using UnityEngine;

public class HazardManager : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private Transform _player;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private FirstPersonController _firstPersonController;
    [SerializeField] private Transform _spawnPoint;

    [Header("UI References")]
    [SerializeField] private GameObject _countdownPanel;
    [SerializeField] private TMP_Text _warningText;
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private CanvasGroup _countdownCanvasGroup;

    [Header("Fade Settings")]
    [SerializeField] private float _fadeDuration = 0.3f;

    [Header("Hazard Settings")]
    [SerializeField] private float _defaultCountdownDuration = 5f;

    private Coroutine _hazardCoroutine;
    private bool _isHazardActive;

    public bool IsHazardActive => _isHazardActive;

    private void Awake()
    {
        _countdownPanel.SetActive(false);
    }

    public void TriggerHazard(float countdownDuration)
    {
        if (_isHazardActive)
            return;

        if (_hazardCoroutine != null)
        {
            StopCoroutine(_hazardCoroutine);
        }

        _isHazardActive = true;

        _hazardCoroutine = StartCoroutine(
            HazardCountdown(countdownDuration)
        );
    }

    public void CancelHazard()
    {
        if (!_isHazardActive)
            return;

        _isHazardActive = false;

        if (_hazardCoroutine != null)
        {
            StopCoroutine(_hazardCoroutine);
        }

        _hazardCoroutine = StartCoroutine(FadeOutAndHide());
    }

    private IEnumerator HazardCountdown(float duration)
    {
        _countdownPanel.SetActive(true);

        yield return FadePanel(1f);

        float remainingTime = duration;

        while (remainingTime > 0f)
        {
            _countdownText.text =
                Mathf.CeilToInt(remainingTime).ToString("00");

            remainingTime -= Time.deltaTime;

            yield return null;
        }

        _countdownText.text = "00";

        RespawnPlayer();

        yield return FadePanel(0f);

        _countdownPanel.SetActive(false);
        _isHazardActive = false;
        _hazardCoroutine = null;
    }

    private void RespawnPlayer()
    {
        _characterController.enabled = false;

        _player.SetPositionAndRotation(
            _spawnPoint.position,
            _spawnPoint.rotation
        );

        _characterController.enabled = true;

        if (_firstPersonController != null)
        {
            _firstPersonController.ResetMovement();
        }
    }

    private IEnumerator FadePanel(float targetAlpha)
    {
        float startAlpha = _countdownCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / _fadeDuration;

            _countdownCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                progress
            );

            yield return null;
        }

        _countdownCanvasGroup.alpha = targetAlpha;
    }

    private IEnumerator FadeOutAndHide()
    {
        yield return FadePanel(0f);

        _countdownPanel.SetActive(false);
        _hazardCoroutine = null;
    }
}