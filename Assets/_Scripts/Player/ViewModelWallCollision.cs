using UnityEngine;

public class ViewModelWallCollision : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Transform _viewModel;

    [Header("Detection")]
    [SerializeField] private float _checkDistance = 0.65f;
    [SerializeField] private float _checkRadius = 0.12f;
    [SerializeField] private float _horizontalSpread = 0.25f;
    [SerializeField] private LayerMask _obstacleLayers;

    [Header("Position Settings")]
    [SerializeField] private Vector3 _normalPosition = Vector3.zero;
    [SerializeField] private Vector3 _blockedOffset =
        new Vector3(0f, -0.12f, -0.2f);

    [Header("Smoothing")]
    [SerializeField] private float _transitionSpeed = 8f;

    private Vector3 _targetPosition;

    private void LateUpdate()
    {
        CheckForObstacles();

        _viewModel.localPosition = Vector3.Lerp(
            _viewModel.localPosition,
            _targetPosition,
            _transitionSpeed * Time.deltaTime
        );
    }

    private void CheckForObstacles()
    {
        Transform cameraTransform = _playerCamera.transform;

        Vector3 center = cameraTransform.position;
        Vector3 left = center - cameraTransform.right * _horizontalSpread;
        Vector3 right = center + cameraTransform.right * _horizontalSpread;

        bool isBlocked =
            CastFromPoint(center) ||
            CastFromPoint(left) ||
            CastFromPoint(right);

        _targetPosition = isBlocked
            ? _normalPosition + _blockedOffset
            : _normalPosition;
    }

    private bool CastFromPoint(Vector3 origin)
    {
        Vector3 direction = _playerCamera.transform.forward;

        bool hitWall = Physics.SphereCast(
            origin,
            _checkRadius,
            direction,
            out RaycastHit hit,
            _checkDistance,
            _obstacleLayers,
            QueryTriggerInteraction.Ignore
        );

        Debug.DrawRay(
            origin,
            direction * _checkDistance,
            hitWall ? Color.red : Color.green
        );

        if (hitWall)
        {
            Debug.Log($"Viewmodel obstruction detected: {hit.collider.name}");
        }

        return hitWall;
    }
}