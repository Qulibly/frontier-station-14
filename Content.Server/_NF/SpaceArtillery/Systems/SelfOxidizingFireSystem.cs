using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.GameTicking;
using Content.Server.Temperature.Components;
using Content.Server.Temperature.Systems;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using Content.Shared.Alert;
using Content.Shared.Maps;
using Robust.Shared.Map.Components;
using Content.Shared.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Toggleable;
using Content.Shared.Verbs;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.Containers.EntitySystems;
using Content.Shared.FixedPoint;
using Content.Shared.Spillable;
using Content.Shared.Nutrition.EntitySystems;
using Content.Server.Fluids.EntitySystems;
using Content.Server.Fluids.Components;
using Content.Shared.Fluids;
using Content.Shared.Fluids.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.DoAfter;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry;
using Content.Server.Explosion.EntitySystems;
using Robust.Shared.Audio.Systems;
using Content.Shared.Atmos.Components;
using Content.Shared.Destructible;

namespace Content.Server._NF.SelfOxidizingFire;

public sealed partial class SelfOxidizingFireSystem : EntitySystem
{
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [Dependency] private readonly ITileDefinitionManager _tileDefinitionManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    //[Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly AlertsSystem _alertsSystem = default!;
    [Dependency] private readonly TemperatureSystem _temperatureSystem = default!;
    [Dependency] private readonly AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private readonly SharedHandsSystem _handsSystem = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solutionContainerSystem = default!;
    [Dependency] protected readonly OpenableSystem Openable = default!;
    [Dependency] private readonly PuddleSystem _puddleSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly ReactiveSystem _reactive = default!;
    [Dependency] private readonly ExplosionSystem _explosionSystem = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;

    private const float UpdateTime = 2.5f;
    private float _timer;
    protected ISawmill Sawmill = default!;

    public override void Initialize()
    {
        base.Initialize();
        Sawmill = Logger.GetSawmill("SelfOxidizingFire");
        SubscribeLocalEvent<SelfOxidizingFireComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SelfOxidizingFireComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<SelfOxidizingFireComponent, GetVerbsEvent<AlternativeVerb>>(OnGetAltVerbs);
        SubscribeLocalEvent<SelfOxidizingFireComponent, ResistFireAlertEvent>(OnResistFireAlert);
        SubscribeLocalEvent<SelfOxidizingFireComponent, DestructionEventArgs>(OnDestruction);
    }

    private void OnComponentInit(EntityUid uid, SelfOxidizingFireComponent component, ComponentInit args)
    {
        UpdateAppearance(uid, component);
    }

    private void OnComponentRemove(EntityUid uid, SelfOxidizingFireComponent component, ComponentRemove args)
    {
        //component.OnFire = false;
        //component.FireStacks = 0;
        //UpdateAppearance(uid, component);
    }

    public void Extinguish(EntityUid uid, SelfOxidizingFireComponent? flammable = null)
    {
        //component.OnFire = false;
        //component.FireStacks = 0;
        //
        //_entManager.RemoveComponent<SelfOxidizingFireComponent>(uid);

        if (!Resolve(uid, ref flammable))
            return;

        if (!flammable.OnFire)
            return;

        flammable.OnFire = false;
        flammable.FireStacks = 0;

        _alertsSystem.ClearAlert(uid, flammable.FireAlert);

        UpdateAppearance(uid, flammable);
        _entManager.RemoveComponent(uid, flammable);
    }

    private void OnGetAltVerbs(EntityUid uid, SelfOxidizingFireComponent component, GetVerbsEvent<AlternativeVerb> args)
    {
        Sawmill.Info($"User {args.User} tries to verb {args.Target}");
        if (!args.CanAccess || !args.CanInteract || args.Hands == null)
            return;

        Sawmill.Info($"User {args.User} is executing verb on {args.Target}");
        AlternativeVerb verb = new()
        {
            Text = Loc.GetString("Extinguish PLACEHOLDER"),
            Act = () => VerbExtinguish(args.Target, args.User, args.Hands)
        };
        args.Verbs.Add(verb);
    }

    private void VerbExtinguish(EntityUid target, EntityUid user, HandsComponent hands)
    {
        var item = hands.ActiveHandEntity;

        Sawmill.Info($"Solution container prepared");
        if (!TryComp<SolutionContainerManagerComponent>(item, out var solutionContainer))
            return;

        Sawmill.Info($"Solution container prepared");
        if (!TryComp<SpillableComponent>(item, out var spill))
            return;

        Sawmill.Info($"Solution is being checked for contents");
        if (!_solutionContainerSystem.TryGetSolution(solutionContainer.Owner, spill.SolutionName, out var soln, out var solution)) // TODO Set values properly
            return;



        Sawmill.Info($"Container checked if its closed");
        if (Openable.IsClosed(target))
            return;

        Sawmill.Info($"Container checked if its empty");
        if (solution.Volume == FixedPoint2.Zero)
            return;

        var totalSplit = FixedPoint2.Min(solution.MaxVolume * 0.2, solution.Volume);
        var splitSolution = _solutionContainerSystem.SplitSolution(soln.Value, totalSplit);


        Sawmill.Info($"Spilling is being conducted");
        var puddleSolution = _solutionContainerSystem.SplitSolution(soln.Value, totalSplit);
        _puddleSystem.TrySpillAt(Transform(user).Coordinates, puddleSolution, out _);
        Sawmill.Info($"Spilling ended");

        _reactive.DoEntityReaction(target, splitSolution, ReactionMethod.Touch);
        Sawmill.Info($"Spill reacted");

        //_doAfterSystem.TryStartDoAfter(new DoAfterArgs(EntityManager, user, spill.SpillDelay ?? 0, new SpillDoAfterEvent(), solutionContainer.Owner, target: target)
        //       {
        //           BreakOnDamage = false,
        //           BreakOnMove = true,
        //           NeedHand = true,
        //       });
    }

    private void OnResistFireAlert(EntityUid uid, SelfOxidizingFireComponent component, ref ResistFireAlertEvent args)
    {
        Extinguish(uid, component);
    }

    private void OnDestruction(EntityUid uid, SelfOxidizingFireComponent component, ref DestructionEventArgs args)
    {
        _explosionSystem.QueueExplosion(uid, "SOFexplosion", 6f, 0.5f, 1.25f, canCreateVacuum: true); //on true destroyes tiles
    }

    public void UpdateAppearance(EntityUid uid, SelfOxidizingFireComponent? flammable = null, AppearanceComponent? appearance = null)
    {
        if (!Resolve(uid, ref flammable, ref appearance))
            return;

        _appearance.SetData(uid, SelfOxidizingFireVisuals.OnFire, flammable.OnFire, appearance);
        _appearance.SetData(uid, SelfOxidizingFireVisuals.FireStacks, flammable.FireStacks, appearance);

        // Also enable toggleable-light visuals
        // This is intended so that matches & candles can re-use code for un-shaded layers on in-hand sprites.
        // However, this could cause conflicts if something is ACTUALLY both a toggleable light and flammable.
        // if that ever happens, then fire visuals will need to implement their own in-hand sprite management.
        _appearance.SetData(uid, ToggleableLightVisuals.Enabled, flammable.OnFire, appearance);
    }

    public override void Update(float frameTime)
    {
        _timer += frameTime;

        if (_timer < UpdateTime)
            return;

        _timer -= UpdateTime;

        var query = EntityQueryEnumerator<SelfOxidizingFireComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var flammable, out _))
        {
            switch (flammable.Fuel)
            {
                case var _ when flammable.Fuel > 1000:
                    flammable.Size = 3;
                    break;
                case var _ when flammable.Fuel > 400:
                    flammable.Size = 2;
                    break;
                case var _ when flammable.Fuel > 100:
                    flammable.Size = 1;
                    break;
                case var _ when flammable.Fuel > 0:
                    flammable.Size = 0;
                    break;
                case var _ when flammable.Fuel <= 0:
                    Extinguish(uid, flammable);
                    break;
            }


            var air = _atmosphereSystem.GetContainingMixture(uid);
            var air_temp = air?.Temperature ?? 0f;
            var air_press = air?.Pressure ?? 0f;

            var oxygen = air?.GetMoles(Gas.Oxygen) ?? 0f;
            var nitrogen = air?.GetMoles(Gas.Nitrogen) ?? 0f;
            var waterVapor = air?.GetMoles(Gas.WaterVapor) ?? 0f;

            //Sawmill.Info($"Self Oxidizing Fire update Entity: {ToPrettyString(uid)}  Temperature: {air_temp}  Pressure: {air_press}  Oxygen: {oxygen}  Nitrogen: {nitrogen}");
            _alertsSystem.ShowAlert(uid, flammable.FireAlert);

            if (!flammable.OnFire)
            {
                _alertsSystem.ClearAlert(uid, flammable.FireAlert);
                continue;
            }

            flammable.BurnPlayingStream = _audio.PlayPredicted(flammable.BurnSound, uid, uid)?.Entity;

            if (flammable.Volatility >= 200)
            {
                //In testing used 12 intensity, max 1.25
                _explosionSystem.QueueExplosion(uid, "SOFexplosion", flammable.Fuel * flammable.Size * 0.2f, 0.5f, flammable.Size, canCreateVacuum: false); //on true destroyes tiles

                flammable.Volatility = flammable.Volatility / 2f;
                flammable.Fuel = flammable.Fuel / (flammable.Size * 2f);
            }

            // Handles the fire reacting to changes in atmosphere
            // First checks if there even is atmosphere or should treat it as vacum
            // TODO write "temp" in actually sane method
            // TODO do changing of valitility and fuel in more sane way
            // TODO change 'magic numbers' into actual variables
            // TODO check if multiplying damage does anything
            // TODO make fuel and volatility react proper
            if (air == null || air.Temperature < 5f || air.Pressure <= 2f)
            {
                _damageableSystem.TryChangeDamage(uid, flammable.DamageVacuum * flammable.Size, interruptsDoAfters: false);

                if (TryComp(uid, out TemperatureComponent? temp))
                {
                    _temperatureSystem.ChangeHeat(uid, -2500 * flammable.FireStacks * flammable.Size, false, temp);
                    flammable.Fuel -= 27f * flammable.Size;
                }

                flammable.Fuel += 1f * flammable.Size + 1f;
                flammable.Volatility += 1f * flammable.Size;
            }
            else
            {
                //Handles fire if air exists


                if (nitrogen > 1f)
                {
                    _damageableSystem.TryChangeDamage(uid, flammable.DamageNitrogen, interruptsDoAfters: false);

                    if (air != null)
                        air.Temperature += 10 * flammable.Size;
                    flammable.Volatility += nitrogen * 0.4f / 2f * flammable.Size;
                    air?.SetMoles(Gas.Nitrogen, nitrogen * 0.6f / flammable.Size);
                    air?.AdjustMoles(Gas.NitrousOxide, nitrogen * 0.02f * flammable.Size);
                    if (TryComp(uid, out TemperatureComponent? temp_))
                    {
                        _temperatureSystem.ChangeHeat(uid, +500 * flammable.FireStacks * flammable.Size, false, temp_);
                        flammable.Fuel -= 10f * flammable.Size;
                    }
                    flammable.Fuel += 2f * flammable.Size;
                }
                else
                    air?.SetMoles(Gas.Nitrogen, 0);

                if (oxygen > 1f)
                {
                    _damageableSystem.TryChangeDamage(uid, flammable.DamageOxygen * flammable.Size, interruptsDoAfters: false);

                    if (air != null)
                        air.Temperature += 25 * flammable.Size;
                    flammable.Volatility += oxygen * 0.4f / 4f * flammable.Size;
                    air?.SetMoles(Gas.Oxygen, oxygen * 0.6f / flammable.Size);
                    air?.AdjustMoles(Gas.CarbonDioxide, oxygen * 0.2f * flammable.Size);
                    if (TryComp(uid, out TemperatureComponent? temp__))
                    {
                        _temperatureSystem.ChangeHeat(uid, +2500 * flammable.FireStacks * flammable.Size, false, temp__);
                        flammable.Fuel -= 10f * flammable.Size;
                    }
                    flammable.Fuel += 25f + oxygen * 0.4f * flammable.Size;
                }
                else
                    air?.SetMoles(Gas.Oxygen, 0);

                if (waterVapor > 1f)
                {
                    air?.SetMoles(Gas.WaterVapor, waterVapor * 0.9f);
                    flammable.Fuel -= waterVapor;
                }
                //Things that happen regardless of oxygen and nitrogen being present
                if (air != null)
                    air.Temperature -= 15 * flammable.Size;

                _damageableSystem.TryChangeDamage(uid, flammable.DamageAir * flammable.Size, interruptsDoAfters: false);

                if (TryComp(uid, out TemperatureComponent? temp___))
                {
                    _temperatureSystem.ChangeHeat(uid, -750 * flammable.FireStacks * flammable.Size, false, temp___);
                    flammable.Fuel -= 10f * flammable.Size;
                }
                flammable.Fuel += 5f * flammable.Size + 1f;
                flammable.Volatility += 10f * flammable.Size;
                //if (TryComp(uid, out TemperatureComponent? temp))
                //    _temperatureSystem.ChangeHeat(uid, -12500 * flammable.FireStacks, true, temp);
            }
        }
    }
}
