using Microsoft.Xna.Framework;
using Nez.Persistence;
using Nez.Systems;
using System.Collections.Generic;
using System.IO;

namespace IceCreamJam {
	public static class ContentManagerExt {
		public static List<Vector2[]> LoadPolygons(this NezContentManager content, string name) {
			List<Vector2[]> polys = new List<Vector2[]>();
			using (var stream = TitleContainer.OpenStream(name))
			using (StreamReader sr = new StreamReader(stream)) {
				polys.AddRange(Json.FromJson<Vector2[][]>(sr.ReadToEnd()));
			}
			return polys;
		}
	}
}
