using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ElectricalArc : MonoBehaviour
{
    [Header("Arc Settings")]
    [SerializeField] private Vector3 _startPoint = Vector3.zero;
    [SerializeField] private Vector3 _direction = Vector3.forward;
    [SerializeField] private float _arcLength = 1.5f;
    [SerializeField] private float _jaggedness = 0.25f;
    [SerializeField] private int _segments = 6;
    [SerializeField] private float _lineWidth = 0.025f;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.useWorldSpace = false;
        _lineRenderer.positionCount = _segments;
        _lineRenderer.startWidth = _lineWidth;
        _lineRenderer.endWidth = _lineWidth;

        GenerateArc();
    }

    private void GenerateArc()
    {
        Vector3 direction = _direction.normalized;

        for (int i = 0; i < _segments; i++)
        {
            float progress = (float)i / (_segments - 1);

            Vector3 position =
                _startPoint + direction * (_arcLength * progress);

            if (i != 0 && i != _segments - 1)
            {
                position += Random.insideUnitSphere * _jaggedness;
            }

            _lineRenderer.SetPosition(i, position);
        }
    }

    public void RegenerateArc()
    {
        GenerateArc();
    }
}