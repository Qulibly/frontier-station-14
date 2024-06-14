using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization;

namespace Content.Shared.SpaceArtillery;

/// <summary>
/// This is used for identifying contraband items and can be added to items through yml
/// </summary>
[RegisterComponent]
public sealed partial class CopilotComponent : Component
{
    /// <summary>
    /// The value of the contraband. Defaults to 1 for easy addition to any number of items.
    /// </summary>
    [DataField("value")]
    public int Value = 1;

}
