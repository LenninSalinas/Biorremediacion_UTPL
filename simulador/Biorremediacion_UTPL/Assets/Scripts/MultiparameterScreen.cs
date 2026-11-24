using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>Multiparameter display: shows the latest reading for each probe type.</summary>
[DisallowMultipleComponent]
public class MultiparameterScreen : MonoBehaviour
{
    private const string EmptyValue = "--";

    [Header("UI")]
    [FormerlySerializedAs("textoPH")]
    [SerializeField] private TextMeshProUGUI phText;
    [FormerlySerializedAs("textoO2")]
    [SerializeField] private TextMeshProUGUI dissolvedOxygenText;
    [FormerlySerializedAs("textoCond")]
    [SerializeField] private TextMeshProUGUI conductivityText;

    private void Awake() => ClearAll();

    public void ShowReading(ProbeType type, float value) => SetValue(type, FormatValue(type, value));

    public void ClearReading(ProbeType type) => SetValue(type, EmptyValue);

    public void ClearAll()
    {
        foreach (ProbeType type in (ProbeType[])Enum.GetValues(typeof(ProbeType)))
            ClearReading(type);
    }

    private void SetValue(ProbeType type, string value)
    {
        TextMeshProUGUI target = GetText(type);
        if (target != null) target.text = $"{GetLabel(type)}: {value}";
    }

    private TextMeshProUGUI GetText(ProbeType type) => type switch
    {
        ProbeType.Ph => phText,
        ProbeType.DissolvedOxygen => dissolvedOxygenText,
        ProbeType.Conductivity => conductivityText,
        _ => null
    };

    private static string GetLabel(ProbeType type) => type switch
    {
        ProbeType.Ph => "pH",
        ProbeType.DissolvedOxygen => "O2",
        ProbeType.Conductivity => "Cond",
        _ => type.ToString()
    };

    // Invariant culture: always "7.50", never "7,50", regardless of the player's system language.
    private static string FormatValue(ProbeType type, float value) => type switch
    {
        ProbeType.Ph => value.ToString("F2", CultureInfo.InvariantCulture),
        ProbeType.DissolvedOxygen => value.ToString("F2", CultureInfo.InvariantCulture) + " mg/L",
        ProbeType.Conductivity => value.ToString("F0", CultureInfo.InvariantCulture) + " µS/cm",
        _ => value.ToString(CultureInfo.InvariantCulture)
    };
}