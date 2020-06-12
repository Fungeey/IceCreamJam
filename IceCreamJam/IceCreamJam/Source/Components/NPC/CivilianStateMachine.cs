using IceCreamJam.Entities.Civilians;
using IceCreamJam.Entities.Enemies;
using IceCreamJam.Source.Entities;
using Microsoft.Xna.Framework;
using Nez;
using Nez.AI.FSM;
using Nez.Sprites;
using System.Collections;

namespace IceCreamJam.Source.Components {
	class CivilianStateMachine : Component {

		public StateMachine<Civilian> stateMachine;
		private const float range = 300;

		public override void OnAddedToEntity() {
			base.OnAddedToEntity();

			stateMachine = new StateMachine<Civilian>((Civilian)Entity, new States.StateIdle());
			stateMachine.AddState(new States.StateIdle());
			stateMachine.AddState(new States.StateWander());
			stateMachine.AddState(new States.StateSeekTreat());
			stateMachine.AddState(new States.StateFlee());
		}

		internal static class States {
			internal abstract class CivilianState : State<Civilian> {
				public SpriteAnimator animator;

				public override void Begin() {
					animator = _context.GetComponent<SpriteAnimator>();
				}

				protected Vector2 GetAvoidVector(Collider[] entities) {
					var avoidVector = new Vector2();
					var nearbyCivilians = 0;

					foreach(Collider col in entities) {
						if(col == null)
							continue;

						Entity o = col.Entity;

						var dir = (_context.Position - o.Position);
						var dist = dir.Length();

						if(dist > 0) {
							avoidVector += dir / (float)Mathf.Pow(dist, 5);
							nearbyCivilians++;
						}
					}

					if(nearbyCivilians != 0)
						avoidVector /= nearbyCivilians;

					if(avoidVector.Length() > 0)
						avoidVector.Normalize();
					else
						avoidVector = Vector2.Zero;

					return avoidVector;
				}
			}

			internal class NormalState : CivilianState {
				public override void Update(float deltaTime) {
					// If in range of enemy, change to run
					Collider[] colliders = new Collider[10];
					Physics.OverlapCircleAll(_context.Position, range, colliders);

					foreach(Collider c in colliders) {
						if(c == null)
							continue;

						if(c.Entity is Enemy) {
							_machine.ChangeState<StateFlee>();
							return;
						}

						if(c.Entity is Treat && !(_machine.CurrentState is StateSeekTreat)) {
							_machine.ChangeState<StateSeekTreat>();
							return;
						}
					}
				}
			}

			internal class StateIdle : NormalState {
				private ICoroutine StartWalk;

				public override void Begin() {
					base.Begin();
					animator.Play("Idle");
					StartWalk = Core.StartCoroutine(WalkTimer());
				}

				public override void End() {
					StartWalk.Stop();
				}

				IEnumerator WalkTimer() {
					yield return Coroutine.WaitForSeconds(Random.NextFloat(5) + 2);
					_machine.ChangeState<StateWander>();
				}
			}

			internal class StateWander : NormalState {
				private ICoroutine StopWalk;
				private Vector2 direction;

				public override void Begin() {
					base.Begin();
					animator.Play("Walking");
					StopWalk = Core.StartCoroutine(StopTimer());
					direction = Utility.RandomPointOnCircle();
				}

				public override void End() {
					StopWalk.Stop();
				}

				public override void Update(float deltaTime) {
					base.Update(deltaTime);
					_context.Move(direction/2);
				}

				IEnumerator StopTimer() {
					yield return Coroutine.WaitForSeconds(Random.NextFloat(5) + 2);
					_machine.ChangeState<StateIdle>();
				}
			}

			internal class StateSeekTreat : CivilianState {

				public override void Begin() {
					base.Begin();
					animator.Play("Walking");
				}

				public override void Update(float deltaTime) {
					if(animator.CurrentAnimationName == "Celebration") {
						if(animator.CurrentFrame == animator.CurrentAnimation.Sprites.Length - 1)
							_machine.ChangeState<StateIdle>();
						return;
					}

					var closeColliders = new Collider[10];
					Physics.OverlapCircleAll(_context.Position, range, closeColliders);

					foreach(Collider c in closeColliders) {
						if(c == null)
							continue;

						if(c.Entity is Enemy) {
							_machine.ChangeState<StateFlee>();
							return;
						}

						if(c.Entity is Treat) {
							var dist = (c.Entity.Position - _context.Position);
							if(dist.Length() < 10) {
								animator.Play("Celebration", SpriteAnimator.LoopMode.ClampForever);
								c.Entity.Destroy();
								return;
							}

							if(animator.CurrentAnimationName != "Celebration")
								_context.Move(dist.Normalized());

							return;
						}
					}

					// No treats nearby
					_machine.ChangeState<StateIdle>();
				}
			}

			internal class StateFlee : CivilianState {
				public override void Begin() {
					base.Begin();
					animator.Play("Shock", SpriteAnimator.LoopMode.ClampForever);
				}

				public override void Update(float deltaTime) {
					if(animator.CurrentAnimationName == "Shock") {
						if(animator.CurrentFrame == animator.CurrentAnimation.Sprites.Length - 1)
							animator.Play("Walking");
						return;
					}

					var closeEnemies = new Collider[10];
					Physics.OverlapCircleAll(_context.Position, range, closeEnemies);

					foreach(Collider c in closeEnemies) {
						if(c == null)
							continue;

						if(c.Entity is Enemy) {
							_context.Move(GetAvoidVector(closeEnemies));
							return;
						}
					}

					_machine.ChangeState<StateIdle>();
				}
			}
		}
	}
}
