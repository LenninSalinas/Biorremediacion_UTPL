using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary>
/// Body of water with base quality levels. Each sample adds a small random
/// variation so repeated measurements look realistic.
/// </summary>
[DisallowMultipleComponent]
public class WaterZone : MonoBehaviour
{
    [Header("Base levels of this body of water")]
    [Tooltip("7 = neutral, below 7 = acidic, above 7 = alkaline.")]
    [FormerlySerializedAs("basePH")]
    [SerializeField] private float basePh = 7.5f;

    [Tooltip("Dissolved oxygen in mg/L.")]
    [FormerlySerializedAs("baseO2")]
    [SerializeField, Min(0f)] private float baseDissolvedOxygen = 8.2f;

    [Tooltip("Electrical conductivity in µS/cm.")]
    [FormerlySerializedAs("baseConductividad")]
    [SerializeField, Min(0f)] private float baseConductivity = 300f;

    [Header("Realism")]
    [Tooltip("Maximum random variation per sample (e.g. 0.1 makes a pH of 7.5 read between 7.4 and 7.6).")]
    [FormerlySerializedAs("rangoVariacion")]
    [SerializeField, Min(0f)] private float variationRange = 0.1f;

    /// <summary>Takes a new sample of the given parameter, including random variation.</summary>
    public float Sample(ProbeType type) =>
        GetBaseValue(type) + Random.Range(-variationRange, variationRange);

    private float GetBaseValue(ProbeType type) => type switch
    {
        ProbeType.Ph => basePh,
        ProbeType.DissolvedOxygen => baseDissolvedOxygen,
        ProbeType.Conductivity => baseConductivity,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown probe type.")
    };
}