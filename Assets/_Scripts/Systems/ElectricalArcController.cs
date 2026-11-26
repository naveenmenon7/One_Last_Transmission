using System.Collections;
using UnityEngine;

public class ElectricalArcController : MonoBehaviour
{
    [Header("Arc References")]
    [SerializeField] private ElectricalArc[] _arcs;

    [Header("Discharge Timing")]
    [SerializeField] private float _minInterval = 1f;
    [SerializeField] private float _maxInterval = 3f;

    [Header("Discharge Duration")]
    [SerializeField] private float _minDuration = 0.08f;
    [SerializeField] private float _maxDuration = 0.2f;

    [Header("Flash Light")]
    [SerializeField] private Light _flashLight;

    private void Start()
    {
        SetArcsVisible(false);
        StartCoroutine(RandomDischarge());
    }

    private IEnumerator RandomDischarge()
    {
        while (true)
        {
            float waitTime = Random.Range(
                _minInterval,
                _maxInterval
            );

            yield return new WaitForSeconds(waitTime);

            foreach (ElectricalArc arc in _arcs)
            {
                arc.RegenerateArc();
            }

            SetArcsVisible(true);

            float duration = Random.Range(
                _minDuration,
                _maxDuration
            );

            yield return new WaitForSeconds(duration);

            SetArcsVisible(false);
        }
    }

    private void SetArcsVisible(bool visible)
    {
        foreach (ElectricalArc arc in _arcs)
        {
            arc.GetComponent<LineRenderer>().enabled = visible;
        }

        if (_flashLight != null)
        {
            _flashLight.enabled = visible;
        }
    }
}