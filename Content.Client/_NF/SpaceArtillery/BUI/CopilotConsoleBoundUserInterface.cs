using Content.Client.Shuttles.UI;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Events;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Content.Client._NF.SpaceArtillery;
using Content.Client._NF.SpaceArtillery.UI;
using Content.Shared._NF.SpaceArtillery.BUI;
using Content.Shared.SpaceArtillery;

namespace Content.Client._NF.SpaceArtillery.BUI;

[UsedImplicitly]
public sealed class CopilotConsoleBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private CopilotConsoleWindow? _window;

    public CopilotConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = new CopilotConsoleWindow();
        _window.OnClose += Close;
        _window.ShowCopilot += SendCopilotMessage;
        _window.ShowVessel += SendVesselMessage;
        _window.OpenCenteredLeft();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not CopilotConsoleBoundUserInterfaceState bState)
            return;

        _window?.UpdateState(bState);
    }

    private void SendCopilotMessage(bool obj)
    {
       /* SendMessage(new CopilotShowCopilotMessage()
        {
            Show = obj,
        });*/
    }

    private void SendVesselMessage(bool obj)
    {
        /*SendMessage(new CopilotShowVesselMessage()
        {
            Show = obj,
        });*/
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _window?.Close();
            _window = null;
        }
    }
}
