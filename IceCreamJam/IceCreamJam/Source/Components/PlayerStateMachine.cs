using IceCreamJam.Entities;
using Nez;
using Nez.AI.FSM;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IceCreamJam.Components {
	class PlayerStateMachine : Component {

		public StateMachine<Truck> stateMachine;

		private SetAnimator setAnimator;
		private DirectionComponent direction;
		private ColliderManager colliderManager;

		private List<Sprite> sprites;

		public float idleFPS = 4f;
		public float dashFPS = 18;

		public override void OnAddedToEntity() {
			this.direction = Entity.GetComponent<DirectionComponent>();
			this.setAnimator = Entity.GetComponent<SetAnimator>();
			this.colliderManager = Entity.GetComponent<ColliderManager>();

			InitAnimations();
			direction.OnDirectionChange += i => colliderManager.SetIndex((int)i);

			stateMachine = new StateMachine<Truck>((Truck)Entity, new States.StateNormal());
			stateMachine.AddState(new States.StateFullDash());
			stateMachine.AddState(new States.StateMiniDash());
		}

		private void InitAnimations() {
			LoadTruckSprites();
			SetupAnimationSets();
		}

		private void LoadTruckSprites() {
			Nez.Systems.NezContentManager content = Entity.Scene.Content;
			this.sprites = new List<Sprite>();
			sprites.AddRange(Sprite.SpritesFromAtlas(content.LoadTexture(ContentPaths.Truck + "truck.png"), 64, 64));
			sprites.AddRange(Sprite.SpritesFromAtlas(content.LoadTexture(ContentPaths.Truck + "truckdash1.png"), 64, 64));
			sprites.AddRange(Sprite.SpritesFromAtlas(content.LoadTexture(ContentPaths.Truck + "truckdash2.png"), 64, 64));
			sprites.AddRange(Sprite.SpritesFromAtlas(content.LoadTexture(ContentPaths.Truck + "truckdash3.png"), 64, 64));
		}

		private void SetupAnimationSets() {
			int directionalSelector() => (int)direction.Direction;
			var idleAnimationSet = new AnimationSet("idle", directionalSelector, Enumerable.Range(0, 8).Select(i => Utility.SpriteAnimationFromParams(idleFPS, sprites[i * 2], sprites[i * 2 + 1])).ToList());
			var moveAnimationSet = new AnimationSet("move", directionalSelector, Enumerable.Range(0, 8).Select(i => Utility.SpriteAnimationFromParams(idleFPS, sprites[i * 2], sprites[i * 2 + 1])).ToList());
			var fullDashAnimationSet = new AnimationSet("fullDash", directionalSelector, Enumerable.Range(0, 8).Select(i => Utility.SpriteAnimationFromParams(dashFPS, sprites[16 + i], sprites[24 + i], sprites[32 + i])).ToList());

			setAnimator.AddAnimationSet(idleAnimationSet).AddAnimationSet(moveAnimationSet).AddAnimationSet(fullDashAnimationSet);
		}

		internal static class States {
			internal abstract class PlayerState : State<Truck> { }
			internal class StateNormal : PlayerState {
				public override void Begin() {
					_context.GetComponent<SetAnimator>().PlaySet("idle");
				}

				public override void Update(float deltaTime) {

				}
			}

			internal class StateFullDash : PlayerState {
				public override void Begin() {
					_context.GetComponent<SetAnimator>().PlaySet("fullDash");
				}

				public override void Update(float deltaTime) {

				}
			}

			internal class StateMiniDash : PlayerState {
				public override void Update(float deltaTime) {

				}
			}
		}
	}
}
