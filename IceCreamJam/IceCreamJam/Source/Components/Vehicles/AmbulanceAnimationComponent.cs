using IceCreamJam.Components;
using IceCreamJam.Entities.Enemies;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System.Linq;

namespace IceCreamJam.Source.Components.Vehicles {
	class AmbulanceAnimationComponent : Component, IUpdatable {

		public SpriteAnimator animator;
		private AnimationSet moveAnim;
		private AnimationSet openAnim;
		private bool switchedAnimationSet = false;

		public bool AnimationFinished { get; private set; }

		public override void OnAddedToEntity() {
			animator = Entity.GetComponent<SpriteAnimator>();

			var texture = Entity.Scene.Content.LoadTexture(ContentPaths.Ambulance + "Ambulance.png");
			var sprites = Sprite.SpritesFromAtlas(texture, 96, 96);
			int directionalSelector() => (int)(Entity as Ambulance).currentDirection;

			var anims = Enumerable.Range(0, 8)
				.Select(i => new SpriteAnimation(sprites.GetRange(i * 6, 6).ToArray(), 4))
				.ToArray();
			moveAnim = new AnimationSet("move", directionalSelector, anims);
			animator.AddAnimationSet(moveAnim);

			var openTexture = Entity.Scene.Content.LoadTexture(ContentPaths.Ambulance + "AmbulanceOpen.png");
			var openSprites = Sprite.SpritesFromAtlas(openTexture, 96, 96);
			var openAnims = Enumerable.Range(0, 8).Select(i => new SpriteAnimation(openSprites.GetRange((i/2) * 3, 3).ToArray(), 4)).ToArray();
			openAnim = new AnimationSet("open", directionalSelector, openAnims);
			animator.AddAnimationSet(openAnim);
		}

		public void Update() {
			if(animator.CurrentAnimation != null) {
				AnimationFinished = animator.CurrentFrame == animator.CurrentAnimation.Sprites.Count() - 1;
				if(animator.CurrentAnimationName.Contains("open"))
					return;
			}
			
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
