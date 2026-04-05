using Content.Shared.Actions;
using Robust.Shared.Serialization;

namespace Content.Shared.SpaceArtillery;

public abstract partial class SharedSpaceArtillerySystem : EntitySystem
{
}
/// <summary>
/// Raised when someone fires the artillery
/// </summary>
public sealed partial class FireActionEvent : InstantActionEvent
{
}

[NetSerializable, Serializable]
public enum CopilotConsoleUiKey : byte
{
    Copilot,
    ArmamentAvailability
}

[Serializable, NetSerializable]
public enum SpaceArtilleryVisuals : byte
{
    CoolantCount,
    CoolantMax,
    VisualState,
}

[NetSerializable, Serializable]
public enum SpaceArtilleryVisualState
{
    On,
    Underpowered,
    Off
}

[Serializable, NetSerializable]
public enum SpaceArtilleryVisualLayers : byte
{
    Lights
}
