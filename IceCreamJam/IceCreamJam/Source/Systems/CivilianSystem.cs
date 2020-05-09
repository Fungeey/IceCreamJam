using IceCreamJam.Entities;
using IceCreamJam.Entities.Civilians;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;

namespace IceCreamJam.Systems {
    class CivilianSystem : EntitySystem {
        private Truck truck;

        private const bool DebugSeekCrosshair = false;

        private const int approachRadius = 150;
        private const int stopRadius = 75;
        private const int awayRadius = 50;

        public CivilianSystem(Matcher matcher) : base(matcher) { }

        protected override void Process(List<Entity> entities) {
            base.Process(entities);

            if(truck == null)
                truck = (Truck)Scene.FindEntity("Truck");

            foreach(Entity e in entities) {
                var civilian = (Civilian)e;

                if(DebugSeekCrosshair) {
                    civilian.Move((Scene.Camera.MouseToWorldPoint() - civilian.Position).Normalized());
                    continue;
                }

                var distance = Vector2.Distance(truck.Position, civilian.Position);
                if(distance <= 0)
                    continue;

                // Flip to face truck
                civilian.Flip(truck.Position.X < civilian.Position.X);

                var direction = Vector2.Normalize(truck.Position - civilian.Position);

                var vector = Vector2.Zero;
                var avoid = GetAvoidVector(civilian, entities);

                if(distance <= approachRadius && distance >= stopRadius)
                    vector = direction;
                else if(distance <= awayRadius) {
                    var angle = Mathf.Atan2(truck.rb.Velocity.Y, truck.rb.Velocity.X);

                    var b = truck.Position;
                    var a = truck.Position + truck.rb.Velocity;
                    var position = Math.Sign((b.X - a.X) * (civilian.Position.Y - a.Y) - (b.X - a.X) * (civilian.Position.X - a.X));
                    var offset = Mathf.Deg2Rad * 90 * position;
                    angle += offset;

                    var perpendicular = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                    direction = Vector2.Normalize(direction + perpendicular);

                    vector = (-direction) * truck.rb.Velocity.Length() / 2 * (1 / distance);
                }

                var final = vector + avoid;

                civilian.Move(final);
                civilian.PauseWalk(final.Length() == 0);
            }
        }

        private Vector2 GetAvoidVector(Civilian npc, List<Entity> entities) {
            var avoid = new Vector2();
            var count = 0;

            foreach(Entity o in entities) {
                var dir = (npc.Position - o.Position);
                var dist = dir.Length();

                if(dist < 20f && dist > 0) {
                    avoid += dir / (float)Mathf.Pow(dist, 5);
                    count++;
                }
            }

            if(count != 0)
                avoid /= count;

            if(avoid.Length() > 0)
                avoid.Normalize();
            else
                avoid = Vector2.Zero;

            return avoid;
        }
    }
}
