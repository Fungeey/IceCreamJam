using Nez;
using Nez.Tiled;

namespace IceCreamJam.Tiled {
    class TiledMap : Entity {

        public TmxMap map;

        public TiledMap(TmxMap map) {
            this.map = map;
        }

        public override void OnAddedToScene() {
            base.OnAddedToScene();

            AddComponent(new TiledMapRenderer(map));
        }
    }
}
