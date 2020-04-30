using Nez;

namespace IceCreamJam {
	public class ButtonAxis : VirtualAxis.Node {
		public override float Value => _value;
		float _value;
		bool towardsPositive;

		public VirtualButton Positive;
		public VirtualButton Negative;

		public ButtonAxis(VirtualButton negative, VirtualButton positive) {
			Negative = negative;
			Positive = positive;
		}

		public override void Update() {
			if (Positive.IsDown && Negative.IsDown) {
				// new press means change the direction accordingly
				if (Positive.IsPressed && !Positive.IsRepeating)
					towardsPositive = true;
				else if (Negative.IsPressed && !Negative.IsRepeating)
					towardsPositive = false;
				_value = towardsPositive ? Positive.IsPressed ? 1 : 0 : Negative.IsPressed ? -1 : 0;
			} else if (Positive.IsPressed && !Negative.IsDown) {
				_value = 1;
			} else if (Negative.IsPressed && !Positive.IsDown) {
				_value = -1;
			} else {
				_value = 0;
			}
		}
	}
}
