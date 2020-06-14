using IceCreamJam.Components;
using IceCreamJam.RoadSystem;
using IceCreamJam.Source.Components.Vehicles;
using IceCreamJam.Source.RoadSystem;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace IceCreamJam.Entities {
	class CivilianCar : Vehicle {

        private Mover mover;
        public Node previousNode, currentNode, nextNode;
        public Direction8 currentDirection;
        public CivilianCarStateMachine stateMachine;
        private readonly CivilianCarSpawnSystem spawner;

        public CivilianCar(CivilianCarSpawnSystem spawner) {
            this.spawner = spawner;
            spawner.AddCar(this);
        }

        public override void OnAddedToScene() {
            base.OnAddedToScene();
            Name = "CivilianCar";

            AddComponent(new CircleCollider(16));
            AddComponent(new RenderSorterComponent());
            AddComponent(new SpriteAnimator());
            AddComponent(new VehicleAnimationComponent());

            stateMachine = AddComponent(new CivilianCarStateMachine());

            mover = AddComponent(new Mover());
        }

        public void Move(Vector2 delta) {
            mover.Move(delta, out var result);
		}

        public void TargetNextNode() {
            if(previousNode == null) {      // Arriving at first node coming from spawn point
				previousNode = currentNode;
				currentNode = previousNode.GetRandomConnectedNode();
				nextNode = currentNode.GetRandomConnectedNode(previousNode);
				return;
			}

            var previous = currentNode;
            previousNode = currentNode;
            currentNode = nextNode;

            nextNode = currentNode.GetRandomConnectedNode(previous);
        }

		public override void OnRemovedFromScene() {
			base.OnRemovedFromScene();
            spawner.RemoveCar(this);
        }
	}
}
