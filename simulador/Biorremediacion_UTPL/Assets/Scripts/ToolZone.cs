using UnityEngine;

/// <summary>Volume (the "square") where the tools are stored.</summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public class ToolZone : MonoBehaviour
{
    private BoxCollider box;
    private BoxCollider Box => box != null ? box : (box = GetComponent<BoxCollider>());

    // Called by the editor when the component is added.
    private void Reset()
    {
        Box.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void Awake() => Box.isTrigger = true;

    /// <summary>Exact test, even if the box is rotated or scaled.</summary>
    public bool Contains(Vector3 worldPoint)
    {
        Vector3 local = transform.InverseTransformPoint(worldPoint) - Box.center;
        Vector3 halfSize = Box.size * 0.5f;
        return Mathf.Abs(local.x) <= halfSize.x
            && Mathf.Abs(local.y) <= halfSize.y
            && Mathf.Abs(local.z) <= halfSize.z;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.25f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Box.center, Box.size);
    }
}