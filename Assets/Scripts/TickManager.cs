using UnityEngine;
using System;

public class TickManager : MonoBehaviour
{
    public static TickManager Instance;

    public float tickInterval = 1f;
    public float tickSpeedMultiplier = 1f;

    public event Action OnTick;

    private float _timer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        tickInterval = 0.01f; // Ticks every 0.1 seconds
    }

    private void Update()
    {
        _timer += Time.deltaTime * tickSpeedMultiplier;
        if (_timer >= tickInterval)
        {
            _timer -= tickInterval;
            OnTick?.Invoke();
        }
    }
}
