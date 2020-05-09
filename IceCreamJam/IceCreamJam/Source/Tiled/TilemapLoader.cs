using IceCreamJam.Entities;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using System;

namespace IceCreamJam.Tiled {
    class TilemapLoader : SceneComponent {

        public TiledMap tiledMap;
        public event Action OnLoad;

        public override void OnEnabled() {
            base.OnEnabled();
        }

        public void Load(string filePath) {
            TmxMap map = Scene.Content.LoadTiledMap(filePath);
            tiledMap = Scene.AddEntity(new TiledMap(map));

            var layer = tiledMap.map.GetLayer<TmxLayer>(Constants.TiledLayerBuildings);
            foreach(TmxLayerTile t in layer.Tiles) {
                if(t == null)
                    continue;

                t.TilesetTile.Properties.TryGetValue(Constants.TiledPropertyID, out var value);

                if(value != "") {
                    Scene.AddEntity(new Building(value) {
                        Position = tiledMap.map.TileToWorldPosition(t.Position) + new Vector2(0, Constants.TiledCellSize),
                    });
                }
            }


            OnLoad?.Invoke();
        }
    }
}
