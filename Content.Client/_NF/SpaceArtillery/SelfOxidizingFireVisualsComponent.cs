namespace Content.Client.Atmos.Components;

/// <summary>
/// Sets which sprite RSI is used for displaying the fire visuals and what state to use based on the fire stacks
/// accumulated.
/// </summary>
[RegisterComponent]
public sealed partial class SelfOxidizingFireVisualsComponent : Component
{
    [DataField("fireStackAlternateState")]
    public int FireStackAlternateState = 3;

    [DataField("normalState")]
    public string? NormalState = "1";

    [DataField("alternateState")]
    public string? AlternateState;

    [DataField("sprite")]
    //public string? Sprite;
    public string? Sprite = "_NF/Objects/SpaceArtillery/self_oxidizing_fire.rsi";

    [DataField("lightEnergyPerStack")]
    public float LightEnergyPerStack = 0.5f;

    [DataField("lightRadiusPerStack")]
    public float LightRadiusPerStack = 0.3f;

    [DataField("maxLightEnergy")]
    public float MaxLightEnergy = 10f;

    [DataField("maxLightRadius")]
    public float MaxLightRadius = 4f;

    [DataField("lightColor")]
    public Color LightColor = Color.Blue;

    /// <summary>
    ///     Client side point-light entity. We use this instead of directly adding a light to
    ///     the burning entity as entities don't support having multiple point-lights.
    /// </summary>
    public EntityUid? LightEntity;
}