using Content.Shared.SpaceArtillery;
using Content.Server.DeviceLinking.Events;
using Content.Server.Projectiles;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Interaction.Events;
using System.Numerics;
using Content.Shared.CombatMode;
using Content.Shared.Interaction;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;
using Content.Shared.Actions;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Power;
using Content.Shared.Rounding;
using Robust.Shared.Containers;
using Content.Shared.Stacks;
using Content.Server.Stack;
using Robust.Shared.Prototypes;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Random;

namespace Content.Shared.SpaceArtillery;

public sealed partial class SpaceArtillerySystem : EntitySystem
{
	[Dependency] private readonly IPrototypeManager _prototypeManager = default!;
	[Dependency] private readonly ProjectileSystem _projectile = default!;
	[Dependency] private readonly GunSystem _gun = default!;
	[Dependency] private readonly SharedCombatModeSystem _combat = default!;
	//[Dependency] private readonly RotateToFaceSystem _rotate = default!;
	[Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
	[Dependency] private readonly SharedBuckleSystem _buckle = default!;
	[Dependency] private readonly SharedContainerSystem _containerSystem = default!;
	[Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
	[Dependency] private readonly SharedPhysicsSystem _physicsSystem = default!;
	[Dependency] private readonly IRobustRandom _random = default!;
	
	private const float ShootSpeed = 30f;
	private const float distance = 100;
	private const float rotOffset = 3.14159265f; //Without it the artillery would be shooting backwards.The rotation is in radians and equals to 180 degrees
	//private readonly TransformSystem _transform;
	
	protected ISawmill Sawmill = default!;
	
	public override void Initialize()
	{
		base.Initialize();
		Sawmill = Logger.GetSawmill("SpaceArtillery");
		SubscribeLocalEvent<SpaceArtilleryComponent, SignalReceivedEvent>(OnSignalReceived);
		SubscribeLocalEvent<SpaceArtilleryComponent, BuckleChangeEvent>(OnBuckleChange);
		SubscribeLocalEvent<SpaceArtilleryComponent, FireActionEvent>(OnFireAction);
		SubscribeLocalEvent<SpaceArtilleryComponent, PowerChangedEvent>(OnApcChanged);
		SubscribeLocalEvent<SpaceArtilleryComponent, ChargeChangedEvent>(OnBatteryChargeChanged);
		SubscribeLocalEvent<SpaceArtilleryComponent, EntInsertedIntoContainerMessage>(OnCoolantSlotChanged);
        SubscribeLocalEvent<SpaceArtilleryComponent, EntRemovedFromContainerMessage>(OnCoolantSlotChanged);
		SubscribeLocalEvent<SpaceArtilleryComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SpaceArtilleryComponent, ComponentRemove>(OnComponentRemove);
	}
	//TODO detect and handle when weapon fires from gun system and when it does not

    private void OnComponentInit(EntityUid uid, SpaceArtilleryComponent component, ComponentInit args)
    {
		if(component.IsCoolantRequiredToFire == true)
        _itemSlotsSystem.AddItemSlot(uid, SpaceArtilleryComponent.CoolantSlotSlotId, component.CoolantSlot);
    }

    private void OnComponentRemove(EntityUid uid, SpaceArtilleryComponent component, ComponentRemove args)
    {
        _itemSlotsSystem.RemoveItemSlot(uid, component.CoolantSlot);
    }


	private void OnSignalReceived(EntityUid uid, SpaceArtilleryComponent component, ref SignalReceivedEvent args)
	{
		if(component.IsPowered == true || component.IsPowerRequiredForSignal == false)
		{
			//Sawmill.Info($"Space Artillery has been pinged. Entity: {ToPrettyString(uid)}");
			if (args.Port == component.SpaceArtilleryFirePort && component.IsArmed == true)
			{
				if(TryComp<BatteryComponent>(uid, out var battery))
				{
					if((component.IsPowered == true && battery.Charge >= component.PowerUseActive) || component.IsPowerRequiredToFire == false)
					{
						if((component.IsCoolantRequiredToFire == true && component.CoolantStored >= 1) || component.IsCoolantRequiredToFire == false)
						{
							TryFireArtillery(uid, component, battery);
						}
					}
				}
			}
			if (args.Port == component.SpaceArtillerySafetyPort)
			{
				if (TryComp<CombatModeComponent>(uid, out var combat))
				{
					if(combat.IsInCombatMode == false && combat.IsInCombatMode != null)
					{
						_combat.SetInCombatMode(uid, true, combat);
						component.IsArmed = true;
					}
					else
					{
						_combat.SetInCombatMode(uid, false, combat);
						component.IsArmed = false;
					}
				}
			}
		}
	}
	
	private void OnBuckleChange(EntityUid uid, SpaceArtilleryComponent component, ref BuckleChangeEvent args)
    {
        // Once Gunner buckles
        if (args.Buckling)
        {

            // Update actions

            if (TryComp<ActionsComponent>(args.BuckledEntity, out var actions))
            {
                _actionsSystem.AddAction(args.BuckledEntity, ref component.FireActionEntity, component.FireAction, uid, actions);
            }
            return;
        }

        // Once gunner unbuckles
        // Clean up actions 
        _actionsSystem.RemoveProvidedActions(args.BuckledEntity, uid);

    }
	
    /// <summary>
    /// This fires when the gunner presses the fire action
    /// </summary>
	
    private void OnFireAction(EntityUid uid, SpaceArtilleryComponent component, FireActionEvent args)
    {
		if((component.IsPowered == true || component.IsPowerRequiredForMount == false) && component.IsArmed == true)
		{
			if(TryComp<BatteryComponent>(uid, out var battery))
			{
				if((component.IsPowered == true && battery.Charge >= component.PowerUseActive) || component.IsPowerRequiredToFire == false)
				{
					if((component.IsCoolantRequiredToFire == true && component.CoolantStored >= 1) || component.IsCoolantRequiredToFire == false)
					{
						if (args.Handled)
							return;

						TryFireArtillery(uid, component, battery);

						args.Handled = true;
					}
				}
			}
		}
    }


	private void OnApcChanged(EntityUid uid, SpaceArtilleryComponent component, ref PowerChangedEvent args){
		
		if(TryComp<BatterySelfRechargerComponent>(uid, out var batteryCharger)){
		
			if (args.Powered)
			{
				component.IsCharging = true;
				batteryCharger.AutoRecharge = true;
				batteryCharger.AutoRechargeRate = component.PowerChargeRate;
			}
			else
			{
				component.IsCharging = false;
				batteryCharger.AutoRecharge = true;
				batteryCharger.AutoRechargeRate = component.PowerUsePassive * -1;
				
				if(TryComp<BatteryComponent>(uid, out var battery))
					battery.CurrentCharge -= component.PowerUsePassive; //It is done so that BatterySelfRecharger will get start operating instead of being blocked by fully charged battery
			}
		}
	}
	
	
	private void OnBatteryChargeChanged(EntityUid uid, SpaceArtilleryComponent component, ref ChargeChangedEvent args){
		
		if(args.Charge > 0)
		{
			component.IsPowered = true;
		}
		else
		{
			component.IsPowered = false;
		}
		
		if(TryComp<ApcPowerReceiverComponent>(uid, out var apcPowerReceiver) && TryComp<BatteryComponent>(uid, out var battery))
		{
			if(battery.IsFullyCharged == false)
			{
				apcPowerReceiver.Load = component.PowerUsePassive + component.PowerChargeRate;
			}
			else
			{
				apcPowerReceiver.Load = component.PowerUsePassive;
			}
		}
	}
	
	private void OnCoolantSlotChanged(EntityUid uid, SpaceArtilleryComponent component, ContainerModifiedMessage args)
    {
		GetInsertedCoolantAmount(component, out var storage);
		
		// validating the coolant slot was setup correctly in the yaml
        if (component.CoolantSlot.ContainerSlot is not BaseContainer coolantSlot)
        {
            return;
        }
		
		// validate stack prototypes
        if (!TryComp<StackComponent>(component.CoolantSlot.ContainerSlot.ContainedEntity, out var stackComponent) ||
            stackComponent.StackTypeId == null)
        {
            return;
        }
		
        // and then check them against the Armament's CoolantType
        if (_prototypeManager.Index<StackPrototype>(component.CoolantType) != _prototypeManager.Index<StackPrototype>(stackComponent.StackTypeId))
        {
            return;
        }
		
		var currentCoolant = component.CoolantStored;
		var maxCoolant = component.MaxCoolantStored;
		var totalCoolantPresent = currentCoolant + storage;
		if(totalCoolantPresent > maxCoolant)
		{
			var remainingCoolant = totalCoolantPresent - maxCoolant;
			stackComponent.Count = remainingCoolant;
			stackComponent.UiUpdateNeeded = true;
			component.CoolantStored = maxCoolant;
		}
		else
		{
			component.CoolantStored = totalCoolantPresent;
			 _containerSystem.CleanContainer(coolantSlot);
		}
	}
	
	private void TryFireArtillery(EntityUid uid, SpaceArtilleryComponent component, BatteryComponent battery)
	{
		var xform = Transform(uid);
		
		if (!_gun.TryGetGun(uid, out var gunUid, out var gun))
		{
			return;
		}
		
		if(TryComp<TransformComponent>(uid, out var transformComponent)){
			
			if(component.IsPowerRequiredToFire == true)
			{
				battery.CurrentCharge -= component.PowerUseActive;
			}
			if(component.IsCoolantRequiredToFire == true)
			{
				component.CoolantStored -= 1;
			}
			
			var worldPosX = transformComponent.WorldPosition.X;
			var worldPosY = transformComponent.WorldPosition.Y;
			var worldRot = transformComponent.WorldRotation+rotOffset;
			var targetSpot = new Vector2(worldPosX - distance * (float) Math.Sin(worldRot), worldPosY + distance * (float) Math.Cos(worldRot));
			
			var _gridUid = transformComponent.GridUid;
			
			EntityCoordinates targetCordinates;
			targetCordinates = new EntityCoordinates(xform.MapUid!.Value, targetSpot);
			
			_gun.AttemptShoot(uid, gunUid, gun, targetCordinates);
			
			
			///Space Recoil is handled here
			if(transformComponent.Anchored == true)
			{
				if(TryComp<PhysicsComponent>(_gridUid, out var gridPhysicsComponent) && _gridUid is {Valid :true} gridUid)
				{
					var gridMass = gridPhysicsComponent.FixturesMass;
					var linearVelocityLimitGrid = component.VelocityLimitRecoilGrid;
					var oldLinearVelocity = gridPhysicsComponent.LinearVelocity;
					var oldAngularVelocity = gridPhysicsComponent.AngularVelocity;
					
					if(oldLinearVelocity.X >= linearVelocityLimitGrid || oldLinearVelocity.Y >= linearVelocityLimitGrid)
						return;
					
					var targetSpotRecoil = new Vector2(worldPosX - component.LinearRecoilGrid * (float) Math.Sin(worldRot), worldPosY + component.LinearRecoilGrid * (float) Math.Cos(worldRot));
					var recoilX = (worldPosX - targetSpotRecoil.X);
					var recoilY = (worldPosY - targetSpotRecoil.Y);
					//TODO fix the velocity limit to work more accuretly
					var newLinearVelocity = new Vector2(MathF.Min(oldLinearVelocity.X + (recoilX/gridMass),linearVelocityLimitGrid), MathF.Min(oldLinearVelocity.Y + (recoilY/gridMass),linearVelocityLimitGrid));
					
					var randomAngularInstability = _random.Next((int) -component.AngularInstabilityGrid, (int) component.AngularInstabilityGrid);
					var newAngularVelocity = oldAngularVelocity + (randomAngularInstability/gridMass);
					
					
					_physicsSystem.SetLinearVelocity(gridUid, newLinearVelocity);
					_physicsSystem.SetAngularVelocity(gridUid, newAngularVelocity);
					
					Sawmill.Info($"Space Artillery recoil. RecoilX: {recoilX}  RecoilY: {recoilY}  Instability: {randomAngularInstability}");
					Sawmill.Info($"Space Artillery recoil. LinearVelocityX: {newLinearVelocity.X}/{oldLinearVelocity.X}  LinearVelocityY: {newLinearVelocity.Y}/{oldLinearVelocity.Y}  AngularInstability: {newAngularVelocity}/{oldAngularVelocity}");
				}
			} 
			else
			{ 
				if(TryComp<PhysicsComponent>(uid, out var weaponPhysicsComponent) && uid is {Valid :true} weaponUid)
				{
					var weaponMass = weaponPhysicsComponent.FixturesMass;
					var linearVelocityLimitWeapon = component.VelocityLimitRecoilWeapon;
					var oldLinearVelocity = weaponPhysicsComponent.LinearVelocity;
					var oldAngularVelocity = weaponPhysicsComponent.AngularVelocity;
					
					if(oldLinearVelocity.X >= linearVelocityLimitWeapon || oldLinearVelocity.Y >= linearVelocityLimitWeapon)
						return;
					
					var targetSpotRecoil = new Vector2(worldPosX - component.LinearRecoilWeapon * (float) Math.Sin(worldRot), worldPosY + component.LinearRecoilWeapon * (float) Math.Cos(worldRot));
					var recoilX = (worldPosX - targetSpotRecoil.X);
					var recoilY = (worldPosY - targetSpotRecoil.Y);
					var newLinearVelocity = new Vector2(MathF.Min(oldLinearVelocity.X + (recoilX/weaponMass),linearVelocityLimitWeapon), MathF.Min(oldLinearVelocity.Y + (recoilY/weaponMass),linearVelocityLimitWeapon));
					
					var randomAngularInstability = _random.Next((int) -component.AngularInstabilityWeapon, (int) component.AngularInstabilityWeapon);
					var newAngularVelocity = oldAngularVelocity + (randomAngularInstability/weaponMass);
					
					
					_physicsSystem.SetLinearVelocity(uid, newLinearVelocity);
					_physicsSystem.SetAngularVelocity(uid, newAngularVelocity);
				}
			}
		}
	}
	
    private void GetInsertedCoolantAmount(SpaceArtilleryComponent component, out int amount)
    {
        amount = 0;
        var coolantEntity = component.CoolantSlot.ContainerSlot?.ContainedEntity;

        if (!TryComp<StackComponent>(coolantEntity, out var coolantStack) ||
            coolantStack.StackTypeId != component.CoolantType)
        {
            return;
        }

        amount = coolantStack.Count;
        return;
    }
}