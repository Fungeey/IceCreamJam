using IceCreamJam.Entities;
using IceCreamJam.Scenes;
using IceCreamJam.Tiled;
using Microsoft.Xna.Framework;
using Nez;
using System.Collections.Generic;

namespace IceCreamJam.RoadSystem {
    class RoadSystemComponent : SceneComponent {

        public List<Node> nodes;
        private TilemapLoader mapLoader;
        private Truck truck;

        public RoadSystemComponent() { }

        public override void OnEnabled() {
            base.OnEnabled();

            mapLoader = Scene.GetSceneComponent<TilemapLoader>();
            mapLoader.OnLoad += () => nodes = MapNodeGenerator.GenerateNodes(mapLoader.tiledMap.map);
        }

        public override void Update() {
            base.Update();

            if(truck == null)
                truck = (Scene as MainScene).truck;
        }

        public Node TruckTargetNode() {
            if(truck == null)
                return null;
            
            float minDistance = float.MaxValue;
            Node closestNode = nodes[0];
            foreach(Node node in nodes) {
                var dis = Vector2.DistanceSquared(node.position, truck.Position);
                if(dis < minDistance) {
                    minDistance = dis;
                    closestNode = node;
                }
            }

            return closestNode;
        }
    }
}
