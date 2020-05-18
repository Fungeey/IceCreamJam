using IceCreamJam.Components;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System.Linq;

namespace IceCreamJam.Source.Components.Vehicles {
	class VehicleAnimationComponent : Component, IUpdatable {

		private SpriteAnimator animator;
		private AnimationSet moveAnim;
		private VehicleMoveComponent move;
		private bool switchedAnimationSet = false;

		public override void OnAddedToEntity() {
			base.OnAddedToEntity();

			animator = Entity.GetComponent<SpriteAnimator>();
			move = Entity.GetComponent<VehicleMoveComponent>();

			var texture = Entity.Scene.Content.LoadTexture(ContentPaths.Cars + "CarMaster" + Random.NextInt(8) + ".png");
			var sprites = Sprite.SpritesFromAtlas(texture, 64, 64);
			int directionalSelector() => (int)move.CurrentDirection;
			moveAnim = new AnimationSet("move", directionalSelector, Enumerable.Range(0, 8).Select(i => Utility.SpriteAnimationFromParams(4, sprites[i * 2], sprites[i * 2 + 1])).ToList());
			animator.AddAnimationSet(moveAnim);
		}

		public void Update() {
			moveAnim.UpdateSelection();
			if(moveAnim.CurrentAnimation != animator.CurrentAnimation) {
				if(switchedAnimationSet) {
					switchedAnimationSet = false;
					animator.Play(moveAnim.name + moveAnim.CurrentAnimationIndex);
				} else {
					animator.PlayContinuing(moveAnim.name + moveAnim.CurrentAnimationIndex);
				}
			}
		}
	}
}
