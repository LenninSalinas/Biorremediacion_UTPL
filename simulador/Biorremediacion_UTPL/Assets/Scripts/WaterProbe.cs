using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Water probe: when it enters a WaterZone it sends a sample to the multiparameter
/// screen, and clears its own reading when it leaves the water.
/// </summary>
[DisallowMultipleComponent]
public class WaterProbe : MonoBehaviour
{
    [Header("Probe")]
    [SerializeField] private ProbeType probeType;

    [Header("References")]
    [FormerlySerializedAs("pantallaMultiparametro")]
    [SerializeField] private MultiparameterScreen screen;

    public ProbeType Type => probeType;

    private void Awake()
    {
        if (screen == null)
            Debug.LogWarning($"{name}: no MultiparameterScreen assigned, readings will not be shown.", this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (screen == null || !other.TryGetComponent(out WaterZone zone)) return;
        screen.ShowReading(probeType, zone.Sample(probeType));
    }

    private void OnTriggerExit(Collider other)
    {
        if (screen == null || !other.TryGetComponent(out WaterZone _)) return;
        screen.ClearReading(probeType);
    }
}