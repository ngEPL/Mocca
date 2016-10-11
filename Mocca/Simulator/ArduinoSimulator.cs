using System;
using System.Collections.Generic;
using Mocca.DataType;
using Mocca.Physical;

namespace Mocca.Simulator {
	public class ArduinoSimulator : Simulator {
		public ArduinoSimulator(List<MoccaBlockGroup> code) {
			state = DeviceState.Standby;
			device = PhysicalDevice.Arduino;
			this.code = code;
		}
	}
}
