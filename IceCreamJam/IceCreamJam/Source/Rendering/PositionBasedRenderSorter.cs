using IceCreamJam.UI;
using Nez;
using Nez.Sprites;
using System.Collections.Generic;

namespace IceCreamJam.Rendering {
	class PositionBasedRenderSorter : IComparer<IRenderable> {
		public int Compare(IRenderable a, IRenderable b) {
			if(a is UICanvas && b is UICanvas)
				return b.LayerDepth.CompareTo(a.LayerDepth);

			if(a is UICanvas || b is UICanvas)
				return a is UICanvas ? 1 : -1;

			if(a is TiledMapRenderer || b is TiledMapRenderer)
				return a is TiledMapRenderer ? -1 : 1;

			var aSprite = (SpriteRenderer)a;
			var bSprite = (SpriteRenderer)b;

			var e = CheckExceptions(aSprite, bSprite);
			if(e != 0)
				return e;

			var pos = ComparePosition(aSprite, bSprite);
			if(pos == 0)
				return b.RenderLayer.CompareTo(a.RenderLayer);

			return pos;
		}

		private int CheckExceptions(SpriteRenderer a, SpriteRenderer b) {
			if(a.Entity is Crosshair || b.Entity is Crosshair)
				return a.Entity is Crosshair ? 1 : -1;

			if(a.RenderLayer == Constants.RenderLayer_Truck && b.RenderLayer == Constants.RenderLayer_Truck)
					return b.LayerDepth.CompareTo(a.LayerDepth);

			return 0;
		}
		
		private int ComparePosition(SpriteRenderer a, SpriteRenderer b) {
			var aY = a.Transform.Position.Y + a.Height / 2;
			var bY = b.Transform.Position.Y + b.Height / 2;

			var aCol = a.Entity.GetComponent<Collider>();
			if(aCol != null)
				aY = aCol.Bounds.Bottom;
			
			var bCol = b.Entity.GetComponent<Collider>();
			if(bCol != null)
				bY = bCol.Bounds.Bottom;

			return aY.CompareTo(bY);
		}
	}
}
