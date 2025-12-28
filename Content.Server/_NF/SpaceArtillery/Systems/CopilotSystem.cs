using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Components;
using Content.Server.DeviceNetwork.Systems;
using Content.Shared.Interaction;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Content.Shared.Power;
using Content.Shared.UserInterface;
using Content.Server.DeviceLinking.Events;
using Content.Server.DeviceLinking.Components;
using Content.Server.DeviceLinking.Systems;
using Content.Server.DeviceNetwork;
using Content.Shared.SpaceArtillery;
using Content.Shared._NF.SpaceArtillery.BUI;

namespace Content.Server._NF.SpaceArtillery.Copilot;

public sealed class CopilotSystem : EntitySystem
{
    [Dependency] private readonly DeviceLinkSystem _deviceLink = default!;

    public override void Initialize()
    {
        base.Initialize();



        // Interaction
        //SubscribeLocalEvent<CopilotComponent, InteractUsingEvent>(OnInteractUsing);
        //SubscribeLocalEvent<CopilotComponent, GotEmaggedEvent>(OnEmagged);

        // UI
        SubscribeLocalEvent<CopilotComponent, CopilotCustomButtonOneMessage>(OnCopilotCustomButtonOnePressed);
        SubscribeLocalEvent<CopilotComponent, CopilotCustomButtonTwoMessage>(OnCopilotCustomButtonTwoPressed);
        SubscribeLocalEvent<CopilotComponent, CopilotCustomButtonThreeMessage>(OnCopilotCustomButtonThreePressed);
        SubscribeLocalEvent<CopilotComponent, CopilotCustomButtonFourMessage>(OnCopilotCustomButtonFourPressed);
        SubscribeLocalEvent<CopilotComponent, CopilotCustomButtonFiveMessage>(OnCopilotCustomButtonFivePressed);
        SubscribeLocalEvent<CopilotComponent, CopilotCustomButtonSixMessage>(OnCopilotCustomButtonSixPressed);
        SubscribeLocalEvent<CopilotComponent, CopilotCustomButtonSevenMessage>(OnCopilotCustomButtonSevenPressed);
        SubscribeLocalEvent<CopilotComponent, CopilotCustomButtonEightMessage>(OnCopilotCustomButtonEightPressed);
        SubscribeLocalEvent<CopilotComponent, CopilotCustomButtonNineMessage>(OnCopilotCustomButtonNinePressed);
        SubscribeLocalEvent<CopilotComponent, CopilotCustomButtonTenMessage>(OnCopilotCustomButtonTenPressed);
        SubscribeLocalEvent<CopilotComponent, CopilotCustomButtonElevenMessage>(OnCopilotCustomButtonElevenPressed);
        SubscribeLocalEvent<CopilotComponent, CopilotCustomButtonTwelveMessage>(OnCopilotCustomButtonTwelvePressed);

    }



    //Copilot button handling
    private void OnCopilotCustomButtonOnePressed(EntityUid uid, CopilotComponent component, CopilotCustomButtonOneMessage args)
    {
        _deviceLink.SendSignal(uid, component.CopilotCustomButtonOnePort, false);
    }
    private void OnCopilotCustomButtonTwoPressed(EntityUid uid, CopilotComponent component, CopilotCustomButtonTwoMessage args)
    {
        _deviceLink.SendSignal(uid, component.CopilotCustomButtonTwoPort, false);
    }
    private void OnCopilotCustomButtonThreePressed(EntityUid uid, CopilotComponent component, CopilotCustomButtonThreeMessage args)
    {
        _deviceLink.SendSignal(uid, component.CopilotCustomButtonThreePort, false);
    }
    private void OnCopilotCustomButtonFourPressed(EntityUid uid, CopilotComponent component, CopilotCustomButtonFourMessage args)
    {
        _deviceLink.SendSignal(uid, component.CopilotCustomButtonFourPort, false);
    }
    private void OnCopilotCustomButtonFivePressed(EntityUid uid, CopilotComponent component, CopilotCustomButtonFiveMessage args)
    {
        _deviceLink.SendSignal(uid, component.CopilotCustomButtonFivePort, false);
    }
    private void OnCopilotCustomButtonSixPressed(EntityUid uid, CopilotComponent component, CopilotCustomButtonSixMessage args)
    {
        _deviceLink.SendSignal(uid, component.CopilotCustomButtonSixPort, false);
    }
    private void OnCopilotCustomButtonSevenPressed(EntityUid uid, CopilotComponent component, CopilotCustomButtonSevenMessage args)
    {
        _deviceLink.SendSignal(uid, component.CopilotCustomButtonSevenPort, false);
    }
    private void OnCopilotCustomButtonEightPressed(EntityUid uid, CopilotComponent component, CopilotCustomButtonEightMessage args)
    {
        _deviceLink.SendSignal(uid, component.CopilotCustomButtonEightPort, false);
    }
    private void OnCopilotCustomButtonNinePressed(EntityUid uid, CopilotComponent component, CopilotCustomButtonNineMessage args)
    {
        _deviceLink.SendSignal(uid, component.CopilotCustomButtonNinePort, false);
    }
    private void OnCopilotCustomButtonTenPressed(EntityUid uid, CopilotComponent component, CopilotCustomButtonTenMessage args)
    {
        _deviceLink.SendSignal(uid, component.CopilotCustomButtonTenPort, false);
    }
    private void OnCopilotCustomButtonElevenPressed(EntityUid uid, CopilotComponent component, CopilotCustomButtonElevenMessage args)
    {
        _deviceLink.SendSignal(uid, component.CopilotCustomButtonElevenPort, false);
    }
    private void OnCopilotCustomButtonTwelvePressed(EntityUid uid, CopilotComponent component, CopilotCustomButtonTwelveMessage args)
    {
        _deviceLink.SendSignal(uid, component.CopilotCustomButtonTwelvePort, false);
    }


}