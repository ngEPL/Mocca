using System;
using Mocca.Physical;
using Mocca.Simulator.Microbit;

namespace Mocca.Simulator {
	public class Simulator {
		public PhysicalDevice device = PhysicalDevice.Unknown;
		public DeviceState state;
		public Simulator(PhysicalDevice device) {
			this.device = device;
			this.state = initState(device);
		}

		public int run() {
			while(true) {
				if (state.panicCode == 1) {
					return 0;
				}
			}
		}

		public DeviceState initState(PhysicalDevice device) {
			switch (device) {
				case PhysicalDevice.Microbit:
					return new MicrobitState();
				default:
					throw new FormatException();
			}
		}
	}

	public class DeviceState {
		public bool isUpdated = false;
		public int panicCode = -1;
	}
}
