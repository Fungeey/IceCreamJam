using Microsoft.Xna.Framework;
using Nez;

namespace IceCreamJam.Components {
    /// <summary>
    /// Moves its entity to follow a target
    /// </summary>
    class EntitySpringComponent : Component, IUpdatable {

        public Entity targetEntity;
        public Vector2 offset;
        public Vector2 RelativePosition;
        public float tension;
        public Vector2 TargetPosition => targetEntity.Position + offset;

        public EntitySpringComponent(Entity target, float tension) : this(target, Vector2.Zero, tension) { }

        public EntitySpringComponent(Entity target, Vector2 offset, float tension) {
            this.targetEntity = target;
            this.offset = offset;
            this.tension = Mathf.Clamp(tension, 0.1f, float.MaxValue);
        }

        public override void OnAddedToEntity() {
            Entity.Position = TargetPosition;
        }

        public void Update() {
            RelativePosition += (TargetPosition - (Entity.Position + RelativePosition)) / tension;
            Entity.Position = TargetPosition + RelativePosition;
        }

        public void Shock(Vector2 force) {
            RelativePosition += force;
        }
    }
}
