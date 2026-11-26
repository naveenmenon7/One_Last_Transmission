using System.Collections;
using UnityEngine;

public class RandomSparkEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem _sparkParticles;

    [Header("Random Timing")]
    [SerializeField] private float _minInterval = 1f;
    [SerializeField] private float _maxInterval = 5f;

    private void Start()
    {
        StartCoroutine(PlayRandomSparks());
    }

    private IEnumerator PlayRandomSparks()
    {
        while (true)
        {
            float waitTime = Random.Range(
                _minInterval,
                _maxInterval
            );

            yield return new WaitForSeconds(waitTime);

            _sparkParticles.Play();
        }
    }
}