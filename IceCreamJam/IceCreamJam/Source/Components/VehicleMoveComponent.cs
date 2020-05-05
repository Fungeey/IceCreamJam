using IceCreamJam.RoadSystem;
using Microsoft.Xna.Framework;
using Nez;
using System;

namespace IceCreamJam.Components {
    class VehicleMoveComponent : Component, IUpdatable {

        private Mover mover;
        private VehiclePathfindingComponent pathfinding;
        private float speed = 320;
        private Color color;

        private Direction8 currentDirection;

        private bool IsTurning;
        private bool IsWaiting;
        public event Action OnCrossingFinished;
        // State machine: Move towards next node, rotating around an intersection

        private Vector2 TargetPosition => pathfinding.currentNode.Position + GetOffset(pathfinding.currentNode);

        public override void OnAddedToEntity() {
            base.OnAddedToEntity();
            mover = Entity.AddComponent(new Mover());
            pathfinding = Entity.GetComponent<VehiclePathfindingComponent>();
        }

        public void Update() {
            if(pathfinding.arrived)
                return;

            if(IsTurning)
                DoTurn();
            else if(!IsWaiting)
                ApproachNextNode();
        }
        
        private void DoTurn() {
            if(pathfinding.IsLastNode())
                return;

            var offset = pathfinding.currentNode.GetOffsetAfter(pathfinding.NextNode);
            
            var turnTarget = pathfinding.currentNode.Position + offset;
            
            var distance = (turnTarget - Entity.Position);
            var direction = distance.Normalized();
            currentDirection = Direction8Ext.FromVector2(distance);
            
            if(!IsBlocked())
                mover.Move(direction * Time.DeltaTime * speed, out var result);
            
            if(distance.Length() < 10) {
                // Finish turn
                IsTurning = false;
                IsWaiting = false;
                OnCrossingFinished?.Invoke();
            }
        }

        private void ApproachNextNode() {
            var distance = (TargetPosition - Entity.Position);
            var direction = distance.Normalized();
            currentDirection = Direction8Ext.FromVector2(distance);

            if(!IsBlocked())
                mover.Move(direction.Normalized() * Time.DeltaTime * speed + GetAlignVector(TargetPosition), out var result);

            if(distance.Length() < 10) {
                // Arrived at intersection. Await for signal from Node to start.
                // Only turn if there is another node.
                if(!pathfinding.IsLastNode()) {
                    pathfinding.currentNode.QueueVehicle(this);
                    IsWaiting = true;
                } else {
                    pathfinding.arrived = true;
                }
            }
        }

        public void StartCrossing() {
            IsTurning = true;
            IsWaiting = false;
        }

        private bool IsBlocked() {
            var hit = Physics.Linecast(Entity.Position, Entity.Position + currentDirection.ToNormalizedVector2(50));
            return hit.Collider != null;
        }

        private Vector2 GetAlignVector(Vector2 target) {
            // Move to be in line with the current target position
            // If moving vertically, adjust horizontally to be in line.
            // If moving horizontally, adjust vertically to be in line.
            return currentDirection.IsVertical() ? new Vector2(target.X - Entity.Position.X, 0) : new Vector2(0, target.Y - Entity.Position.Y);
        }

        private Vector2 GetOffset(Node node) {
            if(!pathfinding.IsFirstNode())
                return node.GetOffsetBefore(pathfinding.PreviousNode);
            return currentDirection.RotateClockwise(2).ToVector2() * 32;
        }

        public override void DebugRender(Batcher batcher) {
            base.DebugRender(batcher);

            batcher.DrawLine(Entity.Position, TargetPosition, Color.Red, 2);
            batcher.DrawLine(Entity.Position, Entity.Position + currentDirection.ToNormalizedVector2(50), Color.Blue, 3);
            batcher.DrawCircle(pathfinding.currentNode.Position, 50, Color.Black, 3);

            if(IsTurning) {
                //var target = pathfinding.currentNode.Position + pathfinding.currentNode.GetOffsetAfter(pathfinding.NextNode);
                //batcher.DrawLine(pathfinding.currentNode.Position, target, Color.Yellow, 10);

                var rect = new Rectangle(Entity.Position.ToPoint() - new Point(16, 16), new Point(32, 32));
                batcher.DrawHollowRect(rect, Color.Turquoise, 5);
            }

            if(IsWaiting) {
                var rect = new Rectangle(Entity.Position.ToPoint() - new Point(16, 16), new Point(32, 32));
                batcher.DrawHollowRect(rect, Color.Green, 5);
            }
        }
    }
}
