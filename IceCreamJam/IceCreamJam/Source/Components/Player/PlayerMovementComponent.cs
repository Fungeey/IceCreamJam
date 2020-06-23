using IceCreamJam.Tiled;
using Microsoft.Xna.Framework;
using Nez;

namespace IceCreamJam.Components {
    class PlayerMovementComponent : Component, IUpdatable {
        Collider collider;
        ArcadeRigidbody rb;
        DirectionComponent direction;
        PlayerInputComponent playerInput;

        PlayerStateMachine playerState;

        TilemapLoader loader;

        /// <summary>
        /// when accelerating, speed is set to this value if less than the kickstart
        /// </summary>
        public float kickstartSpeed = 20f;
        /// <summary>
        /// the acceleration of the vehicle in pixels/second/second
        /// </summary>
        public float acceleration = 40f;
        /// <summary>
        /// the normal maximum speed of the vehicle in pixels/second
        /// </summary>
        public float normalMaxSpeed = 200f;
        /// <summary>
        /// the deceleration of the vehicle when passively coasting in pixels/second/second
        /// </summary>
        public float coastDeceleration = 30f;
        /// <summary>
        /// the deceleration of the vehicle when actively braking in pixels/second/second
        /// </summary>
        public float brakeDeceleration = 80f;

        /// <summary>
        /// the base time per increment while turning in seconds
        /// </summary>
        public float turnTime = 0.125f;

        /// <summary>
        /// the speed added to the current speed at the beginning of a dash in pixels/second
        /// </summary>
        public float initialDashBoost = 100f;
        /// <summary>
        /// the maximum speed of the vehicle during a full dash in pixels/second
        /// </summary>
        public float fullDashMaxSpeed = 300f;
        /// <summary>
        /// the cooldown time before a full dash can be used again in seconds
        /// </summary>
        public float fullDashCooldownTime = 10f;
        /// <summary>
        /// the duration of a full dash in seconds
        /// </summary>
        public float fullDashTime = 4f;
        /// <summary>
        /// the duration of the lingering effects of a full dash in seconds
        /// </summary>
        public float fullDashLingerTime = 1f;
        /// <summary>
        /// the duration and cooldown time of a mini dash in seconds
        /// </summary>
        public float miniDashTime = 1f;


        private Direction8 targetHeading;
        private Direction8 CurrentHeading {
            get => direction.Direction; set => direction.Direction = value;
        }
        private Vector2 currentDirectionVector = new Vector2(1, 0);

        [Inspectable]
        public float Speed { get; private set; } = 0f;
        [Inspectable]
        public float MaxSpeed { get; private set; } = 200f;
        [Inspectable]
        private float turnTimer = 0;

        public override void OnAddedToEntity() {
            collider = Entity.GetComponent<Collider>();
            rb = Entity.GetComponent<ArcadeRigidbody>();
            direction = Entity.GetComponent<DirectionComponent>();
            playerInput = Entity.GetComponent<PlayerInputComponent>();
            playerState = Entity.GetComponent<PlayerStateMachine>();

            loader = Entity.Scene.GetSceneComponent<TilemapLoader>();

            playerInput.OnInputStart += this.PlayerInput_OnInputStart;
            direction.OnDirectionChange += this.Direction_OnDirectionChange;
        }

        private void PlayerInput_OnInputStart(Direction8 obj) {
            targetHeading = obj;
        }

        private void Direction_OnDirectionChange(Direction8 obj) {
            currentDirectionVector = obj.ToVector2().Normalized();
        }

        public void Update() {
            var state = playerState.stateMachine.CurrentState;
            if (state is PlayerStateMachine.States.StateNormal) {
                // when facing different direction from input, attempt to turn
                if (playerInput.InputHeld && CurrentHeading != targetHeading) {
                    int offset = CalculateRotationOffset(CurrentHeading.Difference(targetHeading));
                    CurrentHeading = CurrentHeading.RotateClockwise(offset);
                } else turnTimer = 0;
                Speed = CalculateCurrentSpeed(this.Speed);
            } else if (state is PlayerStateMachine.States.StateFullDash) {
                Speed = fullDashMaxSpeed;
            } else if (state is PlayerStateMachine.States.StateMiniDash stateMiniDash) {
                Speed = Mathf.Lerp(stateMiniDash.miniDashInitialSpeed + initialDashBoost, stateMiniDash.miniDashInitialSpeed, 1 - stateMiniDash.miniDashTimer / miniDashTime);
            }

            Vector2 currentVelocity = currentDirectionVector * Speed;
            rb.Velocity = currentVelocity;

            // TODO: remove hack to instantly stop movement when rammed into a building
            Vector2 movement = currentVelocity * Time.DeltaTime;
            if (collider.CollidesWithAny(ref movement, out CollisionResult result)) {
                if (result.Collider.PhysicsLayer.IsFlagSet((int)Constants.PhysicsLayers.Buildings)) {
                    Speed = 0;
                }
            }

            if (loader.mapIsLoaded)
                ClampPositionInBounds();
        }

        private void ClampPositionInBounds() {
            var halfBounds = collider.Bounds.Size * 0.5f;
            Entity.Position = Vector2.Clamp(Entity.Position, loader.mapBounds.Location + halfBounds, loader.mapBounds.Max - halfBounds);
        }

        private float CalculateCurrentSpeed(float speed) {
            if (InputManager.brake) {
                return Mathf.Approach(speed, 0, brakeDeceleration * Time.DeltaTime);
            } else if (playerInput.InputHeld) {
                if (speed < kickstartSpeed) speed = kickstartSpeed;
                if (CurrentHeading == targetHeading)
                    return Mathf.Approach(speed, MaxSpeed, acceleration * Time.DeltaTime);
                else return speed;
            } else {
                return Mathf.Approach(speed, 0, coastDeceleration * Time.DeltaTime);
            }
        }

        private int CalculateRotationOffset(int difference) {
            if (Speed == 0) {
                return difference;
            } else {
                if (turnTimer >= turnTime) {
                    turnTimer -= turnTime;
                    return System.Math.Sign(difference);
                } else {
                    turnTimer += Time.DeltaTime + (Time.DeltaTime * Speed / MaxSpeed);
                    return 0;
                }
            }
        }

        public override void DebugRender(Batcher batcher) {
            batcher.DrawLineAngle(Transform.Position, Transform.Rotation, 10, Color.Red);
        }
    }
}