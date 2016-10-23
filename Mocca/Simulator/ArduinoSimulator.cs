using System;
using System.Collections.Generic;
using Mocca.DataType;
using Mocca.Physical;

namespace Mocca.Simulator.Arduino {
	public enum Accessory {
		LED,
		DCMotor,
		SERVOMotor,
		LCD,
		Piezo,
		PhotoResistor,
		PushButton,
		TempSensor,
		TiltSensor,
		External,
		Unknown
	}

	public enum Port {
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

		private Dictionary<Port, Accessory> map; // = new Dictionary<Port, Accessory>();
		private List<MoccaVariable> variable;

		public ArduinoSimulator(Dictionary<Port, Accessory> map) {
			state = DeviceState.Standby;
			device = PhysicalDevice.Arduino;
		}

		public ArduinoSimulator(List<MoccaBlockGroup> code, Dictionary<Port, Accessory> map) {
			state = DeviceState.Starting;
			device = PhysicalDevice.Arduino;
			this.code = code;
		}

		public void setCode(List<MoccaBlockGroup> code) {
			state = DeviceState.Starting;
			this.code = code;
		}

		public override void run() {
			state = DeviceState.Running;

			// Main Loop
			foreach(MoccaBlockGroup blockgroup in code) {
				Console.WriteLine(blockgroup.name);
				foreach (MoccaSuite line in blockgroup.suite) {
					Console.WriteLine(line.GetType());
				}
			}
		}
	}
}
