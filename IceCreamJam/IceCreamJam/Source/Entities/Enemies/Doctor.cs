using IceCreamJam.WeaponSystem.EnemyWeapons;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System.Collections;

namespace IceCreamJam.Entities.Enemies {
    class Doctor : Enemy {

        private const float stunCooldown = 0.5f;
        private const float range = 300f;
        private bool canBeStunned;
        private bool canShoot;

        private Vector2 shootDirection;
        private ICoroutine approachCoroutine;
        private SpriteAnimator animator;

        private enum DoctorStates {
            idle,
            approach,
            attack
        }
        private DoctorStates state = DoctorStates.approach;

        public Doctor() {
            maxHealth = 5;
        }

        public override void Initialize(Vector2 position) {
            base.Initialize(position);
            canBeStunned = true;
            canShoot = true;

            if(!isNewEnemy) {
                SetEnabled(true);
                approachCoroutine = Core.StartCoroutine(ChangeState());
            }
        }

        public override void OnAddedToScene() {
            base.OnAddedToScene();
            SetupAnimations();

            var b = AddComponent(new BoxCollider() { 
                LocalOffset = new Vector2(0, 4),
                Width = 10,
                Height = 14
            });
            b.PhysicsLayer = (int)Constants.PhysicsLayers.NPC;
            approachCoroutine = Core.StartCoroutine(ChangeState());
        }

        private void SetupAnimations() {
            this.animator = new SpriteAnimator() { RenderLayer = Constants.RenderLayer_Enemy };

            var runTexture = Scene.Content.LoadTexture(ContentPaths.Doctor + "DocRun.png");
            var runSprites = Sprite.SpritesFromAtlas(runTexture, 20, 28);
            animator.AddAnimation("Run", runSprites.ToArray());

            var attackTexture = Scene.Content.LoadTexture(ContentPaths.Doctor + "DocAttack.png");
            var attackSprites = Sprite.SpritesFromAtlas(attackTexture, 54, 30);

            for(int i = 0; i < 3; i++)
                animator.AddAnimation($"Attack{i}", attackSprites.GetRange(i * 7, 7).ToArray());

            var hurtTexture = Scene.Content.LoadTexture(ContentPaths.Doctor + "DocHurt.png");
            var hurtSprites = Sprite.SpritesFromAtlas(hurtTexture, 19, 27);
            hurtSprites.AddRange(hurtSprites);
            animator.AddAnimation("Hurt", 20, hurtSprites.ToArray());
            animator.AddAnimation("Idle", 8, attackSprites[0]);

            renderer = animator;
            AddComponent(animator);
            animator.Play("Run");

            animator.OnAnimationCompletedEvent += AnimationCompleted;
        }

        private void AnimationCompleted(string name) {
            if(name == "Hurt") {
                if(state == DoctorStates.approach)
                    animator.Play("Run");
                if(state == DoctorStates.attack)
                    animator.Play("Attack2");
            }

            if(name == "Attack2") {
                approachCoroutine = Core.StartCoroutine(ChangeState());
            }
        }

        IEnumerator ChangeState() {
            state = DoctorStates.approach;
            animator.Play("Run");

            yield return Coroutine.WaitForSeconds(1);

            state = DoctorStates.attack;
            animator.Play("Attack2", SpriteAnimator.LoopMode.ClampForever);
        }

        IEnumerator StunCooldownTimer() {
            canBeStunned = false;
            yield return Coroutine.WaitForSeconds(stunCooldown);
            canBeStunned = true;
        }

        IEnumerator SpawnOtherKnives() {
            yield return Coroutine.WaitForSeconds(0.25f);
            Shoot();
            yield return Coroutine.WaitForSeconds(0.25f);
            Shoot();
        }

        public override void Damage(float damage) {
            base.Damage(damage);

            if(canBeStunned) {
                animator.Play("Hurt", SpriteAnimator.LoopMode.ClampForever);
                Core.StartCoroutine(StunCooldownTimer());
            }
        }

        public override void Update() {
            base.Update();

            animator.FlipX = truck.Position.X < Position.X;
            
            CheckRange();
            switch(state) {
                case DoctorStates.approach: ApproachState(); break;
                case DoctorStates.attack: AttackState(); break;
                default: IdleState(); break;
            }
        }

        private void CheckRange() {
            float distance = Vector2.Distance(Position, truck.Position);
            if(distance > range) {
                state = DoctorStates.idle;
                approachCoroutine = null;
            } else {
                if(approachCoroutine == null)
                    approachCoroutine = Core.StartCoroutine(ChangeState());
            }
        }

        private void IdleState() {
            if(animator.CurrentAnimationName != "Idle")
                animator.Play("Idle");
        }

        private void ApproachState() {
            var direction = Vector2.Normalize(truck.Position - Position);

            if(animator.CurrentAnimationName != "Hurt")
                mover.Move(direction, out var result);
        }

        private void AttackState() {
            if(animator.CurrentAnimationName == "Attack2") {
                if(animator.CurrentFrame == 3 && canShoot) {
                    canShoot = false;
                    shootDirection = Vector2.Normalize(truck.Position - Position);
                    Shoot();

                    Core.StartCoroutine(SpawnOtherKnives());
                }
                if(animator.CurrentFrame == 0)
                    canShoot = true;
            }
        }

        private void Shoot() {
            if(Scene == null)       // Don't continue shooting if doctor is dead
                return;

            var knife = Pool<DoctorKnife>.Obtain();
            knife.Initialize(shootDirection, Position);

            if(knife.IsNewProjectile)
                Scene.AddEntity(knife);
        }

        public override void OnDeath() {
            // Spawn death animation
            var deathAnimation = Scene.AddEntity(new AnimatedEntity());

            deathAnimation.Position = Position;

            var texture = Scene.Content.LoadTexture(ContentPaths.Doctor + "DocDeath.png");
            var sprites = Sprite.SpritesFromAtlas(texture, 48, 32);

            deathAnimation.animator.AddAnimation("Death", 20, sprites.ToArray());
            deathAnimation.animator.Play("Death", SpriteAnimator.LoopMode.ClampForever);

            Pool<Doctor>.Free(this);
        }
    }
}
