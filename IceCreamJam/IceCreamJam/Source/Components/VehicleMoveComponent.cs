using IceCreamJam.RoadSystem;
using Microsoft.Xna.Framework;
using Nez;

namespace IceCreamJam.Components {
    class VehicleMoveComponent : Component, IUpdatable {

        private Mover mover;
        private VehiclePathfindingComponent pathfinding;

        private Direction8 currentDirection;
        
        // State machine: Move towards next node, rotating around an intersection

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            mover = Entity.AddComponent(new Mover());
            pathfinding = Entity.GetComponent<VehiclePathfindingComponent>();
        }

        public void Update() {
            if(pathfinding.arrived)
                return;

            var target = TargetPosition(pathfinding.currentNode);
            var distance = (target - Entity.Position);
            var direction = distance.Normalized();
            currentDirection = Direction8Ext.FromVector2(direction);

            var alignVector = GetAlignVector(target);
            var avoidVector = GetAvoidVector();

            if(!avoidVector)
                mover.Move((direction + alignVector).Normalized() * Time.DeltaTime * 100, out var result);

            if(distance.Length() < 10)
                pathfinding.ArriveAtNode();
        }

        private bool GetAvoidVector() {
            var hit = Physics.Linecast(Entity.Position, Entity.Position + currentDirection.ToNormalizedVector2(25));
            return hit.Collider != null;
        }

        private Vector2 GetAlignVector(Vector2 target) {
            // Move to be in line with the current target position
            // If moving vertically, adjust horizontally to be in line.
            // If moving horizontally, adjust vertically to be in line.

            var alignVector = new Vector2();
            if(currentDirection == Direction8.South || currentDirection == Direction8.North)
                alignVector = new Vector2((target.X - Entity.Position.X), 0);

            if(currentDirection == Direction8.West || currentDirection == Direction8.East)
                alignVector = new Vector2(0, (target.Y - Entity.Position.Y));

            return alignVector;
        }

        public Vector2 TargetPosition(Node node) {
            return node.position + GetOffset(node);
        }

        private Vector2 GetOffset(Node node) {
            var previousNodeOffset = pathfinding.IsFirstNode() ? Vector2.Zero : 
                pathfinding.GetPreviousNode().GetDirectionTo(node).RotateClockwise(2).ToVector2();
            var nextNodeOffset = pathfinding.IsLastNode() ? Vector2.Zero : 
                node.GetDirectionTo(pathfinding.GetNextNode()).RotateClockwise(2).ToVector2();              

            var offset = previousNodeOffset + nextNodeOffset;
            if(offset == Vector2.Zero)
                offset = currentDirection.RotateClockwise(2).ToVector2();

            return offset * 32;
        }

        public override void DebugRender(Batcher batcher) {
            base.DebugRender(batcher);

            batcher.DrawLine(Entity.Position, TargetPosition(pathfinding.currentNode), Color.Red, 2);
        }
    }
}
