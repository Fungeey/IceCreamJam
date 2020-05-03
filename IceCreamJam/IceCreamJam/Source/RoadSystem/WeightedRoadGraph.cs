using Nez.AI.Pathfinding;
using System.Collections.Generic;

namespace IceCreamJam.RoadSystem {
    class WeightedRoadGraph : IWeightedGraph<Node> {
        public int Cost(Node from, Node to) {
            from.connections.TryGetValue(to, out var connection);
            return (int)connection.distance;
        }

        public IEnumerable<Node> GetNeighbors(Node node) {
            return node.connections.Keys;
        }

        public List<Node> Search(Node start, Node goal) => WeightedPathfinder.Search(this, start, goal);
    }
}
