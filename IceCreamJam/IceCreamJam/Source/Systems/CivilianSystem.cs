using IceCreamJam.Entities;
using IceCreamJam.Entities.Civilians;
using IceCreamJam.Source.Entities;
using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IceCreamJam.Systems {
    class CivilianSystem : EntitySystem {
        private Truck truck;
        private ArcadeRigidbody truckrb;

        public List<Treat> treats = new List<Treat>();

        private const bool DebugSeekCrosshair = false;

        private const int approachRadius = 150;
        private const int stopRadius = 75;
        private const int awayRadius = 50;

        public CivilianSystem(Matcher matcher) : base(matcher) { }
        protected override void Begin() {
            truck = (Truck)Scene.FindEntity("Truck");
            truckrb = truck.GetComponent<ArcadeRigidbody>();
        }

        protected override void Process(List<Entity> entities) {
            return;

            foreach(Entity e in entities) {
                var civilian = (Civilian)e;

                if(DebugSeekCrosshair) {
                    civilian.Move((Scene.Camera.MouseToWorldPoint() - civilian.Position).Normalized());
                    continue;
                }

                if(treats.Count == 0)
                    continue;

                var target = treats.OrderBy(t => Vector2.Distance(t.Position, e.Position)).First();
                var distance = Vector2.Distance(target.Position, e.Position);
                if(distance > 200)
                    return;

                if(distance < 5) {
                    treats.Remove(target);
                    target.Destroy();
                }

                civilian.Flip(target.Position.X < civilian.Position.X);

                var dir = Vector2.Normalize(target.Position - e.Position);
                civilian.Move(dir);


                //var distance = Vector2.Distance(truck.Position, civilian.Position);
                //if(distance <= 0)
                //    continue;
                //
                //// Flip to face truck
                //civilian.Flip(truck.Position.X < civilian.Position.X);
                //
                //var direction = Vector2.Normalize(truck.Position - civilian.Position);
                //
                //var vector = Vector2.Zero;
                //var avoid = GetAvoidVector(civilian, entities);
                //
                //if(distance <= approachRadius && distance >= stopRadius)
                //    vector = direction;
                //else if(distance <= awayRadius) {
                //    var angle = Mathf.Atan2(truckrb.Velocity.Y, truckrb.Velocity.X);
                //
                //    var b = truck.Position;
                //    var a = truck.Position + truckrb.Velocity;
                //    var position = Math.Sign((b.X - a.X) * (civilian.Position.Y - a.Y) - (b.X - a.X) * (civilian.Position.X - a.X));
                //    var offset = Mathf.Deg2Rad * 90 * position;
                //    angle += offset;
                //
                //    var perpendicular = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                //    direction = Vector2.Normalize(direction + perpendicular);
                //
                //    vector = (-direction) * truckrb.Velocity.Length() / 2 * (1 / distance);
                //}
                //
                //var final = vector + avoid;
                //
                //civilian.Move(final);
                //civilian.PauseWalk(final.Length() == 0);
            }
        }

        private Vector2 GetAvoidVector(Civilian npc, List<Entity> entities) {
            var avoidVector = new Vector2();
            var nearbyCivilians = 0;

            foreach(Entity o in entities) {
                var dir = (npc.Position - o.Position);
                var dist = dir.Length();

                if(dist < 20f && dist > 0) {
                    avoidVector += dir / (float)Mathf.Pow(dist, 5);
                    nearbyCivilians++;
                }
            }

            if(nearbyCivilians != 0)
                avoidVector /= nearbyCivilians;

            if(avoidVector.Length() > 0)
                avoidVector.Normalize();
            else
                avoidVector = Vector2.Zero;

            return avoidVector;
        }
    }
}
