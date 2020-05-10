using Nez.AI.Pathfinding;
using System.Collections.Generic;
using System.Linq;

namespace IceCreamJam.RoadSystem {
    class WeightedRoadGraph : IWeightedGraph<Node> {

        private Node previousNode, startNode;

        public int Cost(Node from, Node to) {
            from.connections.TryGetValue(to, out var connection);
            return (int)connection.distance;
        }

        public IEnumerable<Node> GetNeighbors(Node node) {
            if(startNode == node)
                return node.connections.Where(kvp => kvp.Key == previousNode).Select(kvp => kvp.Key);

            return node.connections.Keys;
        }

        public List<Node> Search(Node start, Node goal, Node previous) {
            startNode = start;
            previousNode = previous;
            return WeightedPathfinder.Search(this, start, goal);
        }

        public List<Node> Search(Node start, Node goal) => WeightedPathfinder.Search(this, start, goal);
    }
}
