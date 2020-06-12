using IceCreamJam.Systems;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace IceCreamJam.Source.Entities {
	class Treat : Entity {

		public Vector2 velocity;
		private SpriteRenderer renderer;
		private static CivilianSystem civilianSystem;

		public Treat() {
			Name = "Treat";
		}

		public override void OnAddedToScene() {
			base.OnAddedToScene();

			if(civilianSystem == null)
				civilianSystem = Scene.GetEntityProcessor<CivilianSystem>();
			civilianSystem.treats.Add(this);

			renderer = AddComponent(new SpriteRenderer(Scene.Content.LoadTexture(ContentPaths.TestTreat)));
			AddComponent(new CircleCollider(5) { IsTrigger = true });

			if(velocity == Vector2.Zero)
				velocity = Utility.RandomPointOnCircle() * 5;
		}

		public override void Update() {
			base.Update();

			Position += velocity;
			velocity *= 0.95f;

			renderer.SetLocalOffset(new Vector2(0, Mathf.Sin(Time.TotalTime * 2) + 0.5f) * 5);
		}
	}
}
