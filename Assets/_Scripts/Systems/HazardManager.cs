using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private float _respawnMovementDelay = 1.5f;

    private Coroutine _hazardCoroutine;
    private bool _isHazardActive;
    public bool IsHazardActive => _isHazardActive;

    private HashSet<HazardFloor> _activeHazardFloors =
    new HashSet<HazardFloor>();

    private Coroutine _exitCheckCoroutine;

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
        Debug.Log("CancelHazard called. Active: " + _isHazardActive);
        
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
        _isHazardActive = true;

        _countdownPanel.SetActive(true);

        _countdownText.text = Mathf.CeilToInt(duration).ToString("00");

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
        _firstPersonController.SetMovementLocked(true);

        _characterController.enabled = false;

        _player.SetPositionAndRotation(
            _spawnPoint.position,
            _spawnPoint.rotation
        );

        _characterController.enabled = true;

        _firstPersonController.ResetMovement();

        StartCoroutine(UnlockPlayerMovement());
    }

    private IEnumerator UnlockPlayerMovement()
    {
        yield return new WaitForSeconds(_respawnMovementDelay);

        _firstPersonController.SetMovementLocked(false);
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

    public void EnterHazardFloor(
    HazardFloor hazardFloor,
    float countdownDuration)
    {
        _activeHazardFloors.Add(hazardFloor);

        // Cancel a pending exit check if we entered another hazard tile.
        if (_exitCheckCoroutine != null)
        {
            StopCoroutine(_exitCheckCoroutine);
            _exitCheckCoroutine = null;
        }

        // Start countdown only when entering the first hazard tile.
        if (!_isHazardActive)
        {
            _hazardCoroutine = StartCoroutine(
                HazardCountdown(countdownDuration)
            );
        }
    }

    public void ExitHazardFloor(HazardFloor hazardFloor)
    {
        _activeHazardFloors.Remove(hazardFloor);

        Debug.Log(
            $"Exited: {hazardFloor.name} | Remaining hazard floors: {_activeHazardFloors.Count}"
        );

        if (_activeHazardFloors.Count == 0)
        {
            if (_exitCheckCoroutine != null)
                StopCoroutine(_exitCheckCoroutine);

            _exitCheckCoroutine = StartCoroutine(
                CheckForHazardExit()
            );
        }
    }

    private IEnumerator CheckForHazardExit()
    {
        yield return new WaitForSeconds(0.1f);

        _exitCheckCoroutine = null;

        Debug.Log(
            "Hazard exit check. Remaining floors: " +
            _activeHazardFloors.Count
        );

        if (_activeHazardFloors.Count == 0)
        {
            Debug.Log("No hazard floors occupied. Cancelling countdown.");

            CancelHazard();
        }
    }
}