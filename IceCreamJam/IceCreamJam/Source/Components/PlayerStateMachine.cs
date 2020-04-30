using IceCreamJam.Source.Entities;
using Nez;
using Nez.AI.FSM;
using System;

namespace IceCreamJam.Source.Components {
	class PlayerStateMachine : Component {

		public StateMachine<Truck> stateMachine;

		public override void OnAddedToEntity() {
			stateMachine = new StateMachine<Truck>((Truck)Entity, new States.StateNormal());
			stateMachine.AddState(new States.StateFullDash());
			stateMachine.AddState(new States.StateMiniDash());
		}

		internal static class States {
			internal abstract class PlayerState : State<Truck> { }
			internal class StateNormal : PlayerState {
				public override void Update(float deltaTime) {
					throw new NotImplementedException();
				}
			}

			internal class StateFullDash : PlayerState {
				public override void Update(float deltaTime) {
					throw new NotImplementedException();
				}
			}

			internal class StateMiniDash : PlayerState {
				public override void Update(float deltaTime) {
					throw new NotImplementedException();
				}
			}
		}
	}
}
