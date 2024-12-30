namespace Content.Server._NF.SpaceArtillery.Copilot;
using Content.Shared.SpaceArtillery;
using Content.Shared.Construction.Prototypes;
using Content.Shared.DeviceLinking;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Utility;


[RegisterComponent]
public sealed partial class CopilotComponent : Component
{
	/// <summary>
    /// The ports that send signal when one of the buttons on console are pressed
    /// </summary>

    // Button One 1
    [DataField("copilotCustomButtonOnePort", customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string CopilotCustomButtonOnePort = "CopilotCustomButtonOne";


    // Button Two 2
    [DataField("copilotCustomButtonTwoPort", customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string CopilotCustomButtonTwoPort = "CopilotCustomButtonTwo";


    // Button Three 3
    [DataField("copilotCustomButtonThreePort", customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string CopilotCustomButtonThreePort = "CopilotCustomButtonThree";


    // Button Four 4
    [DataField("copilotCustomButtonFourPort", customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string CopilotCustomButtonFourPort = "CopilotCustomButtonFour";


    // Button Five 5
    [DataField("copilotCustomButtonFivePort", customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string CopilotCustomButtonFivePort = "CopilotCustomButtonFive";


    // Button Six 6
    [DataField("copilotCustomButtonSixPort", customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string CopilotCustomButtonSixPort = "CopilotCustomButtonSix";


    // Button Seven 7
    [DataField("copilotCustomButtonSevenPort", customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string CopilotCustomButtonSevenPort = "CopilotCustomButtonSeven";


    // Button Eight 8
    [DataField("copilotCustomButtonEightPort", customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string CopilotCustomButtonEightPort = "CopilotCustomButtonEight";


    // Button Nine 9
    [DataField("copilotCustomButtonNinePort", customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string CopilotCustomButtonNinePort = "CopilotCustomButtonNine";


    // Button Ten 10
    [DataField("copilotCustomButtonTenPort", customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string CopilotCustomButtonTenPort = "CopilotCustomButtonTen";


    // Button Eleven 11
    [DataField("copilotCustomButtonElevenPort", customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string CopilotCustomButtonElevenPort = "CopilotCustomButtonEleven";


    // Button Twelve 12
    [DataField("copilotCustomButtonTwelvePort", customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string CopilotCustomButtonTwelvePort = "CopilotCustomButtonTwelve";
}