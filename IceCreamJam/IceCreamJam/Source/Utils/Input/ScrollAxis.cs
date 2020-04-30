using Nez;

namespace IceCreamJam {
	public class ScrollAxis : VirtualAxis.Node {
		public override float Value => _value;
		float _value;
		public override void Update() {
			if (Input.MouseWheelDelta >= 120) {
				_value = 1;
			} else if (Input.MouseWheelDelta <= -120) {
				_value = -1;
			} else {
				_value = 0;
			}
		}
	}
}