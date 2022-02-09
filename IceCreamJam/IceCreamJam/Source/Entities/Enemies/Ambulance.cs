using IceCreamJam.Components.Vehicles;
using IceCreamJam.RoadSystem;
using IceCreamJam.Source.Components.Vehicles;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System.Collections.Generic;

namespace IceCreamJam.Entities.Enemies {
	class Ambulance : Enemy {

		public Node currentNode;
		public Direction8 currentDirection;
		public List<Node> path;
		public int nodesTraveled;

		private AmbulanceStateMachine stateMachine;
		public AmbulanceAnimationComponent animationComponent;

		public override void OnAddedToScene() {
			base.OnAddedToScene();

			Name = "Ambulance";
			AddComponent(new SpriteAnimator());
			stateMachine = AddComponent(new AmbulanceStateMachine());
			animationComponent = AddComponent(new AmbulanceAnimationComponent());

			path = new List<Node>();
		}

		public void ReevaluateGraph() {
			var start = AmbulanceStateMachine.roadSystem.GetNodeClosestTo(this.Position);
			path.Clear();
			path.AddRange(AmbulanceStateMachine.roadSystem.graph.Search(start, AmbulanceStateMachine.roadSystem.truckTargetNode));
		}

		public override void Initialize(Vector2 position) {
			base.Initialize(position);
			AmbulanceStateMachine.roadSystem.OnTargetChange += ReevaluateGraph;
		}

		public override void Reset() {
			base.Reset();
			AmbulanceStateMachine.roadSystem.OnTargetChange -= ReevaluateGraph;
		}

		public Node TargetNextNode() {
			if(nodesTraveled == path.Count) {
				stateMachine.stateMachine.ChangeState<AmbulanceStateMachine.States.StateUnload>();
				return currentNode;
			}

			return path[nodesTraveled++];
		}
	}
}
