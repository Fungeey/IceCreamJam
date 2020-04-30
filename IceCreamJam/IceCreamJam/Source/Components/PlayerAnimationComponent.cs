using IceCreamJam.Source.Content;
using Nez;
using Nez.Textures;
using System.Collections.Generic;
using System.Linq;

namespace IceCreamJam.Source.Components {
	class PlayerAnimationComponent : Component {
		private SetAnimator setAnimator;
		private DirectionComponent direction;

		private List<Sprite> sprites;

		public float idleFPS = 4f;
		public float dashFPS = 18;

		public override void OnAddedToEntity() {
			this.direction = Entity.GetComponent<DirectionComponent>();
			this.setAnimator = Entity.GetComponent<SetAnimator>();

			this.sprites = LoadTruckSprites();
			SetupAnimations();
		}

		private List<Sprite> LoadTruckSprites() {
			Nez.Systems.NezContentManager content = Entity.Scene.Content;
			var sprites = new List<Sprite>();
			sprites.AddRange(Sprite.SpritesFromAtlas(content.LoadTexture(ContentPaths.Truck + "truck.png"), 64, 64));
			sprites.AddRange(Sprite.SpritesFromAtlas(content.LoadTexture(ContentPaths.Truck + "truckdash1.png"), 64, 64));
			sprites.AddRange(Sprite.SpritesFromAtlas(content.LoadTexture(ContentPaths.Truck + "truckdash2.png"), 64, 64));
			sprites.AddRange(Sprite.SpritesFromAtlas(content.LoadTexture(ContentPaths.Truck + "truckdash3.png"), 64, 64));
			return sprites;
		}

		private void SetupAnimations() {
			int directionalSelector() => (int)direction.Direction;
			var idleAnimationSet = new AnimationSet("idle", directionalSelector, Enumerable.Range(0, 8).Select(i => Utility.SpriteAnimationFromParams(idleFPS, sprites[i * 2], sprites[i * 2 + 1])).ToList());
			var moveAnimationSet = new AnimationSet("move", directionalSelector, Enumerable.Range(0, 8).Select(i => Utility.SpriteAnimationFromParams(idleFPS, sprites[i * 2], sprites[i * 2 + 1])).ToList());
			var fullDashAnimationSet = new AnimationSet("fullDash", directionalSelector, Enumerable.Range(0, 8).Select(i => Utility.SpriteAnimationFromParams(dashFPS, sprites[16 + i], sprites[24 + i], sprites[32 + i])).ToList());

			setAnimator.AddAnimationSet(idleAnimationSet).AddAnimationSet(moveAnimationSet).AddAnimationSet(fullDashAnimationSet);
		}
	}
}
