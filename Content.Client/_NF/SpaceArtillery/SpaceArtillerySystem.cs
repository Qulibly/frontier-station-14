using Content.Shared.Rounding;
using Content.Shared.SpaceArtillery;
using Robust.Client.GameObjects;

namespace Content.Client._NF.SpaceArtillery;


public sealed class SpaceArtillerySystem : VisualizerSystem<SpaceArtilleryVisualsComponent>//SharedSpaceArtillerySystem
{
    [Dependency] private readonly SpriteSystem _sprite = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpaceArtilleryVisualsComponent, ComponentInit>(OnVisualsInit);
    }

    private void OnVisualsInit(EntityUid uid, SpaceArtilleryVisualsComponent component, ComponentInit args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite)) return;

        if (_sprite.LayerMapTryGet((uid, sprite), CoolantSpaceArtilleryVisualLayers.Coolant, out var coolantLayer, false))
        {
            _sprite.LayerSetRsiState((uid, sprite), CoolantSpaceArtilleryVisualLayers.Coolant, $"{component.CoolantState}-{component.CoolantSteps - 1}");
            _sprite.LayerSetVisible((uid, sprite), CoolantSpaceArtilleryVisualLayers.Coolant, true);
        }

        if (_sprite.LayerMapTryGet((uid, sprite), CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, out var coolantUnshadedLayer, false))
        {
            _sprite.LayerSetRsiState((uid, sprite), CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, $"{component.CoolantState}-unshaded-{component.CoolantSteps - 1}");
            _sprite.LayerSetVisible((uid, sprite), CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, true);
        }
    }

    protected override void OnAppearanceChange(EntityUid uid, SpaceArtilleryVisualsComponent component, ref AppearanceChangeEvent args)
    {
        var sprite = args.Sprite;

        if (sprite == null)
        {
            return;
        }

        if (!args.AppearanceData.TryGetValue(SpaceArtilleryVisuals.CoolantMax, out var capacity))
        {
            capacity = component.CoolantSteps;
        }

        if (!args.AppearanceData.TryGetValue(SpaceArtilleryVisuals.CoolantCount, out var current))
        {
            current = component.CoolantSteps;
        }
        var step = ContentHelpers.RoundToLevels((int)current, (int)capacity, component.CoolantSteps);



        if (step == 0 && !component.ZeroVisible)
        {
            if (_sprite.LayerMapTryGet((uid, sprite), CoolantSpaceArtilleryVisualLayers.Coolant, out _, false))
            {
                _sprite.LayerSetVisible((uid, sprite), CoolantSpaceArtilleryVisualLayers.Coolant, false);
            }

            if (_sprite.LayerMapTryGet((uid, sprite), CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, out _, false))
            {
                _sprite.LayerSetVisible((uid, sprite), CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, false);
            }

            return;
        }


        if (_sprite.LayerMapTryGet((uid, sprite), CoolantSpaceArtilleryVisualLayers.Coolant, out var coolantLayer, false))
        {
            _sprite.LayerSetVisible((uid, sprite), CoolantSpaceArtilleryVisualLayers.Coolant, true);
            _sprite.LayerSetRsiState((uid, sprite), CoolantSpaceArtilleryVisualLayers.Coolant, $"{component.CoolantState}-{step}");
        }
        else
        {
            _sprite.LayerSetVisible((uid, sprite), CoolantSpaceArtilleryVisualLayers.Coolant, false);
        }


        if (_sprite.LayerMapTryGet((uid, sprite), CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, out var coolantUnshadedLayer, false))
        {
            _sprite.LayerSetVisible((uid, sprite), CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, true);
            _sprite.LayerSetRsiState((uid, sprite), CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, $"{component.CoolantState}-unshaded-{step}");
        }
        else
        {
            _sprite.LayerSetVisible((uid, sprite), CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, false);
        }
    }
}
