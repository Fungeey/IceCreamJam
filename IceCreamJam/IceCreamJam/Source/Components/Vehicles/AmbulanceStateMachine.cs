using IceCreamJam.Entities;
using IceCreamJam.Entities.Enemies;
using IceCreamJam.RoadSystem;
using Microsoft.Xna.Framework;
using Nez;
using Nez.AI.FSM;
using System.Collections;

namespace IceCreamJam.Components.Vehicles {
	class AmbulanceStateMachine : Component, IUpdatable {

		public const float speed = 180f;
		public StateMachine<Ambulance> stateMachine;
		public static RoadSystemComponent roadSystem;

		public override void OnAddedToEntity() {
			base.OnAddedToEntity();

			if(roadSystem == null)
				roadSystem = Entity.Scene.GetSceneComponent<RoadSystemComponent>();

			stateMachine = new StateMachine<Ambulance>(Entity as Ambulance, new States.StateDrive());
			stateMachine.AddState(new States.StateTurn());
			stateMachine.AddState(new States.StateUnload());
			stateMachine.AddState(new States.StateDriveAway());
		}

		public void Update() {
			stateMachine.Update(Time.DeltaTime);
		}

		public override void DebugRender(Batcher batcher) {
			base.DebugRender(batcher);
			((States.AmbulanceState)stateMachine.CurrentState).DebugRender(batcher);
		}

		internal static class States {

			internal abstract class AmbulanceState : State<Ambulance> {
				public virtual void DebugRender(Batcher batcher) {}
			}

			internal class StateDrive : AmbulanceState {

				public override void Begin() {
					base.Begin();
					if(_context.currentNode == null) {
						_context.ReevaluateGraph();
						_context.currentNode = _context.path[_context.nodesTraveled++];
					}
				}

				public override void Update(float deltaTime) {
					var targetPosition = _context.currentNode.Position;

					var distance = (targetPosition - _context.Position);
					var direction = distance.Normalized();
					_context.currentDirection = Direction8Ext.FromVector2(distance);

					_context.Move(direction.Normalized() * Time.DeltaTime * speed);

					if((Enemy.truck.Position - _context.Position).Length() < 300 && _context.currentDirection.IsOrthogonal()) {
						_machine.ChangeState<StateUnload>();
						return;
					}
					
					// Arrived at intersection
					if(distance.Length() < 10)
						_machine.ChangeState<StateTurn>();
				}

				public override void DebugRender(Batcher batcher) {
					base.DebugRender(batcher);
					batcher.DrawCircle(_context.currentNode.Position, 50, Color.Blue, 10);
				}
			}
			internal class StateTurn : AmbulanceState {

				public override void Begin() {
					base.Begin();

					if(_context.nodesTraveled == _context.path.Count) {
						_machine.ChangeState<StateUnload>();
					} else {
						_context.currentNode = _context.path[_context.nodesTraveled++];
						_machine.ChangeState<StateDrive>();
					}
				}

				public override void Update(float deltaTime) { }
			}

			internal class StateUnload : AmbulanceState {
				private ICoroutine spawnRoutine;

				public override void Begin() {
					base.Begin();
					int anim = (int)_context.currentDirection;
					_context.animationComponent.animator.Play("open" + anim, Nez.Sprites.SpriteAnimator.LoopMode.ClampForever);
				}

				public override void Update(float deltaTime) {
					if(_context.animationComponent.AnimationFinished && _context.animationComponent.animator.CurrentAnimationName.Contains("open")) {
						if(spawnRoutine == null)
							spawnRoutine = Core.StartCoroutine(Spawn());
					}
				}

				IEnumerator Spawn() {
					for(int i = 0; i < 5; i++) {
						yield return Coroutine.WaitForSeconds(0.5f);
						_context.Scene.AddEntity(new Doctor() { Position = _context.Position });
					}
					_machine.ChangeState<StateDriveAway>();
					_context.Destroy();
				}
			}

			internal class StateDriveAway : AmbulanceState {
				public override void Update(float deltaTime) {

				}
			}
		}
	}
}
