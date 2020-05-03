using IceCreamJam.RoadSystem;
using IceCreamJam.Scenes;
using Microsoft.Xna.Framework;
using Nez;
using System.Collections.Generic;

namespace IceCreamJam.Components {
    class VehicleMoveComponent : Component, IUpdatable {

        private Mover mover;
        public List<Node> path;
        public Node currentTarget;
        private int nodesTraveled;
        public bool arrived;

        // State machine: Move towards next node, rotating around an intersection

        public static RoadSystemComponent roadSystem;
        private bool reevaluate;

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            mover = Entity.AddComponent(new Mover());

            path = new List<Node>();

            roadSystem = Entity.Scene.GetSceneComponent<RoadSystemComponent>();
            roadSystem.OnTargetChange += () => reevaluate = true;

            ReevaluateGraph();
        }

        public void Move(Vector2 vec) {
            mover.Move(vec, out var result);
        }

        public Node GetNextNode() {
            if(nodesTraveled == path.Count) {
                arrived = true;
                return currentTarget;
            }

            return path[nodesTraveled++];
        }

        public void Update() {
            if(reevaluate)
                ReevaluateGraph();

            if(arrived)
                return;

            var distance = (currentTarget.position - Entity.Position);
            var direction = distance.Normalized();
            Move(direction * Time.DeltaTime * 180);

            if(distance.Length() < 50)
                currentTarget = GetNextNode();
        }

        private void ReevaluateGraph() {
            reevaluate = false;
            arrived = false;

            var start = roadSystem.GetNodeClosestTo(Entity);
            path.Clear();
            path.AddRange(roadSystem.graph.Search(start, roadSystem.truckTargetNode));

            nodesTraveled = 0;
            currentTarget = GetNextNode();
        }

        public override void DebugRender(Batcher batcher) {
            base.DebugRender(batcher);

            for(int i = nodesTraveled; i < path.Count; i++) { 
                var node = path[i];
                batcher.DrawLine(Entity.Position, node.position, Color.Red);
            }
        }
    }
}
