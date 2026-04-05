using Content.Shared.Rounding;
using Robust.Client.GameObjects;

namespace Content.Shared.SpaceArtillery;


public sealed class SpaceArtillerySystem : SharedSpaceArtillerySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpaceArtilleryVisualsComponent, ComponentInit>(OnMagazineVisualsInit);
        SubscribeLocalEvent<SpaceArtilleryVisualsComponent, AppearanceChangeEvent>(OnMagazineVisualsChange);
    }

    private void OnMagazineVisualsInit(EntityUid uid, SpaceArtilleryVisualsComponent component, ComponentInit args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite)) return;

        if (sprite.LayerMapTryGet(CoolantSpaceArtilleryVisualLayers.Coolant, out _))
        {
            sprite.LayerSetState(CoolantSpaceArtilleryVisualLayers.Coolant, $"{component.CoolantState}-{component.CoolantSteps - 1}");
            sprite.LayerSetVisible(CoolantSpaceArtilleryVisualLayers.Coolant, false);
        }

        if (sprite.LayerMapTryGet(CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, out _))
        {
            sprite.LayerSetState(CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, $"{component.CoolantState}-unshaded-{component.CoolantSteps - 1}");
            sprite.LayerSetVisible(CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, true);
        }
    }

    private void OnMagazineVisualsChange(EntityUid uid, SpaceArtilleryVisualsComponent component, ref AppearanceChangeEvent args)
    {
        // tl;dr
        // 1.If no mag then hide it OR
        // 2. If step 0 isn't visible then hide it (mag or unshaded)
        // 3. Otherwise just do mag / unshaded as is
        var sprite = args.Sprite;

        if (sprite == null)
        {
            throw new InvalidOperationException("Sprite is null!!!");
            //return;
        }

        if (!args.AppearanceData.TryGetValue(SpaceArtilleryVisuals.CoolantMax, out var capacity))
        {
            capacity = component.CoolantSteps;
        }

        if (!args.AppearanceData.TryGetValue(SpaceArtilleryVisuals.CoolantCount, out var current))
        {
            current = component.CoolantSteps;
        }
        //ISSUE WITH LAYERS. LAYER COOLANT DOES NOT EXIST
        //coolant-5 DOES NOT EXIST
        //COOLANT LAYER DOES NOT WANT TO BECOME VISIBLE
        var step = ContentHelpers.RoundToLevels((int)current, (int)capacity, component.CoolantSteps);
        sprite.LayerSetVisible(CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, true);
        sprite.LayerSetVisible(CoolantSpaceArtilleryVisualLayers.Base, false);
        sprite.LayerSetState(CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, $"{component.CoolantState}-unshaded-{step}");
        if (step == 0 && !component.ZeroVisible)
        {
            if (sprite.LayerMapTryGet(CoolantSpaceArtilleryVisualLayers.Coolant, out _))
            {
                sprite.LayerSetVisible(CoolantSpaceArtilleryVisualLayers.Coolant, false);
            }

            if (sprite.LayerMapTryGet(CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, out _))
            {
                sprite.LayerSetVisible(CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, false);
            }

            return;
        }

        if (sprite.LayerMapTryGet(CoolantSpaceArtilleryVisualLayers.Coolant, out _))
        {
            sprite.LayerSetVisible(CoolantSpaceArtilleryVisualLayers.Coolant, true);
            sprite.LayerSetState(CoolantSpaceArtilleryVisualLayers.Coolant, $"{component.CoolantState}-{step}");
        }

        if (sprite.LayerMapTryGet(CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, out _))
        {
            sprite.LayerSetVisible(CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, true);
            sprite.LayerSetState(CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, $"{component.CoolantState}-unshaded-{step}");
        }
        if (sprite.LayerMapTryGet(CoolantSpaceArtilleryVisualLayers.Coolant, out _))
        {
            sprite.LayerSetVisible(CoolantSpaceArtilleryVisualLayers.Coolant, false);
        }

        if (sprite.LayerMapTryGet(CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, out _))
        {
            sprite.LayerSetVisible(CoolantSpaceArtilleryVisualLayers.CoolantUnshaded, false);
        }
    }



}
