
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private TMP_Text _interactionPrompt;
    [SerializeField] private CanvasGroup _promptCanvasGroup;

    [Header("Interaction Settings")]
    [SerializeField] private float _interactionRange = 3f;
    [SerializeField] private float _detectionRadius = 0.15f;
    [SerializeField] private LayerMask _interactionLayer;

    [Header("Prompt Animation")]
    [SerializeField] private float _fadeSpeed = 5f;

    private IInteractable _currentInteractable;

    private void Update()
    {
        DetectInteractable();
        HandleInteractionInput();
        UpdatePromptFade();
    }

    private void DetectInteractable()
    {
        _currentInteractable = null;

        Ray ray = _playerCamera.ViewportPointToRay(
        new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.SphereCast(
            ray,
            _detectionRadius,
            out RaycastHit hit,
            _interactionRange,
            _interactionLayer,
            QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                _currentInteractable = interactable;
                _interactionPrompt.text =
                    _currentInteractable.GetInteractionPrompt();
            }
        }
    }

    private void HandleInteractionInput()
    {
        if (_currentInteractable == null)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            _currentInteractable.Interact();
        }
    }

    private void UpdatePromptFade()
    {
        float targetAlpha = _currentInteractable != null ? 1f : 0f;

        _promptCanvasGroup.alpha = Mathf.MoveTowards(
            _promptCanvasGroup.alpha,
            targetAlpha,
            _fadeSpeed * Time.deltaTime
        );
    }
}
