using Content.Client.Atmos.Components;
using Content.Shared.Atmos;
using Robust.Client.GameObjects;
using Robust.Shared.Map;

namespace Content.Client.Atmos.EntitySystems;

/// <summary>
/// This handles the display of fire effects on flammable entities.
/// </summary>
public sealed class SelfOxidizingFireVisualizerSystem : VisualizerSystem<SelfOxidizingFireVisualsComponent>
{
    [Dependency] private readonly PointLightSystem _lights = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SelfOxidizingFireVisualsComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SelfOxidizingFireVisualsComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnShutdown(EntityUid uid, SelfOxidizingFireVisualsComponent component, ComponentShutdown args)
    {
        if (component.LightEntity != null)
        {
            Del(component.LightEntity.Value);
            component.LightEntity = null;
        }

        // Need LayerMapTryGet because Init fails if there's no existing sprite / appearancecomp
        // which means in some setups (most frequently no AppearanceComp) the layer never exists.
        if (TryComp<SpriteComponent>(uid, out var sprite) &&
            sprite.LayerMapTryGet(SelfOxidizingFireVisualLayers.Fire, out var layer))
        {
            sprite.RemoveLayer(layer);
        }
    }

    private void OnComponentInit(EntityUid uid, SelfOxidizingFireVisualsComponent component, ComponentInit args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite) || !TryComp(uid, out AppearanceComponent? appearance))
            return;

        sprite.LayerMapReserveBlank(SelfOxidizingFireVisualLayers.Fire);
        sprite.LayerSetVisible(SelfOxidizingFireVisualLayers.Fire, false);
        sprite.LayerSetShader(SelfOxidizingFireVisualLayers.Fire, "unshaded");
        sprite.LayerSetRotation(SelfOxidizingFireVisualLayers.Fire, 45); // Test placeholder
        if (component.Sprite != null)
            sprite.LayerSetRSI(SelfOxidizingFireVisualLayers.Fire, component.Sprite);

        UpdateAppearance(uid, component, sprite, appearance);
    }

    protected override void OnAppearanceChange(EntityUid uid, SelfOxidizingFireVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite != null)
            UpdateAppearance(uid, component, args.Sprite, args.Component);
    }

    private void UpdateAppearance(EntityUid uid, SelfOxidizingFireVisualsComponent component, SpriteComponent sprite, AppearanceComponent appearance)
    {
        if (!sprite.LayerMapTryGet(SelfOxidizingFireVisualLayers.Fire, out var index))
            return;

        AppearanceSystem.TryGetData<bool>(uid, SelfOxidizingFireVisuals.OnFire, out var onFire, appearance);
        AppearanceSystem.TryGetData<float>(uid, SelfOxidizingFireVisuals.FireStacks, out var fireStacks, appearance);
        sprite.LayerSetVisible(index, onFire);

        if (!onFire)
        {
            if (component.LightEntity != null)
            {
                Del(component.LightEntity.Value);
                component.LightEntity = null;
            }

            return;
        }

        if (fireStacks > component.FireStackAlternateState && !string.IsNullOrEmpty(component.AlternateState))
            sprite.LayerSetState(index, component.AlternateState);
        else
            sprite.LayerSetState(index, component.NormalState);

        component.LightEntity ??= Spawn(null, new EntityCoordinates(uid, default));
        var light = EnsureComp<PointLightComponent>(component.LightEntity.Value);

        _lights.SetColor(component.LightEntity.Value, component.LightColor, light);

        // light needs a minimum radius to be visible at all, hence the + 1.5f
        _lights.SetRadius(component.LightEntity.Value, Math.Clamp(1.5f + component.LightRadiusPerStack * fireStacks, 0f, component.MaxLightRadius), light);
        _lights.SetEnergy(component.LightEntity.Value, Math.Clamp(1 + component.LightEnergyPerStack * fireStacks, 0f, component.MaxLightEnergy), light);

        // TODO flickering animation? Or just add a noise mask to the light? But that requires an engine PR.
    }
}

public enum SelfOxidizingFireVisualLayers : byte
{
    Fire
}