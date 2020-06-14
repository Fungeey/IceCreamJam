using IceCreamJam.Entities;
using IceCreamJam.RoadSystem;
using Microsoft.Xna.Framework;
using Nez;
using Nez.AI.FSM;
using System;

namespace IceCreamJam.Source.RoadSystem {
	class CivilianCarStateMachine : Component, IUpdatable {

		public const bool doDebugRender = true;
		private const float speed = 80;
		private const float checkDistance = 64;

		public StateMachine<CivilianCar> stateMachine;
		public static RoadSystemComponent roadSystem;

		public event Action OnTurnFinished;

		public override void OnAddedToEntity() {
			base.OnAddedToEntity();

			if(roadSystem == null)
				roadSystem = Entity.Scene.GetSceneComponent<RoadSystemComponent>();

			stateMachine = new StateMachine<CivilianCar>((CivilianCar)Entity, new States.StateInitialize());
			stateMachine.AddState(new States.StateDriving());
			stateMachine.AddState(new States.StateTurning());
			stateMachine.AddState(new States.StateWaiting());
		}

		public void Update() {
			stateMachine.Update(Time.DeltaTime);
		}

		public override void DebugRender(Batcher batcher) {
			base.DebugRender(batcher);
			if(doDebugRender) (stateMachine.CurrentState as States.CarState).DebugRender(batcher);
		}

		public void StartTurn() {
			stateMachine.ChangeState<States.StateTurning>();
		}

		internal static class States {
			internal abstract class CarState : State<CivilianCar> {

				protected Vector2 GetAlignVector(Vector2 target) {
					return _context.currentDirection.IsVertical() ?         // Move to be in line with the current target position
						new Vector2(target.X - _context.Position.X, 0) :    // If moving vertically, adjust horizontally to be in line
						new Vector2(0, target.Y - _context.Position.Y);     // If moving horizontally, adjust vertically to be in line
				}

				protected bool IsBlocked() {
					Collider[] colliders = new Collider[3];		// Have to check multiple colliders otherwise the truck won't be picked up
					var rect = GetCheckRectangle();
					Physics.OverlapRectangleAll(ref rect, colliders);
					foreach(Collider collider in colliders) {
						var isBlocked = collider != null && ((collider.Entity is Vehicle && collider.Entity != _context) || collider.Entity is Truck);
						if(isBlocked)
							return true;
					}
					return false;
				}

				private RectangleF GetCheckRectangle() {
					if(!_context.currentDirection.IsHorizontal() && !_context.currentDirection.IsVertical())
						return new RectangleF();

					var pos = _context.Position + _context.currentDirection.ToNormalizedVector2(25 + checkDistance/2);
					var size = _context.currentDirection.IsVertical() ? new Vector2(48, checkDistance) : new Vector2(checkDistance, 48);
					return new RectangleF(pos - size/2, size);
				}

				public virtual void DebugRender(Batcher batcher) {
					if(_context.previousNode != null) batcher.DrawCircle(_context.previousNode.Position, 100, Color.White, 15);
					if(_context.currentNode != null) batcher.DrawCircle(_context.currentNode.Position, 100, Color.Gray, 15);
					if(_context.nextNode != null) batcher.DrawCircle(_context.nextNode.Position, 100, Color.Black, 15);

					batcher.DrawRect(GetCheckRectangle(), Color.Black);
				}
			}
			internal class StateInitialize : CarState {
				private Vector2 startPos;
				private Vector2 targetPosition;

				public override void Begin() {
					_context.currentNode = roadSystem.GetNodeClosestTo(_context.Position);
					startPos = _context.Position;
				}

				public override void Update(float deltaTime) {
					targetPosition = _context.currentNode.Position + Node.GetOffsetBefore(_context.currentNode.Position, startPos);

					var distance = targetPosition - _context.Position;
					var direction = distance.Normalized();
					_context.currentDirection = Direction8Ext.FromVector2(distance);

					if(!IsBlocked()) {
						var align = GetAlignVector(targetPosition) / 3;
						_context.Move(direction.Normalized() * Time.DeltaTime * speed + align);
					}

					if(distance.Length() < 10)
						_machine.ChangeState<StateWaiting>();
				}

				public override void DebugRender(Batcher batcher) {
					base.DebugRender(batcher);
					batcher.DrawLine(_context.Position, targetPosition, Color.Blue, 10);
				}
			}

			internal class StateDriving : CarState {
				private Vector2 targetPosition;

				public override void Update(float deltaTime) {
					var offset = Node.GetOffsetBefore(_context.currentNode.Position, _context.previousNode.Position);
					targetPosition = _context.currentNode.Position + offset;

					var distance = (targetPosition - _context.Position);
					var direction = distance.Normalized();
					_context.currentDirection = Direction8Ext.FromVector2(distance);

					if(!IsBlocked()) {
						var align = GetAlignVector(targetPosition) / 3;
						_context.Move(direction.Normalized() * Time.DeltaTime * speed + align);
					}

					// Arrived at intersection
					if(distance.Length() < 10)
						_machine.ChangeState<StateWaiting>();
				}

				public override void DebugRender(Batcher batcher) {
					base.DebugRender(batcher);
					batcher.DrawLine(_context.Position, targetPosition, Color.Green, 10);
				}
			}

			internal class StateTurning : CarState {
				private Vector2 turnTarget;

				public override void Begin() {
					base.Begin();
					_context.TargetNextNode();
				}

				public override void Update(float deltaTime) {
					var offset = Node.GetOffsetAfter(_context.currentNode, _context.previousNode);
					turnTarget = _context.previousNode.Position + offset;

					var distance = (turnTarget - _context.Position);
					var direction = distance.Normalized();
					_context.currentDirection = Direction8Ext.FromVector2(distance);

					if(!IsBlocked())
						_context.Move(direction * Time.DeltaTime * speed);

					if(distance.Length() < 10) {
						// Finish turn
						_context.stateMachine.OnTurnFinished?.Invoke();
						_machine.ChangeState<StateDriving>();
					}
				}

				public override void DebugRender(Batcher batcher) {
					base.DebugRender(batcher);
					batcher.DrawCircle(turnTarget, 20, Color.Red, 15);
				}
			}

			internal class StateWaiting : CarState {
				public override void Begin() {
					base.Begin();
					_context.currentNode.QueueVehicle(_context.stateMachine);
				}

				public override void Update(float deltaTime) { }
			}
		}
	}
}
