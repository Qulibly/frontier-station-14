using Content.Shared.Alert;
using Content.Shared.Damage;
using Robust.Shared.Prototypes;
using Robust.Shared.Audio;

namespace Content.Server._NF.SelfOxidizingFire;

[RegisterComponent]
public sealed partial class SelfOxidizingFireComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField]
    public bool OnFire = true;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField]
    public float FireStacks = 10;

    //Determines the fire size and strength. Extinguishing will lower it. Is essentially fire's "health"
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField]
    public float Fuel = 101;

    //Handles the spread of the fire
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField]
    public float Volatility = 0;

    //Determines fire's strength. At 3 is able to resist single extinguish attempt. with each becomes more capable
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField]
    public float Size = 1;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField(required: true)]
    public DamageSpecifier DamageVacuum = new() // Empty by default, we don't want any funny NREs.
    {
        DamageDict = new()
        {
            { "Cold", 2.5 },
            { "Heat", 1 },
            { "Structural", 25 }
        }
    };

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField(required: true)]
    public DamageSpecifier DamageAir = new() // Empty by default, we don't want any funny NREs.
    {
        DamageDict = new()
        {
            { "Cold", 1 },
            { "Heat", 0.5 },
            { "Structural", 12 }
        }
    };

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField(required: true)]
    public DamageSpecifier DamageNitrogen = new() // Empty by default, we don't want any funny NREs.
    {
        DamageDict = new()
        {
            { "Cold", 0.25 },
            { "Heat", 0.12 },
            { "Structural", 12 }
        }
    };

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField(required: true)]
    public DamageSpecifier DamageOxygen = new() // Empty by default, we don't want any funny NREs.
    {
        DamageDict = new()
        {
            { "Cold", 1 },
            { "Heat", 1 },
            { "Structural", 35 }
        }
    };

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField]
    public SoundSpecifier? BurnSound = new SoundPathSpecifier("/Audio/Effects/Vehicle/carhorn.ogg")
    {
        Params = AudioParams.Default.WithVolume(-3f)
    };

    [ViewVariables]
    public EntityUid? BurnPlayingStream;
    [DataField]
    public ProtoId<AlertPrototype> FireAlert = "SelfOxidizingFire";
}
