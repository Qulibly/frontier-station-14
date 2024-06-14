using Content.Shared.Actions;
using Robust.Shared.Serialization;

namespace Content.Shared.SpaceArtillery;

public sealed class SharedSpaceArtillerySystem : EntitySystem
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
    Copilot
}