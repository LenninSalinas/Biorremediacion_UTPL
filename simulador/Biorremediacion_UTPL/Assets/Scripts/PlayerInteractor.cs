using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Player interaction: aiming, highlighting, equipping gloves, holding and releasing tools,
/// and operating the telescopic grabber. It is the only script that reads mouse input
/// for interaction, so all the rules live in one place.
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    private const int GrabButton = 0;    // left click
    private const int ProbeButton = 1;   // right click
    private static readonly Vector3 ScreenCenter = new Vector3(0.5f, 0.5f, 0f);
    private static readonly Color DefaultCrosshairColor = Color.white;
    private static readonly Color ActiveCrosshairColor = Color.green;

    [Header("Grabbing")]
    [FormerlySerializedAs("HandPointIzquierdo")]
    [SerializeField] private GameObject leftHandPoint;
    [FormerlySerializedAs("HandPointDerecho")]
    [SerializeField] private GameObject rightHandPoint;
    [SerializeField] private Image crosshair;
    [SerializeField, Min(0f)] private float grabDistance = 5f;
    [SerializeField] private Camera playerCamera;

    [Header("Gloves")]
    [SerializeField] private GameObject gloveVisualCamera;
    [SerializeField] private Renderer leftHandRenderer;
    [SerializeField] private Renderer rightHandRenderer;
    [FormerlySerializedAs("defaultHandMat")]
    [SerializeField] private Material defaultHandMaterial;
    [SerializeField] private Material gloveMaterial;

    [Header("Information")]
    [SerializeField] private KeyCode infoKey = KeyCode.I;

    private Interactable heldLeft;
    private Interactable heldRight;
    private TelescopicGrabber equippedGrabber;
    private Interactable currentTarget;        // highlighted object under the crosshair
    private GameObject objectUnderCrosshair;   // anything under the crosshair (for the info panel)
    private bool hasLeftGlove;
    private bool hasRightGlove;
    private int raycastMask;

    private bool HasBothGloves => hasLeftGlove && hasRightGlove;

    // ───────────────────────── Lifecycle ─────────────────────────

    private void Awake()
    {
        raycastMask = ~LayerMask.GetMask("Water", "Ignore Raycast");
        if (playerCamera == null) playerCamera = Camera.main;
    }

    private void Start()
    {
        if (gloveVisualCamera != null) gloveVisualCamera.SetActive(true);
        RemoveGloves();
        LockCursor();
    }

    private void OnDisable() => SetTarget(null);

    private void Update()
    {
        if (HandleInfoPanel()) return;   // no interaction while the panel is open

        UpdateTarget();
        if (Input.GetMouseButtonDown(GrabButton)) OnGrabPressed();
        if (Input.GetMouseButtonDown(ProbeButton)) OnProbePressed();
    }

    // ───────────────────────── Aiming and highlighting ─────────────────────────

    private void UpdateTarget()
    {
        Interactable newTarget = null;
        objectUnderCrosshair = null;

        Ray ray = playerCamera.ViewportPointToRay(ScreenCenter);
        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance, raycastMask, QueryTriggerInteraction.Ignore))
        {
            objectUnderCrosshair = hit.collider.gameObject;
            Interactable candidate = hit.collider.GetComponentInParent<Interactable>();
            if (candidate != null && IsInteractable(candidate, hit.distance))
                newTarget = candidate;
        }

        SetTarget(newTarget);
        UpdateHint();
    }

    private bool IsInteractable(Interactable candidate, float distance)
    {
        if (candidate.IsHeld) return false;
        if (candidate.Type.IsGlove()) return true;
        if (!HasBothGloves) return false;

        if (candidate.Type == InteractableType.Probe)
        {
            return equippedGrabber != null
                && distance <= equippedGrabber.ProbeReach
                && equippedGrabber.CanAttach(candidate);
        }

        return GrabRules.CanHold(candidate.Type, TypeOf(heldLeft), TypeOf(heldRight));
    }

    private void SetTarget(Interactable newTarget)
    {
        if (newTarget == currentTarget) return;

        if (currentTarget != null) currentTarget.Highlight.Hide();
        currentTarget = newTarget;
        if (currentTarget != null) currentTarget.Highlight.Show();

        if (crosshair != null)
            crosshair.color = currentTarget != null ? ActiveCrosshairColor : DefaultCrosshairColor;
    }

    private static InteractableType? TypeOf(Interactable interactable) =>
        interactable != null ? interactable.Type : (InteractableType?)null;

    // ───────────────────────── Left click: hold / release ─────────────────────────

    private void OnGrabPressed()
    {
        if (currentTarget == null)
        {
            ReleaseNext();
            return;
        }

        switch (currentTarget.Type)
        {
            case InteractableType.LeftGlove:
            case InteractableType.RightGlove:
                EquipGlove(currentTarget);
                break;
            case InteractableType.Probe:
                break;   // probes attach with right click; left click must not drop the grabber
            default:
                HoldObject(currentTarget);
                break;
        }
    }

    private void HoldObject(Interactable interactable)
    {
        HandSide side = interactable.Type.GetHoldingHand();
        GameObject handPoint = side == HandSide.Left ? leftHandPoint
                             : side == HandSide.Right ? rightHandPoint
                             : null;
        if (handPoint == null) return;

        SetTarget(null);
        interactable.Hold(handPoint.transform, Interactable.ColliderMode.Disable);

        if (side == HandSide.Left)
        {
            heldLeft = interactable;
        }
        else
        {
            heldRight = interactable;
            equippedGrabber = interactable.GetComponent<TelescopicGrabber>();
        }
    }

    private void ReleaseNext()
    {
        if (heldRight != null) ReleaseRight();
        else if (heldLeft != null) ReleaseLeft();
    }

    private void ReleaseRight()
    {
        if (equippedGrabber != null)
        {
            equippedGrabber.ReleaseProbe();   // the probe never stays stuck to a dropped grabber
            equippedGrabber = null;
        }
        heldRight.Release();
        heldRight = null;
    }

    private void ReleaseLeft()
    {
        heldLeft.Release();
        heldLeft = null;
    }

    // ───────────────────────── Right click: probe ─────────────────────────

    private void OnProbePressed()
    {
        if (equippedGrabber == null) return;

        if (equippedGrabber.HasProbe)
        {
            equippedGrabber.ReleaseProbe();
        }
        else if (currentTarget != null && currentTarget.Type == InteractableType.Probe)
        {
            Interactable probe = currentTarget;
            SetTarget(null);
            equippedGrabber.AttachProbe(probe);
        }
    }

    // ───────────────────────── Gloves ─────────────────────────

    private void EquipGlove(Interactable glove)
    {
        bool isLeft = glove.Type == InteractableType.LeftGlove;
        Renderer handRenderer = isLeft ? leftHandRenderer : rightHandRenderer;
        if (handRenderer != null && gloveMaterial != null) handRenderer.sharedMaterial = gloveMaterial;

        if (isLeft) hasLeftGlove = true;
        else hasRightGlove = true;

        SetTarget(null);
        Destroy(glove.gameObject);
    }

    public void RemoveGloves()
    {
        if (defaultHandMaterial != null)
        {
            if (leftHandRenderer != null) leftHandRenderer.sharedMaterial = defaultHandMaterial;
            if (rightHandRenderer != null) rightHandRenderer.sharedMaterial = defaultHandMaterial;
        }
        hasLeftGlove = false;
        hasRightGlove = false;
    }

    // ───────────────────────── Info panel and hint ─────────────────────────

    /// <returns>True if the info panel is open (or was just opened).</returns>
    private bool HandleInfoPanel()
    {
        HUDController hud = HUDController.Instance;
        if (hud == null) return false;

        if (hud.IsPanelOpen())
        {
            if (Input.GetKeyDown(infoKey) || Input.GetKeyDown(KeyCode.Escape))
            {
                hud.CloseInfoPanel();
                SetCrosshairVisible(true);
                StartCoroutine(LockCursorAtEndOfFrame());
            }
            return true;
        }

        if (Input.GetKeyDown(infoKey))
        {
            InformationObject info = GetInfoUnderCrosshair();
            if (info != null)
            {
                hud.OpenInfoPanel(info.displayName, info.size, info.specifications);
                SetCrosshairVisible(false);
                UnlockCursor();
                return true;
            }
        }
        return false;
    }

    private void UpdateHint()
    {
        HUDController hud = HUDController.Instance;
        if (hud == null) return;

        if (GetInfoUnderCrosshair() != null) hud.UpdateHint($"[{infoKey}] Info");
        else hud.HideHint();
    }

    private InformationObject GetInfoUnderCrosshair() =>
        objectUnderCrosshair != null ? objectUnderCrosshair.GetComponentInParent<InformationObject>() : null;

    private void SetCrosshairVisible(bool isVisible)
    {
        if (crosshair != null) crosshair.gameObject.SetActive(isVisible);
    }

    // ───────────────────────── Cursor ─────────────────────────

    private static void LockCursor()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private static void UnlockCursor()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private static IEnumerator LockCursorAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        LockCursor();
    }
}