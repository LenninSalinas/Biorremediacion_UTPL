/// <summary>
/// Pure rules (no scene dependencies) for what the player can hold at the same time.
/// The only allowed pair is multiparameter (left hand) + grabber (right hand).
/// </summary>
public static class GrabRules
{
    public static bool CanHold(InteractableType target,
                               InteractableType? heldInLeft,
                               InteractableType? heldInRight)
    {
        HandSide side = target.GetHoldingHand();
        if (side == HandSide.None) return false;

        InteractableType? targetHand = side == HandSide.Left ? heldInLeft : heldInRight;
        InteractableType? otherHand = side == HandSide.Left ? heldInRight : heldInLeft;

        if (targetHand.HasValue) return false;   // that hand is already busy
        if (!otherHand.HasValue) return true;    // both hands are free
        return IsAllowedPair(target, otherHand.Value);
    }

    private static bool IsAllowedPair(InteractableType a, InteractableType b) =>
        (a == InteractableType.Multiparameter && b == InteractableType.Grabber) ||
        (a == InteractableType.Grabber && b == InteractableType.Multiparameter);
}