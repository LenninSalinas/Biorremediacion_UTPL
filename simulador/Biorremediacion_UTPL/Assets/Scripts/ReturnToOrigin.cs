using System.Collections;
using UnityEngine;

/// <summary>
/// Sends the object back to its original place after it has been left alone for a while:
/// quickly if it was dropped inside the tool zone, slowly if it was dropped outside.
/// Driven by Interactable events, so nothing runs while the object is being held.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Interactable))]
public class ReturnToOrigin : MonoBehaviour
{
    [Tooltip("If empty, the ToolZone in the scene is found automatically.")]
    [SerializeField] private ToolZone zone;
    [SerializeField, Min(0f)] private float delayInsideZone = 2f;
    [SerializeField, Min(0f)] private float delayOutsideZone = 30f;
    [Tooltip("Duration of the return animation. 0 = instant.")]
    [SerializeField, Min(0f)] private float returnDuration = 0.5f;

    private Interactable interactable;
    private Coroutine returnRoutine;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        if (zone == null) zone = FindAnyObjectByType<ToolZone>();
    }

    private void OnEnable()
    {
        interactable.Held += HandleHeld;
        interactable.Released += HandleReleased;
    }

    private void OnDisable()
    {
        interactable.Held -= HandleHeld;
        interactable.Released -= HandleReleased;
        CancelReturn();
    }

    private void HandleHeld(Interactable _) => CancelReturn();

    private void HandleReleased(Interactable _)
    {
        CancelReturn();
        returnRoutine = StartCoroutine(WaitAndReturn());
    }

    private void CancelReturn()
    {
        if (returnRoutine == null) return;
        StopCoroutine(returnRoutine);
        returnRoutine = null;
    }

    private IEnumerator WaitAndReturn()
    {
        bool isInside = IsInsideZone();
        float elapsed = 0f;

        // Time.deltaTime is 0 while the info panel is open (timeScale = 0), so the countdown pauses.
        while (elapsed < GetDelay(isInside))
        {
            yield return null;

            bool nowInside = IsInsideZone();
            if (nowInside != isInside)
            {
                isInside = nowInside;   // zone changed: restart the countdown
                elapsed = 0f;
            }
            else
            {
                elapsed += Time.deltaTime;
            }
        }

        interactable.PrepareForReturn();
        Transform t = transform;
        Vector3 startPosition = t.localPosition;
        Quaternion startRotation = t.localRotation;

        for (float time = 0f; time < returnDuration; time += Time.deltaTime)
        {
            float progress = Mathf.SmoothStep(0f, 1f, time / returnDuration);
            t.localPosition = Vector3.Lerp(startPosition, interactable.OriginalLocalPosition, progress);
            t.localRotation = Quaternion.Slerp(startRotation, interactable.OriginalLocalRotation, progress);
            yield return null;
        }

        interactable.ResetToOriginalPose();
        returnRoutine = null;
    }

    private float GetDelay(bool isInside) => isInside ? delayInsideZone : delayOutsideZone;

    private bool IsInsideZone() => zone != null && zone.Contains(transform.position);
}