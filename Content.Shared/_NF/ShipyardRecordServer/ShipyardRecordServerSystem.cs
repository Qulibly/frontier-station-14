using Robust.Shared.Prototypes;
using Robust.Shared.Enums;

namespace Content.Shared.ShipyardRecordServer;

public sealed class ShipyardRecordServerSystem : EntitySystem
{
        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<ShipyardRecordsServerComponent, ComponentInit>(OnComponentInit);
        }

        /// <summary>
        ///     Initialise Shipyard Record List
        /// </summary>
        private void OnComponentInit(EntityUid uid, ShipyardRecordsServerComponent component, ComponentInit args)
        {
            var record = new ShipyardRecordsServerComponent.RecordEntry { VesselName = "aaaaa", VesselOwnerName = "a", VesselOwnerSpecies = "Weh", VesselOwnerGender = Gender.Epicene, VesselOwnerAge = 21, VesselOwnerFingerprints = "Nullified", VesselOwnerDNA = "GGGGGGGGG", VesselCategory = "Unknown", VesselClass = "Undetermined", VesselGroup = "no shipyard", VesselPrice = 0, VesselDescription = "It simply Doesnt Exist" };

        }
}
