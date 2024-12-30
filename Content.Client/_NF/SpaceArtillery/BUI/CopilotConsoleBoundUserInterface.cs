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

        _window.CopilotCustomButtonOnePressed += OnCopilotCustomButtonOnePressed;
        _window.CopilotCustomButtonTwoPressed += OnCopilotCustomButtonTwoPressed;
        _window.CopilotCustomButtonThreePressed += OnCopilotCustomButtonThreePressed;
        _window.CopilotCustomButtonFourPressed += OnCopilotCustomButtonFourPressed;
        _window.CopilotCustomButtonFivePressed += OnCopilotCustomButtonFivePressed;
        _window.CopilotCustomButtonSixPressed += OnCopilotCustomButtonSixPressed;
        _window.CopilotCustomButtonSevenPressed += OnCopilotCustomButtonSevenPressed;
        _window.CopilotCustomButtonEightPressed += OnCopilotCustomButtonEightPressed;
        _window.CopilotCustomButtonNinePressed += OnCopilotCustomButtonNinePressed;
        _window.CopilotCustomButtonTenPressed += OnCopilotCustomButtonTenPressed;
        _window.CopilotCustomButtonElevenPressed += OnCopilotCustomButtonElevenPressed;
        _window.CopilotCustomButtonTwelvePressed += OnCopilotCustomButtonTwelvePressed;

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



    private void OnCopilotCustomButtonOnePressed()
    {
        SendMessage(new CopilotCustomButtonOneMessage());
    }
    private void OnCopilotCustomButtonTwoPressed()
    {
        SendMessage(new CopilotCustomButtonTwoMessage());
    }
  private void OnCopilotCustomButtonThreePressed()
    {
        SendMessage(new CopilotCustomButtonThreeMessage());
    }
  private void OnCopilotCustomButtonFourPressed()
    {
        SendMessage(new CopilotCustomButtonFourMessage());
    }
  private void OnCopilotCustomButtonFivePressed()
    {
        SendMessage(new CopilotCustomButtonFiveMessage());
    }
  private void OnCopilotCustomButtonSixPressed()
    {
        SendMessage(new CopilotCustomButtonSixMessage());
    }
  private void OnCopilotCustomButtonSevenPressed()
    {
        SendMessage(new CopilotCustomButtonSevenMessage());
    }
  private void OnCopilotCustomButtonEightPressed()
    {
        SendMessage(new CopilotCustomButtonEightMessage());
    }
  private void OnCopilotCustomButtonNinePressed()
    {
        SendMessage(new CopilotCustomButtonNineMessage());
    }
  private void OnCopilotCustomButtonTenPressed()
    {
        SendMessage(new CopilotCustomButtonTenMessage());
    }
  private void OnCopilotCustomButtonElevenPressed()
    {
        SendMessage(new CopilotCustomButtonElevenMessage());
    }
  private void OnCopilotCustomButtonTwelvePressed()
    {
        SendMessage(new CopilotCustomButtonTwelveMessage());
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
