using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace IceCreamJam.RoadSystem {
    class Node {
        public List<Connection> nodes;
        public Vector2 position;
        public Point tilePosition;

        public Node(Vector2 position, Point tilePosition) {
            this.position = position;
            this.tilePosition = tilePosition;

            nodes = new List<Connection>();
        }

        public void Add(Node node) {
            nodes.Add(new Connection(node, GetDirectionTo(node)));
        }

        public bool IsConnectedTo(Node other) => nodes.Select(c => c.node).Where(n => n == other).Any();
        public bool IsRight(Node previous, Node next) => Direction8Ext.RotateClockwise(previous.GetDirectionTo(this), 2) == GetDirectionTo(next);
        public bool IsLeft(Node previous, Node next) => Direction8Ext.RotateClockwise(previous.GetDirectionTo(this), 6) == GetDirectionTo(next);
        public bool IsStraight(Node previous, Node next) => previous.GetDirectionTo(this) == GetDirectionTo(next);
        public Direction8 GetDirectionTo(Node other) => Direction8Ext.FromVector2(other.position - position);

    }

    struct Connection {
        public readonly Node node;
        public readonly Direction8 direction;

        public Connection(Node connected, Direction8 direction) {
            this.node = connected;
            this.direction = direction;
        }
    }
}
