using IceCreamJam.Components;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IceCreamJam.RoadSystem {
    class Node : Entity {
        public Dictionary<Node, Connection> connections;

        public Point tilePosition;
        public RoadSystemComponent roadSystem;

        public Queue<VehicleMoveComponent> crossOrder;

        public Node(Vector2 position, Point tilePosition) {
            this.Position = position;
            this.tilePosition = tilePosition;
            this.Name = $"Node[{tilePosition.X}-{tilePosition.Y}]";

            connections = new Dictionary<Node, Connection>();
            crossOrder = new Queue<VehicleMoveComponent>();
        }

        public void QueueVehicle(VehicleMoveComponent vehicle) {
            if(crossOrder.Count == 0)
                vehicle.StartCrossing();

            crossOrder.Enqueue(vehicle);
            vehicle.OnCrossingFinished += DequeueVehicle;
        }

        private void DequeueVehicle() {
            var vehicle = crossOrder.Dequeue();
            vehicle.OnCrossingFinished -= DequeueVehicle;
            
            if(crossOrder.Count != 0)
                crossOrder.Peek().StartCrossing();
        }
        
        public Node GetRandomConnectedNode() {
            return connections.ElementAt(Nez.Random.NextInt(connections.Count - 1)).Key;
        }

        public Node GetRandomConnectedNode(Node previous) {
            var withoutPrevious = connections.Where(kvp => kvp.Key.Position != previous.Position).ToList();
            return withoutPrevious.ElementAt(Nez.Random.NextInt(withoutPrevious.Count)).Key;
        }

        public Vector2 GetOffsetBefore(Node previous) {
            var laneOffset = previous.GetDirectionTo(this).RotateClockwise(2).ToVector2() * 32;
            var intersection = GetDirectionTo(previous).ToVector2() * 75;
            return laneOffset + intersection;
        }

        public Vector2 GetOffsetAfter(Node next) {
            var laneOffset = GetDirectionTo(next).RotateClockwise(2).ToVector2() * 32;
            var intersection = GetDirectionTo(next).ToVector2() * 75;
            return laneOffset + intersection;
        }

        public void ConnectTo(Node node) {
            if(connections.ContainsKey(node))
                return;
            connections.Add(node, new Connection(node, GetDirectionTo(node), GetConnectedDistance(node)));
        }

        public bool IsConnectedTo(Node other) => connections.Where(kvp => kvp.Key == other).Any();
        public Direction8 GetDirectionTo(Node other) => Direction8Ext.FromVector2(other.Position - Position);

        private float GetConnectedDistance(Node node) {
            if(node.Position.X == Position.X)
                return Math.Abs(node.Position.Y - Position.Y);

            if(node.Position.Y == Position.Y)
                return Math.Abs(node.Position.X - Position.X);

            throw new ArgumentException("Given node isn't connected!");
        }

        public override void DebugRender(Batcher batcher) {
            base.DebugRender(batcher);

            foreach(Connection c in connections.Values)
                batcher.DrawLine(Position, Position + Direction8Ext.ToVector2(c.direction) * 50, Color.Chartreuse, 5);

            if(roadSystem.truckTargetNode == this)
                batcher.DrawHollowRect(new Rectangle((int)Position.X - 25, (int)Position.Y - 25, 50, 50), Color.White, 5);
        }

        public override void Update() {
            base.Update();
        }
    }

    struct Connection {
        public readonly Node node;
        public readonly Direction8 direction;
        public readonly float distance;

        public Connection(Node connected, Direction8 direction, float distance) {
            this.node = connected;
            this.direction = direction;
            this.distance = distance;
        }
    }
}
