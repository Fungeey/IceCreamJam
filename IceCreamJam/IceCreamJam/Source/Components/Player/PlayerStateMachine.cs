using IceCreamJam.Entities;
using Nez;
using Nez.AI.FSM;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IceCreamJam.Components {
    class PlayerStateMachine : Component, IUpdatable {

        public StateMachine<Truck> stateMachine;

        private SetAnimator setAnimator;
        private DirectionComponent direction;
        private ColliderManager colliderManager;

        private List<Sprite> sprites;

        [Inspectable]
        public float fullDashCooldownTimer;

        public override void OnAddedToEntity() {
            this.direction = Entity.GetComponent<DirectionComponent>();
            this.setAnimator = Entity.GetComponent<SetAnimator>();
            this.colliderManager = Entity.GetComponent<ColliderManager>();

            InitAnimations();
            direction.OnDirectionChange += i => colliderManager.SetIndex((int)i);

            stateMachine = new StateMachine<Truck>((Truck)Entity, new States.StateNormal());
            stateMachine.AddState(new States.StateFullDash());
            stateMachine.AddState(new States.StateMiniDash());
        }

        private void InitAnimations() {
            LoadTruckSprites();
            SetupAnimationSets();
        }

        private void LoadTruckSprites() {
            Nez.Systems.NezContentManager content = Entity.Scene.Content;
            this.sprites = new List<Sprite>();
            sprites.AddRange(Sprite.SpritesFromAtlas(content.LoadTexture(ContentPaths.Truck + "truck.png"), 64, 64));
            sprites.AddRange(Sprite.SpritesFromAtlas(content.LoadTexture(ContentPaths.Truck + "truckdash1.png"), 64, 64));
            sprites.AddRange(Sprite.SpritesFromAtlas(content.LoadTexture(ContentPaths.Truck + "truckdash2.png"), 64, 64));
            sprites.AddRange(Sprite.SpritesFromAtlas(content.LoadTexture(ContentPaths.Truck + "truckdash3.png"), 64, 64));
        }

        private void SetupAnimationSets() {
            float idleFPS = 4f;
            float dashFPS = 18;

            int directionalSelector() => (int)direction.Direction;
            var idleAnimationSet = new AnimationSet("idle", directionalSelector, Enumerable.Range(0, 8).Select(i => Utility.SpriteAnimationFromParams(idleFPS, sprites[i * 2], sprites[i * 2 + 1])).ToList());
            var moveAnimationSet = new AnimationSet("move", directionalSelector, Enumerable.Range(0, 8).Select(i => Utility.SpriteAnimationFromParams(idleFPS, sprites[i * 2], sprites[i * 2 + 1])).ToList());
            var fullDashAnimationSet = new AnimationSet("fullDash", directionalSelector, Enumerable.Range(0, 8).Select(i => Utility.SpriteAnimationFromParams(dashFPS, sprites[16 + i], sprites[24 + i], sprites[32 + i])).ToList());

            setAnimator.AddAnimationSet(idleAnimationSet).AddAnimationSet(moveAnimationSet).AddAnimationSet(fullDashAnimationSet);
        }

        public void Update() {
            stateMachine.Update(Time.DeltaTime);
        }

        internal static class States {
            internal abstract class PlayerState : State<Truck> {
                protected PlayerStateMachine playerStateMachine;
                protected PlayerMovementComponent playerMovementComponent;

                public override void OnInitialized() {
                    playerStateMachine = _context.GetComponent<PlayerStateMachine>();
                    playerMovementComponent = _context.GetComponent<PlayerMovementComponent>();
                }
            }

            internal class StateNormal : PlayerState {
                public override void Begin() {
                    _context.GetComponent<SetAnimator>().PlaySet("idle");
                }

                public override void Reason() {
                    if (InputManager.dash.IsDown) {
                        if (playerStateMachine.fullDashCooldownTimer == 0 && playerMovementComponent.Speed + playerMovementComponent.initialDashBoost >= playerMovementComponent.normalMaxSpeed) {
                            _machine.ChangeState<StateFullDash>();
                        } else {
                            _machine.ChangeState<StateMiniDash>();
                        }
                    }
                }

                public override void Update(float deltaTime) {
                    playerStateMachine.fullDashCooldownTimer = Mathf.Approach(playerStateMachine.fullDashCooldownTimer, 0, deltaTime);
                }
            }

            internal class StateFullDash : PlayerState {
                internal float fullDashTimer;
                public override void Begin() {
                    _context.GetComponent<SetAnimator>().PlaySet("fullDash");
                    fullDashTimer = playerMovementComponent.fullDashTime;
                }

                public override void Reason() {
                    if (fullDashTimer == 0)
                        _machine.ChangeState<StateNormal>();
                }

                public override void Update(float deltaTime) {
                    fullDashTimer = Mathf.Approach(fullDashTimer, 0, deltaTime);
                }

                public override void End() {
                    playerStateMachine.fullDashCooldownTimer = playerMovementComponent.fullDashCooldownTime;
                }
            }

            internal class StateMiniDash : PlayerState {
                internal float miniDashTimer;
                internal float miniDashInitialSpeed;

                public override void Begin() {
                    miniDashTimer = playerMovementComponent.miniDashTime;
                    miniDashInitialSpeed = playerMovementComponent.Speed;
                }

                public override void Reason() {
                    if (miniDashTimer == 0)
                        _machine.ChangeState<StateNormal>();
                }

                public override void Update(float deltaTime) {
                    miniDashTimer = Mathf.Approach(miniDashTimer, 0, deltaTime);
                }
            }
        }
    }
}
