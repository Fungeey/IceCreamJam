using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IceCreamJam.RoadSystem {
    class Node {
        public Dictionary<Node, Connection> connections;

        public Vector2 position;
        public Point tilePosition;

        public Node(Vector2 position, Point tilePosition) {
            this.position = position;
            this.tilePosition = tilePosition;

            connections = new Dictionary<Node, Connection>();
        }

        public void Add(Node node) {
            if(connections.ContainsKey(node))
                return;
            connections.Add(node, new Connection(node, GetDirectionTo(node), GetConnectedDistance(node)));
        }

        public bool IsConnectedTo(Node other) => connections.Where(kvp => kvp.Key == other).Any();
        public bool IsRight(Node previous, Node next) => Direction8Ext.RotateClockwise(previous.GetDirectionTo(this), 2) == GetDirectionTo(next);
        public bool IsLeft(Node previous, Node next) => Direction8Ext.RotateClockwise(previous.GetDirectionTo(this), 6) == GetDirectionTo(next);
        public bool IsStraight(Node previous, Node next) => previous.GetDirectionTo(this) == GetDirectionTo(next);
        public Direction8 GetDirectionTo(Node other) => Direction8Ext.FromVector2(other.position - position);

        private float GetConnectedDistance(Node node) {
            if(node.position.X == position.X)
                return Math.Abs(node.position.Y - position.Y);

            if(node.position.Y == position.Y)
                return Math.Abs(node.position.X - position.X);

            throw new ArgumentException("Given node isn't connected!");
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
