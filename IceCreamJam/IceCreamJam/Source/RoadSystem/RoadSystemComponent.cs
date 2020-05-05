using IceCreamJam.Entities;
using IceCreamJam.Scenes;
using IceCreamJam.Tiled;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;

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
            mapLoader.OnLoad += RegisterNodes;

            graph = new WeightedRoadGraph();
        }

        private void RegisterNodes() {
            nodes = MapNodeGenerator.GenerateNodes(mapLoader.tiledMap.map);
            foreach(Node node in nodes) {
                Scene.AddEntity(node);
                node.roadSystem = this;
            }
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

            if(Input.LeftMouseButtonPressed) {
                Scene.AddEntity(new TestVehicle() { Position = GetRandomEmptyNode().Position });
            }
        }

        private void UpdateTarget() {
            var closestNode = GetNodeClosestTo(Scene.FindEntity("Crosshair"));
            if(truckTargetNode != closestNode)
                OnTargetChange?.Invoke();

            truckTargetNode = closestNode;
        }

        public Node GetNodeClosestTo(Entity target) {
            float minDistance = float.MaxValue;
            Node closestNode = null;
            foreach(Node node in nodes) {
                var dis = Vector2.DistanceSquared(node.Position, target.Position);
                if(dis < minDistance) {
                    minDistance = dis;
                    closestNode = node;
                }
            }

            return closestNode;
        }

        public Node GetRandomNode () {
            return nodes[Nez.Random.NextInt(nodes.Count)];
        }

        public Node GetRandomEmptyNode() {
            var empty = nodes.Where(n => n.crossOrder.Count == 0).ToList();
            return empty[Nez.Random.NextInt(empty.Count)];
        }
    }
}
