using IceCreamJam.Entities;
using IceCreamJam.Scenes;
using IceCreamJam.Tiled;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;

namespace IceCreamJam.RoadSystem {
    class RoadSystemComponent : SceneComponent {

        public List<Node> nodes;
        private TilemapLoader mapLoader;
        private Truck truck;

        public WeightedRoadGraph graph;
        public Node truckTargetNode;
        public event Action OnTargetChange;

        public RoadSystemComponent() { }

        public override void OnEnabled() {
            base.OnEnabled();

            mapLoader = Scene.GetSceneComponent<TilemapLoader>();
            mapLoader.OnLoad += MapLoader_OnLoad;

            graph = new WeightedRoadGraph();
        }

        private void MapLoader_OnLoad() {
            nodes = MapNodeGenerator.GenerateNodes(mapLoader.tiledMap.map);
        }

        public override void Update() {
            base.Update();

            if(truck == null) {
                if((Scene as MainScene).truck != null) {
                    truck = (Scene as MainScene).truck;
                    UpdateTarget();
                }
                return;
            }

            UpdateTarget();
        }

        private void UpdateTarget() {
            var closestNode = GetNodeClosestTo(truck);
            if(truckTargetNode != closestNode)
                OnTargetChange?.Invoke();

            truckTargetNode = closestNode;
        }

        public Node GetNodeClosestTo(Entity target) {
            float minDistance = float.MaxValue;
            Node closestNode = null;
            foreach(Node node in nodes) {
                var dis = Vector2.DistanceSquared(node.position, target.Position);
                if(dis < minDistance) {
                    minDistance = dis;
                    closestNode = node;
                }
            }

            return closestNode;
        }
    }
}
