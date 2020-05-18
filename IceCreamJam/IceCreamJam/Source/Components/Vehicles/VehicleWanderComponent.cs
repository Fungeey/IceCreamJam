using IceCreamJam.RoadSystem;
using Nez;

namespace IceCreamJam.Components.Vehicles {

    /// <summary>
    /// A component to pilot a vehicle around aimlessly
    /// </summary>
    class VehicleWanderComponent : Component {
        public Node currentNode, nextNode, previousNode;

        private VehicleMoveComponent moveComponent;
        public static RoadSystemComponent roadSystem;

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            moveComponent = Entity.GetComponent<VehicleMoveComponent>();
            moveComponent.OnCrossingFinished += () => TargetNextNode();

            roadSystem = Entity.Scene.GetSceneComponent<RoadSystemComponent>();

            previousNode = roadSystem.GetNodeClosestTo(Entity);
            currentNode = previousNode.GetRandomConnectedNode();
            nextNode = currentNode.GetRandomConnectedNode(previousNode);

        }

        private void TargetNextNode() {
            var previous = currentNode;
            previousNode = currentNode;
            currentNode = nextNode;

            nextNode = currentNode.GetRandomConnectedNode(previous);
        }
    }
}
