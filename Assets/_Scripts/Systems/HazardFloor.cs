using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HazardFloor : MonoBehaviour
{
    [Header("Hazard Settings")]
    [SerializeField] private bool _isHazard = true;
    [SerializeField] private float _countdownDuration = 5f;

    [Header("References")]
    [SerializeField] private HazardManager _hazardManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!_isHazard)
            return;

        if (!other.CompareTag("Player"))
            return;

            Debug.Log("ENTERED HAZARD: " + gameObject.name);

        if (_hazardManager == null)
        {
            Debug.LogWarning(
                $"HazardFloor: HazardManager not assigned on {gameObject.name}."
            );
            return;
        }

        _hazardManager.EnterHazardFloor(
            this,
            _countdownDuration
        );
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (_hazardManager == null)
            return;

        _hazardManager.ExitHazardFloor(this);
    }
}