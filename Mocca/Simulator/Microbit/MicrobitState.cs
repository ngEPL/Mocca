using System;
using System.Collections.Generic;

namespace Mocca.Simulator.Microbit {
	public class MicrobitState : DeviceState {
		public Dictionary<string, object> variables;

		public bool[][] led = new bool[][] {
			new bool[] {false, false, false, false, false},
			new bool[] {false, false, false, false, false},
			new bool[] {false, false, false, false, false},
			new bool[] {false, false, false, false, false},
			new bool[] {false, false, false, false, false}
		};

		public byte led_brightness = 255;

		public bool button_a = false;
		public bool button_b = false;

		public bool pin_0 = false;
		public bool pin_1 = false;
		public bool pin_2 = false;
		public bool pin_3V = false;
		public bool pin_GND = false;
	}
}
