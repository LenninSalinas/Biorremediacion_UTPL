using System;
using UnityEngine;

/// <summary>
/// Identity and physical state of an object the player can pick up.
/// Centralizes how it is held and released so the hands and the telescopic grabber
/// apply the same rules, and the object always goes back to its ORIGINAL state.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(InteractableHighlight))]
public class Interactable : MonoBehaviour
{
    public enum ColliderMode { Disable, Trigger }

    private const string GripPointName = "grip_point";

    [SerializeField] private InteractableType type;
    [Tooltip("Extra local offset while held (e.g. the old 'Offset Oxigeno' of the O2 probe).")]
    [SerializeField] private Vector3 holdOffset;

    public event Action<Interactable> Held;
    public event Action<Interactable> Released;

    public InteractableType Type => type;
    public bool IsHeld { get; private set; }
    public InteractableHighlight Highlight { get; private set; }
    public Vector3 OriginalLocalPosition => originalLocalPosition;
    public Quaternion OriginalLocalRotation => originalLocalRotation;

    // Captured only once and never overwritten, so repeated cycles cannot accumulate deformation.
    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;

    private Rigidbody body;
    private bool originalIsKinematic;
    private bool originalUseGravity;

    private Collider[] colliders;
    private bool[] originalColliderEnabled;
    private bool[] originalColliderIsTrigger;

    private Transform gripPoint;

    private void Awake()
    {
        Highlight = GetComponent<InteractableHighlight>();
        body = GetComponent<Rigidbody>();
        gripPoint = transform.Find(GripPointName);
        CaptureOriginalState();
    }

    /// <summary>Attaches the object to an anchor (a hand or the grabber tip).</summary>
    public void Hold(Transform anchor, ColliderMode colliderMode, float scaleMultiplier = 1f)
    {
        if (IsHeld || anchor == null) return;

        FreezePhysics();
        ApplyColliderMode(colliderMode);

        Transform t = transform;
        t.SetParent(anchor, false);
        t.localScale = originalLocalScale * scaleMultiplier;
        // Same formula as the previous code, so already tuned grip points keep working.
        t.localRotation = gripPoint != null ? Quaternion.Inverse(gripPoint.localRotation) : Quaternion.identity;
        t.localPosition = (gripPoint != null ? -gripPoint.localPosition : Vector3.zero) + holdOffset;

        IsHeld = true;
        Held?.Invoke(this);
    }

    /// <summary>Drops the object where it is, with its original size and physics.</summary>
    public void Release()
    {
        if (!IsHeld) return;

        Transform t = transform;
        t.SetParent(originalParent, true);
        t.localScale = originalLocalScale;
        RestoreColliders();
        RestorePhysics();

        IsHeld = false;
        Released?.Invoke(this);
    }

    /// <summary>Freezes physics so the object can be moved by code (return animation).</summary>
    public void PrepareForReturn()
    {
        if (!IsHeld) FreezePhysics();
    }

    /// <summary>Puts the object back exactly as it was when the scene started.</summary>
    public void ResetToOriginalPose()
    {
        if (IsHeld) return;

        FreezePhysics();
        Transform t = transform;
        t.SetParent(originalParent, false);
        t.localPosition = originalLocalPosition;
        t.localRotation = originalLocalRotation;
        t.localScale = originalLocalScale;
        RestoreColliders();
        RestorePhysics();
    }

    private void CaptureOriginalState()
    {
        Transform t = transform;
        originalParent = t.parent;
        originalLocalPosition = t.localPosition;
        originalLocalRotation = t.localRotation;
        originalLocalScale = t.localScale;

        colliders = GetComponentsInChildren<Collider>(true);
        originalColliderEnabled = new bool[colliders.Length];
        originalColliderIsTrigger = new bool[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            originalColliderEnabled[i] = colliders[i].enabled;
            originalColliderIsTrigger[i] = colliders[i].isTrigger;
        }

        if (body != null)
        {
            originalIsKinematic = body.isKinematic;
            originalUseGravity = body.useGravity;
        }
    }

    private void FreezePhysics()
    {
        if (body == null) return;
        if (!body.isKinematic)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
        body.isKinematic = true;
        body.useGravity = false;
    }

    private void RestorePhysics()
    {
        if (body == null) return;
        body.isKinematic = originalIsKinematic;
        body.useGravity = originalUseGravity;
    }

    private void ApplyColliderMode(ColliderMode mode)
    {
        foreach (Collider col in colliders)
        {
            if (col == null) continue;
            if (mode == ColliderMode.Disable) col.enabled = false;
            else col.isTrigger = true;
        }
    }

    private void RestoreColliders()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == null) continue;
            colliders[i].isTrigger = originalColliderIsTrigger[i];
            colliders[i].enabled = originalColliderEnabled[i];
        }
    }
}