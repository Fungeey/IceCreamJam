using Microsoft.Xna.Framework.Input;
using Nez;

namespace IceCreamJam {
	static partial class InputManager {
		/// <summary>
		/// negative values are up, positive values are down
		/// </summary>
		public static readonly VirtualAxis yAxis;
		/// <summary>
		/// negative values are left, positive values are right
		/// </summary>
		public static readonly VirtualAxis xAxis;

		/// <summary>
		/// x: negative values are left, positive values are right<br/>
		/// y: negative values are up, positive values are down
		/// </summary>
		public static readonly VirtualJoystick joystick;

		public static readonly VirtualButton brake;
		public static readonly VirtualButton dash;
		public static readonly VirtualButton dispense;

		public static readonly VirtualButton shoot;
		public static readonly VirtualIntegerAxis switchWeapon;

		static InputManager() {
			xAxis = new VirtualAxis();
			xAxis.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.A, Keys.D));
			xAxis.Nodes.Add(new VirtualAxis.GamePadLeftStickX());

			yAxis = new VirtualAxis();
			yAxis.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.W, Keys.S));
			yAxis.Nodes.Add(new VirtualAxis.GamePadLeftStickY());

			joystick = new VirtualJoystick(false);
			joystick.AddKeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.A, Keys.D, Keys.W, Keys.S);
			joystick.AddGamePadLeftStick();

			brake = new VirtualButton();
			brake.AddKeyboardKey(Keys.LeftShift);
			// TODO: add gamepad button for braking

			dash = new VirtualButton();
			dash.AddKeyboardKey(Keys.Space);
			// TODO: add gamepad button for dashing

			dispense = new VirtualButton();
			dispense.AddKeyboardKey(Keys.F);
			// TODO: add gamepad button for dispensing

			shoot = new VirtualButton();
			shoot.AddMouseLeftButton();
			shoot.AddGamePadRightTrigger(0, 0.2f);

			var prevKey = new VirtualButton(new VirtualButton.KeyboardKey(Keys.Q));
			var nextKey = new VirtualButton(new VirtualButton.KeyboardKey(Keys.E));
			var prevGamepad = new VirtualButton(new VirtualButton.GamePadButton(0, Buttons.X));
			var nextGamepad = new VirtualButton(new VirtualButton.GamePadButton(0, Buttons.A));

			switchWeapon = new VirtualIntegerAxis();
			switchWeapon.Nodes.Add(new ButtonAxis(prevKey, nextKey));
			switchWeapon.Nodes.Add(new ScrollAxis());
			switchWeapon.Nodes.Add(new ButtonAxis(prevGamepad, nextGamepad));
		}
	}
}
