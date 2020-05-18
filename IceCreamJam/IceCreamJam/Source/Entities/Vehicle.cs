using IceCreamJam.Components;
using IceCreamJam.Components.Vehicles;
using Nez;

namespace IceCreamJam.Entities {
    abstract class Vehicle : Entity {

        public VehicleMoveComponent moveComponent;
        public VehicleWanderComponent pathfindingComponent;

        public override void OnAddedToScene() {
            base.OnAddedToScene();
            moveComponent = AddComponent(new VehicleMoveComponent());
            pathfindingComponent = AddComponent(new VehicleWanderComponent());
        }
    }
}
