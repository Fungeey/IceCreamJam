using Microsoft.Xna.Framework.Graphics;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IceCreamJam.Effects {
	public class UIMaskEffect : Effect {
		private EffectParameter progressParam;
		private float progress = 0f;

		[Range(0, 1)]
		public float Progress {
			get => progress;
			set {
				if (progress != value) {
					progress = value;
					progressParam.SetValue(value);
				}
			}
		}

		public UIMaskEffect(GraphicsDevice graphicsDevice, byte[] effectCode) : base(graphicsDevice, effectCode) {
			progressParam = Parameters["Progress"];
			progressParam.SetValue(progress);
		}
	}
}
