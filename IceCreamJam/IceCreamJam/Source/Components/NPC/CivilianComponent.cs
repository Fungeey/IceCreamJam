using Nez;
using Nez.Sprites;
using Nez.Textures;
using System.Linq;

namespace IceCreamJam.Components {
    class CivilianComponent : Component {

		private SpriteAnimator animator;


		public override void OnAddedToEntity() {
			base.OnAddedToEntity();

			animator = Entity.GetComponent<SpriteAnimator>();
			InitializeAnimations();
		}

		private void InitializeAnimations() {
			var n = Random.NextInt(20);
			var texture = Entity.Scene.Content.LoadTexture(ContentPaths.CivilianSheet);
			var sprites = Sprite.SpritesFromAtlas(texture, 32, 32).Where((s, i) => i % 20 == n).ToList();
			
			// TODO: Separate idle into blink and eyes open (blink periodically)
			animator.AddAnimation("Idle", 3, Utility.SelectFromList(sprites, 0, 0, 0, 1, 2).ToArray());
			animator.AddAnimation("Shock", Utility.SelectFromList(sprites, 5, 6, 7, 7).ToArray());
			animator.AddAnimation("Celebration", 2, Utility.SelectFromList(sprites, 9, 10, 10, 10).ToArray());
			animator.AddAnimation("Walking", sprites.GetRange(11, 6).ToArray());
			
			animator.Play("Idle");
		}
	}
}
