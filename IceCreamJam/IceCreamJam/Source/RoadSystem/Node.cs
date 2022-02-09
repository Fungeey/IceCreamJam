using IceCreamJam.Source.RoadSystem;
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

        public Queue<CivilianCarStateMachine> crossOrder;

        public Node(Vector2 position, Point tilePosition) {
            this.Position = position;
            this.tilePosition = tilePosition;
            this.Name = $"Node[{tilePosition.X}-{tilePosition.Y}]";

            connections = new Dictionary<Node, Connection>();
            crossOrder = new Queue<CivilianCarStateMachine>();
        }

        public void QueueVehicle(CivilianCarStateMachine car) {
            if(crossOrder.Count == 0)
                car.StartTurn();

            crossOrder.Enqueue(car);
            car.OnTurnFinished += DequeueVehicle;
        }

        private void DequeueVehicle() {
            var car = crossOrder.Dequeue();
            car.OnTurnFinished -= DequeueVehicle;
            
            if(crossOrder.Count != 0)
                crossOrder.Peek().StartTurn();
        }

        // Unused but might come in handy later
        private RectangleF GetIntersectionArea(Direction8 dir) {
            if(!dir.IsOrthogonal())
                return new RectangleF();

            var pos = Position + dir.ToNormalizedVector2(48);
            var size = dir.IsVertical() ? new Vector2(64, 160) : new Vector2(160, 64);
            return new RectangleF(pos, size);
        }

        public Node GetRandomConnectedNode() {
            return connections.ElementAt(Nez.Random.NextInt(connections.Count - 1)).Key;
        }

        // Get a random node connected to this, excluding the previous node
        public Node GetRandomConnectedNode(Node previous) {
            var withoutPrevious = connections.Where(kvp => kvp.Key.Position != previous.Position).ToList();
            return withoutPrevious.ElementAt(Nez.Random.NextInt(withoutPrevious.Count)).Key;
        }

        public static Vector2 GetOffsetBefore(Vector2 from, Vector2 to) {
            var laneOffset = GetDirectionTo(from, to).RotateClockwise(2).ToVector2() * 32;
            var intersection = GetDirectionTo(to, from).ToVector2() * 75;
            return laneOffset + intersection;
        }

        public static Vector2 GetOffsetAfter(Vector2 from, Vector2 to) {
            var laneOffset = GetDirectionTo(from, to).RotateClockwise(2).ToVector2() * 32;
            var intersection = GetDirectionTo(from, to).ToVector2() * 75;
            return laneOffset + intersection;
        }

        public static Vector2 GetOffsetBefore(Node from, Node to) => GetOffsetBefore(from.Position, to.Position);
        public static Vector2 GetOffsetAfter(Node from, Node to) => GetOffsetAfter(from.Position, to.Position);
        public static Direction8 GetDirectionTo(Vector2 a, Vector2 b) => Direction8Ext.FromVector2(a - b);

        public void ConnectTo(Node node) {
            if(connections.ContainsKey(node))
                return;
            connections.Add(node, new Connection(node, GetDirectionTo(Position, node.Position), GetConnectedDistance(node)));
        }

        public bool IsConnectedTo(Node other) => connections.Where(kvp => kvp.Key == other).Any();

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
