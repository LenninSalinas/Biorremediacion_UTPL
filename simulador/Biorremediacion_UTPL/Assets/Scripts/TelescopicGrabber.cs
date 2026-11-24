using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Passive tool that holds a water probe at its tip.
/// It does not read input: PlayerInteractor decides when to attach or release.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Interactable))]
public class TelescopicGrabber : MonoBehaviour
{
    [Header("Tip")]
    [Tooltip("Empty object placed at the claws of the tip.")]
    [FormerlySerializedAs("puntaAgarrador")]
    [SerializeField] private Transform tip;

    [Tooltip("Maximum distance at which the tip can reach a probe.")]
    [FormerlySerializedAs("distanciaDeteccionSonda")]
    [SerializeField, Min(0f)] private float probeReach = 4f;

    [Tooltip("Probe scale at the tip relative to its original scale (previously a hard-coded 6).")]
    [SerializeField, Min(0.01f)] private float probeScaleAtTip = 6f;

    private Interactable attachedProbe;

    public float ProbeReach => probeReach;
    public bool HasProbe => attachedProbe != null;

    public bool CanAttach(Interactable candidate) =>
        !HasProbe
        && tip != null
        && candidate != null
        && candidate.Type == InteractableType.Probe
        && !candidate.IsHeld;

    public bool AttachProbe(Interactable probe)
    {
        if (!CanAttach(probe)) return false;

        // As a trigger the probe pushes nothing, but WaterProbe still detects the water.
        probe.Hold(tip, Interactable.ColliderMode.Trigger, probeScaleAtTip);
        attachedProbe = probe;
        return true;
    }

    public void ReleaseProbe()
    {
        if (!HasProbe) return;
        attachedProbe.Release();
        attachedProbe = null;
    }
}