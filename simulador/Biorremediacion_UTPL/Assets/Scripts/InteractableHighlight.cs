using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tints the object's materials while the player is aiming at it.
/// Uses MaterialPropertyBlock: no material copies are created and it does not rely on
/// shader variants that URP strips from WebGL builds (such as _EMISSION).
/// </summary>
[DisallowMultipleComponent]
public class InteractableHighlight : MonoBehaviour
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor"); // URP Lit/Unlit
    private static readonly int ColorId = Shader.PropertyToID("_Color");     // legacy shaders

    [SerializeField] private Color highlightColor = new Color(0.3f, 1f, 0.3f);
    [SerializeField, Range(0f, 1f)] private float intensity = 0.6f;

    private struct MaterialSlot
    {
        public Renderer Renderer;
        public int MaterialIndex;
        public int ColorPropertyId;
        public Color HighlightedColor;
    }

    private readonly List<MaterialSlot> slots = new List<MaterialSlot>();
    private MaterialPropertyBlock propertyBlock;

    public bool IsActive { get; private set; }

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        foreach (Renderer rend in GetComponentsInChildren<Renderer>(true))
        {
            if (rend is ParticleSystemRenderer) continue;

            Material[] materials = rend.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                if (TryCreateSlot(rend, i, materials[i], out MaterialSlot slot))
                    slots.Add(slot);
            }
        }
    }

    public void Show()
    {
        if (IsActive) return;
        IsActive = true;

        foreach (MaterialSlot slot in slots)
        {
            if (slot.Renderer == null) continue;
            propertyBlock.Clear();
            propertyBlock.SetColor(slot.ColorPropertyId, slot.HighlightedColor);
            slot.Renderer.SetPropertyBlock(propertyBlock, slot.MaterialIndex);
        }
    }

    public void Hide()
    {
        if (!IsActive) return;
        IsActive = false;

        foreach (MaterialSlot slot in slots)
        {
            if (slot.Renderer != null) slot.Renderer.SetPropertyBlock(null, slot.MaterialIndex);
        }
    }

    private void OnDisable() => Hide();

    private bool TryCreateSlot(Renderer rend, int index, Material material, out MaterialSlot slot)
    {
        slot = default;
        if (material == null) return false;

        int propertyId = material.HasProperty(BaseColorId) ? BaseColorId
                       : material.HasProperty(ColorId) ? ColorId
                       : -1;
        if (propertyId == -1) return false;

        Color original = material.GetColor(propertyId);
        Color tinted = Color.Lerp(original, highlightColor, intensity);
        tinted.a = original.a;

        slot = new MaterialSlot
        {
            Renderer = rend,
            MaterialIndex = index,
            ColorPropertyId = propertyId,
            HighlightedColor = tinted
        };
        return true;
    }
}