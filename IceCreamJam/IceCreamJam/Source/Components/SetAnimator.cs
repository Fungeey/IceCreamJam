using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IceCreamJam.Source.Components {
	class SetAnimator : Component, IUpdatable {
		private SpriteAnimator animator;

		private readonly Dictionary<string, AnimationSet> animationSets;
		private bool switchedAnimationSet = false;

		public AnimationSet CurrentAnimationSet { get; set; }

		public SetAnimator() {
			animationSets = new Dictionary<string, AnimationSet>();
		}

		public override void OnAddedToEntity() {
			this.animator = Entity.GetComponent<SpriteAnimator>();
		}

		public void PlaySet(string name) {
			CurrentAnimationSet = animationSets[name];
			switchedAnimationSet = true;
		}

		public SetAnimator AddAnimationSet(AnimationSet set) {
			animationSets.Add(set.name, set);
			animator.AddAnimationSet(set);
			if (CurrentAnimationSet == null) CurrentAnimationSet = set;
			return this;
		}

		public void Update() {
			CurrentAnimationSet.UpdateSelection();
			if (CurrentAnimationSet.CurrentAnimation != animator.CurrentAnimation) {
				if (switchedAnimationSet) {
					switchedAnimationSet = false;
					animator.Play(CurrentAnimationSet.name + CurrentAnimationSet.CurrentAnimationIndex);
				} else {
					animator.PlayContinuing(CurrentAnimationSet.name + CurrentAnimationSet.CurrentAnimationIndex);
				}
			}
		}

		public List<string> AnimationSetNames => animationSets.Keys.ToList();
	}

	public class AnimationSet {
		public readonly string name;
		private readonly List<SpriteAnimation> animations;
		private readonly Func<int> selector;

		public AnimationSet(string name, Func<int> selector, List<SpriteAnimation> animations) {
			this.name = name;
			this.selector = selector;
			this.animations = animations;
		}

		public AnimationSet(string name, Func<int> selector, params SpriteAnimation[] animations)
			: this(name, selector, new List<SpriteAnimation>(animations)) { }

		public int CurrentAnimationIndex { get; private set; }
		public int Count => animations.Count;
		public SpriteAnimation CurrentAnimation => animations[CurrentAnimationIndex];

		public SpriteAnimation this[int index] => animations[index];

		public void UpdateSelection() {
			CurrentAnimationIndex = selector();
		}
	}

	public static class SpriteAnimatorExt {
		public static SpriteAnimator AddAnimationSet(this SpriteAnimator animator, AnimationSet set) {
			for (int i = 0; i < set.Count; i++)
				animator.AddAnimation(set.name + i, set[i]);
			return animator;
		}
	}
}
