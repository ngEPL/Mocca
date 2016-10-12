using System;
using System.Collections.Generic;
using Mocca.DataType;
using Mocca.Physical;

namespace Mocca.Simulator.Arduino {
	public enum Accessory {
		LED,
		Motor,
		// TODO : Add more Accessory
		External,
		Unknown
	}

	public enum Port { // TODO : Add comments for dev
		DigitalSCL,
		DigitalSDA,
		DigitalAREF,
		DigitalGND,
		Digital13,
		Digital12,
		Digital11PWM,
		Digital10PWM,
		Digital9PWM,
		Digital8,
		Digital7,
		Digital6PWM,
		Digital5PWM,
		Digital4,
		Digital3PWM,
		Digital2,
		Digital1TX,
		Digital0RX,

		PowerIOREF,
		PowerRESET,
		Power3_3V,
		Power5V,
		PowerGND3,
		PowerGND5,
		PowerVIn,

		Analog0,
		Analog1,
		Analog2,
		Analog3,
		Analog4,
		Analog5
	}

	public class ArduinoSimulator : Simulator {

		Dictionary<Port, Accessory> map; // = new Dictionary<Port, Accessory>();

		public ArduinoSimulator(List<MoccaBlockGroup> code) {
			state = DeviceState.Standby;
			device = PhysicalDevice.Arduino;
			this.code = code;
		}
	}
}
