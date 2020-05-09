using Microsoft.Xna.Framework;
using Nez;
using System;

namespace IceCreamJam.Components {
	public class RenderSorterComponent : Component, IUpdatable {

		RenderableComponent renderable;
		private float yPos;

		public override void OnAddedToEntity() {
			base.OnAddedToEntity();
			renderable = Entity.GetComponent<RenderableComponent>();
		}

		public void Update() {
			if(Math.Abs(yPos - Entity.Position.Y) > 1)
				Entity.Scene.RenderableComponents.SetRenderLayerNeedsComponentSort(renderable.RenderLayer);
			
			yPos = Entity.Position.Y;
		}

		public override void DebugRender(Batcher batcher) {
			base.DebugRender(batcher);

			if(Entity.GetComponent<Collider>() != null)
				batcher.DrawCircle(new Vector2(Entity.Position.X, Entity.GetComponent<Collider>().Bounds.Bottom), 3, Color.White);
		}
	}
}
