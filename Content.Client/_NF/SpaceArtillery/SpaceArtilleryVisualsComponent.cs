using Content.Shared.SpaceArtillery;
using System.Collections.Generic;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

namespace Content.Shared.SpaceArtillery;

/// <summary>
/// Visualizer for coolant presence; can change states based on filled count or toggle visibility entirely.
/// </summary>
[RegisterComponent, Access(typeof(SpaceArtillerySystem))]
public sealed partial class SpaceArtilleryVisualsComponent : Component
{
    /// <summary>
    /// What RsiState we use.
    /// </summary>
    [DataField("coolantState")] public string? CoolantState;

    /// <summary>
    /// How many steps there are
    /// </summary>
    [DataField("steps")] public int CoolantSteps;

    /// <summary>
    /// Should we hide when the count is 0
    /// </summary>
    [DataField("zeroVisible")] public bool ZeroVisible;
}

public enum CoolantSpaceArtilleryVisualLayers : byte
{
    Base,
    BaseUnshaded,
    Coolant,
    CoolantUnshaded,
}
