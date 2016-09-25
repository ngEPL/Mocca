using System;
using System.Collections.Generic;
using Mocca.Compiler;
using Mocca.DataType;

namespace Mocca.Physical {
	public enum PhysicalDevice {
		Microbit,
		Arduino,
		RaspberryPi,
		Unknown
	}

	public class PhysicalParser {
		public MoccaCommand cmd;
		public PhysicalDevice device;

		public PhysicalParser(MoccaCommand cmd) {
			this.cmd = cmd;
			this.device = checkDevices(cmd);
		}

		public PhysicalDevice checkDevices(MoccaCommand cmd) {
			PhysicalDevice ret;
			switch (cmd.args[0].ToString()) {
				case "microbit":
					ret = PhysicalDevice.Microbit;
					break;
				case "arduino":
					ret = PhysicalDevice.Arduino;
					break;
				case "rbpi":
					ret = PhysicalDevice.RaspberryPi;
					break;
				default:
					ret = PhysicalDevice.Unknown;
					break;
			}
			return ret;
		}
	}
}