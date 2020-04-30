using IceCreamJam.Entities;
using Nez;
using Nez.Tiled;

namespace IceCreamJam.Tiled {
    class TilemapLoader : SceneComponent {
        public override void OnEnabled() {
            base.OnEnabled();
        }

        public void Load(string filePath) {
            TmxMap map = Scene.Content.LoadTiledMap(filePath);
            var layer = map.GetLayer<TmxLayer>(Constants.TiledLayerBuildings);

            Scene.AddEntity(new TiledMap(map));

            //foreach(TmxLayerTile t in layer.Tiles) {
            //    if(t == null)
            //        continue;
            //
            //    t.TilesetTile.Properties.TryGetValue(Constants.TiledPropertyID, out var value);
            //
            //    Building b;
            //    if(value == "Building") {
            //        b = Scene.AddEntity(new Building());
            //        b.Position = map.TileToWorldPosition(t.Position);
            //    }
            //}
        }
    }
}
