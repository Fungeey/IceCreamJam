using IceCreamJam.WeaponSystem;
using Microsoft.Xna.Framework;
using Nez;
using System.Collections.Generic;

namespace IceCreamJam.Systems {
    class HomingProjectileSystem : EntitySystem {

        public List<HomingProjectile> projectiles;
        public const float projectileDistance = 100f;

        public HomingProjectileSystem(Matcher matcher) : base(matcher) {
            projectiles = new List<HomingProjectile>();
        }

        protected override void Process(List<Entity> entities) {
            base.Process(entities);

            foreach(HomingProjectile p in projectiles) {
                if(!p.Enabled)
                    continue;   // Skip disabled projectiles that are waiting in the Pool

                float minDist = float.MaxValue;
                Entity closest = null;
                foreach(Entity e in entities){
                    if(!e.Enabled) // Skip disabled targets that are waiting in the Pool
                        continue;

                    var distance = Vector2.Distance(p.Position, e.Position);
                    if(distance > projectileDistance)
                        continue;

                    if(distance < minDist) {
                        minDist = distance;
                        closest = e;
                    }
                }

                p.target = closest;
            }
        }
    }
}
