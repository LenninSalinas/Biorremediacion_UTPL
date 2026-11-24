/// <summary>Kind of interactive object in the scene.</summary>
public enum InteractableType
{
    LeftGlove,
    RightGlove,
    Multiparameter,
    Gps,
    Bottle,
    Grabber,
    Probe
}

/// <summary>Player hand that holds an object.</summary>
public enum HandSide
{
    None,
    Left,
    Right
}

public static class InteractableTypeExtensions
{
    public static bool IsGlove(this InteractableType type) =>
        type == InteractableType.LeftGlove || type == InteractableType.RightGlove;

    /// <summary>Hand that holds this type. None means it is not held by hand.</summary>
    public static HandSide GetHoldingHand(this InteractableType type) => type switch
    {
        InteractableType.Multiparameter => HandSide.Left,
        InteractableType.Gps => HandSide.Left,
        InteractableType.Bottle => HandSide.Left,
        InteractableType.Grabber => HandSide.Right,
        _ => HandSide.None
    };
}