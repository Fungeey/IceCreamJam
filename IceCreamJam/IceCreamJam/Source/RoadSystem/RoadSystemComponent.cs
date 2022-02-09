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

        private CivilianCarSpawnSystem carSpawner;
        private Action carSpawnerStart;

        public RoadSystemComponent() { }

        public override void OnEnabled() {
            base.OnEnabled();

            mapLoader = Scene.GetSceneComponent<TilemapLoader>();
            mapLoader.OnLoad += RegisterNodes;

            graph = new WeightedRoadGraph();

            mapLoader.OnLoad += carSpawnerStart = () => carSpawner.Start(mapLoader.carSpawnPoints);
            carSpawner = new CivilianCarSpawnSystem();
            Scene.AddSceneComponent(carSpawner);
        }

		public override void OnRemovedFromScene() {
			base.OnRemovedFromScene();
            mapLoader.OnLoad -= RegisterNodes;
            mapLoader.OnLoad -= carSpawnerStart;
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

            if(truck == null && ((MainScene)Scene).truck != null)
                truck = ((MainScene)Scene).truck;

            UpdateTarget();
        }

        private void UpdateTarget() {
            var closestNode = GetNodeClosestTo(truck.Position);
            if(truckTargetNode != closestNode)
                OnTargetChange?.Invoke();

            truckTargetNode = closestNode;
        }

        public Node GetNodeClosestTo(Vector2 position) {
            float minDistance = float.MaxValue;
            Node closestNode = null;
            foreach(Node node in nodes) {
                var dis = Vector2.DistanceSquared(node.Position, position);
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
