using IceCreamJam.RoadSystem;
using Nez;
using System.Collections.Generic;

namespace IceCreamJam.Components {
    class VehiclePathfindingComponent : Component, IUpdatable {
        public List<Node> path;
        public Node currentNode;

        private int nodesTraveled;
        public bool arrived;

        public static RoadSystemComponent roadSystem;
        private bool reevaluate;

        public int CurrentNodeIndex => path.IndexOf(currentNode);

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            path = new List<Node>();

            roadSystem = Entity.Scene.GetSceneComponent<RoadSystemComponent>();
            roadSystem.OnTargetChange += () => reevaluate = true;

            ReevaluateGraph();
        }

        private void ReevaluateGraph() {
            reevaluate = false;
            arrived = false;

            var start = roadSystem.GetNodeClosestTo(Entity);
            path = roadSystem.graph.Search(start, roadSystem.truckTargetNode);

            // TODO: Find a way to stop vehicles from turning around to "touch" their closest node before continuing.
            // If the direction from start to node 2 is opposite to the direction from entity's position to the start, skip start node.
            // Otherwise, keep start node.

            //newPath.Remove(start);

            nodesTraveled = 0;
            currentNode = TargetNextNode();
        }

        private Node TargetNextNode() {
            if(nodesTraveled == path.Count) {
                arrived = true;
                return currentNode;
            }

            return path[nodesTraveled++];
        }

        public bool IsFirstNode() => CurrentNodeIndex == 0;
        public bool IsLastNode() => CurrentNodeIndex == path.Count - 1;
        public Node GetNextNode() => path[CurrentNodeIndex + 1];
        public Node GetPreviousNode() => path[CurrentNodeIndex - 1];

        public void ArriveAtNode() {
            currentNode = TargetNextNode();
        }

        public void Update() {
            if(reevaluate)
                ReevaluateGraph();
        }
    }
}
