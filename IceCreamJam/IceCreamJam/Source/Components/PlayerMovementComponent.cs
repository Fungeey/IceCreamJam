using Microsoft.Xna.Framework;
using Nez;

namespace IceCreamJam.Components {
	class PlayerMovementComponent : Component, IUpdatable {
		Collider collider;
		ArcadeRigidbody rb;
		DirectionComponent direction;
		PlayerInputComponent playerInput;
		SetAnimator animator;

		/// <summary>
		/// when accelerating, speed is set to this value if less than the kickstart
		/// </summary>
		public float kickstartSpeed = 40f;
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
		public float initialDashBoost = 50f;
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
		private State state;
		private enum State {
			Normal, FullDash, MiniDash
		}

		public bool IsFullDashing => state == State.FullDash;

		[Inspectable]
		public float Speed { get; private set; } = 0f;
		[Inspectable]
		public const float maxSpeed = 200f;
		[Inspectable]
		private float turnTimer = 0;

		[Inspectable]
		private float fullDashCooldownTimer;
		[Inspectable]
		private float fullDashTimer;
		[Inspectable]
		private float miniDashTimer;
		private float miniDashInitialSpeed;

		public override void OnAddedToEntity() {
			collider = Entity.GetComponent<Collider>();
			rb = Entity.GetComponent<ArcadeRigidbody>();
			direction = Entity.GetComponent<DirectionComponent>();
			playerInput = Entity.GetComponent<PlayerInputComponent>();
			animator = Entity.GetComponent<SetAnimator>();

			playerInput.OnInputStart += this.PlayerInput_OnInputStart;
			direction.OnDirectionChange += this.Direction_OnDirectionChange;

			state = State.Normal;
		}

		private void PlayerInput_OnInputStart(Direction8 obj) {
			targetHeading = obj;
		}

		private void Direction_OnDirectionChange(Direction8 obj) {
			currentDirectionVector = obj.ToVector2().Normalized();
		}

		public void Update() {
			if (state == State.Normal) {
				if (InputManager.dash.IsDown) {
					if (fullDashCooldownTimer == 0 && Speed + initialDashBoost >= normalMaxSpeed) {
						state = State.FullDash;
						fullDashTimer = fullDashTime;
						animator.PlaySet("fullDash");
					} else {
						state = State.MiniDash;
						miniDashTimer = miniDashTime;
						miniDashInitialSpeed = Speed;
					}
				}
			} else if (state == State.FullDash) {
				if (fullDashTimer == 0) {
					state = State.Normal;
					fullDashCooldownTimer = fullDashCooldownTime;
					animator.PlaySet("idle");
				}
			} else if (state == State.MiniDash) {
				if (miniDashTimer == 0) {
					state = State.Normal;
					animator.PlaySet("idle");
				}
			}

			if (state == State.Normal) {
				// when facing different direction from input, attempt to turn
				if (playerInput.InputHeld && CurrentHeading != targetHeading) {
					int offset = CalculateRotationOffset(CurrentHeading.Difference(targetHeading));
					CurrentHeading = CurrentHeading.RotateClockwise(offset);
				} else turnTimer = 0;
				Speed = CalculateCurrentSpeed(this.Speed);
				fullDashCooldownTimer = Mathf.Approach(fullDashCooldownTimer, 0, Time.DeltaTime);
			} else if (state == State.FullDash) {
				Speed = fullDashMaxSpeed;
				fullDashTimer = Mathf.Approach(fullDashTimer, 0, Time.DeltaTime);
			} else if (state == State.MiniDash) {
				Speed = Mathf.Lerp(miniDashInitialSpeed + initialDashBoost, miniDashInitialSpeed, 1 - miniDashTimer / miniDashTime);
				miniDashTimer = Mathf.Approach(miniDashTimer, 0, Time.DeltaTime);
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


		}

		private float CalculateCurrentSpeed(float speed) {
			if (InputManager.brake) {
				return Mathf.Approach(speed, 0, brakeDeceleration * Time.DeltaTime);
			} else if (playerInput.InputHeld && CurrentHeading == targetHeading) {
				if (speed < kickstartSpeed) speed = kickstartSpeed;
				return Mathf.Approach(speed, maxSpeed, acceleration * Time.DeltaTime);
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
					turnTimer += Time.DeltaTime + (Time.DeltaTime * Speed / maxSpeed);
					return 0;
				}
			}
		}

		public override void DebugRender(Batcher batcher) {
			batcher.DrawLineAngle(Transform.Position, Transform.Rotation, 10, Color.Red);
		}
	}
}
