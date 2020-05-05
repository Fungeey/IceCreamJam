using IceCreamJam.RoadSystem;
using Microsoft.Xna.Framework;
using Nez;
using System.Collections.Generic;

namespace IceCreamJam.Components {
    class VehiclePathfindingComponent : Component, IUpdatable {
        public List<Node> path;
        public Node currentNode;
        public Color color;

        private VehicleMoveComponent moveComponent;

        private int nodesTraveled;
        public bool arrived;

        public static RoadSystemComponent roadSystem;
        private bool reevaluate;

        public int CurrentNodeIndex => path.IndexOf(currentNode);

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();

            path = new List<Node>();

            moveComponent = Entity.GetComponent<VehicleMoveComponent>();
            moveComponent.OnCrossingFinished += () => currentNode = TargetNextNode();

            roadSystem = Entity.Scene.GetSceneComponent<RoadSystemComponent>();
            //roadSystem.OnTargetChange += () => reevaluate = true;
            //Entity.Position = roadSystem.GetRandomNodePosition();

            ReevaluateGraph();
        }

        private void ReevaluateGraph() {
            reevaluate = false;
            arrived = false;

            var start = roadSystem.GetNodeClosestTo(Entity);
            path = roadSystem.graph.Search(start, roadSystem.GetRandomNode());
            color = Random.NextColor();

            // TODO: Find a way to stop vehicles from turning around to "touch" their closest node before continuing.
            // When is this behavior needed? when is it useless?

            nodesTraveled = 0;
            currentNode = TargetNextNode();
        }

        public Node TargetNextNode() {
            if(nodesTraveled == path.Count) {
                arrived = true;
                reevaluate = true;
                return currentNode;
            }

            return path[nodesTraveled++];
        }

        public bool IsFirstNode() => CurrentNodeIndex == 0;
        public bool IsLastNode() => CurrentNodeIndex == path.Count - 1;
        public Node NextNode => path[CurrentNodeIndex + 1];
        public Node PreviousNode => path[CurrentNodeIndex - 1];

        //public void FinishTurn() {
        //    currentNode = TargetNextNode();
        //}

        public void Update() {
            if(reevaluate || arrived)
                ReevaluateGraph();
        }

        public override void DebugRender(Batcher batcher) {
            base.DebugRender(batcher);

            batcher.DrawCircle(Entity.Position, 100, color, 15);
        }
    }
}
