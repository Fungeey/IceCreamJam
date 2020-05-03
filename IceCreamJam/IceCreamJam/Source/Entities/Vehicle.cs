using IceCreamJam.Components;
using Nez;

namespace IceCreamJam.Entities {
    abstract class Vehicle : Entity {

        public VehicleMoveComponent moveComponent;

        public override void OnAddedToScene() {
            base.OnAddedToScene();
            moveComponent = AddComponent(new VehicleMoveComponent());
        }
    }
}
