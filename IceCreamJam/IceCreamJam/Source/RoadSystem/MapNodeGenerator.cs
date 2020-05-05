using Microsoft.Xna.Framework;
using Nez.Tiled;
using System;
using System.Collections.Generic;

namespace IceCreamJam.RoadSystem {
    static class MapNodeGenerator {
        
        public static List<Node> GenerateNodes(TmxMap map) {
            var layer = map.GetLayer<TmxLayer>(Constants.TiledLayerBackground);

            List<Node> nodes = new List<Node>();
            foreach(TmxLayerTile t in layer.Tiles) {
                if(t == null)
                    continue;

                t.TilesetTile.Properties.TryGetValue(Constants.TiledPropertyID, out var value);

                if(value == Constants.TiledIDIntersection)
                    nodes.Add(new Node(map.TileToWorldPosition(t.Position) + new Vector2(Constants.TiledCellSize/2), t.Position.ToPoint()));
            }

            return ConnectNodes(nodes, layer);
        }

        private static List<Node> ConnectNodes(List<Node> nodes, TmxLayer layer) {
            foreach(Node a in nodes) {
                foreach(Node b in nodes) {
                    if(a.tilePosition == b.tilePosition || a.IsConnectedTo(b))
                        continue;

                    if(IsConnected(a, b, layer)) {
                        a.ConnectTo(b);
                        b.ConnectTo(a);
                    }
                }
            }

            return nodes;
        }

        // Check every tile between the two intersections
        // If blocked by grass, sidewalk, or another intersection, not connected
        private static bool IsConnected(Node a, Node b, TmxLayer layer) {
            if(a.tilePosition.X == b.tilePosition.X) {
                // Vertically aligned
                var dir = Math.Sign(b.tilePosition.Y - a.tilePosition.Y);
                for(int y = a.tilePosition.Y + dir; y != b.tilePosition.Y; y += dir) {
                    var t = layer.GetTile(a.tilePosition.X, y);

                    t.TilesetTile.Properties.TryGetValue(Constants.TiledPropertyID, out var value);

                    if(value != Constants.TiledIDRoad)
                        return false; 
                }
                return true;
            } else if(a.tilePosition.Y == b.tilePosition.Y) {
                // Horizontally aligned
                var dir = Math.Sign(b.tilePosition.X - a.tilePosition.X);

                for(int x = a.tilePosition.X + dir; x != b.tilePosition.X; x += dir) {
                    var t = layer.GetTile(x, a.tilePosition.Y);

                    t.TilesetTile.Properties.TryGetValue(Constants.TiledPropertyID, out var value);

                    if(value != Constants.TiledIDRoad)
                        return false;
                }
                return true;
            }
            return false;
        }
    }
}
