
using System;
using System.Collections.Generic;
using Mocca.Compiler;
using Mocca.DataType;
using Mocca.Blocks;

namespace Mocca.Physical {
	public enum PhysicalDevice {
		Microbit,
		Arduino,
		RaspberryPi,
		Unknown
	}

	public class PhysicalParser {
		public Block block;
		public PhysicalDevice device;

		public PhysicalParser(Block block) {
			this.block = block;
			this.device = checkDevices(block);
		}

		public string Parse() {
			switch (device) {
				case PhysicalDevice.Arduino:
					return ParseArduino();
				case PhysicalDevice.Microbit:
					return ParseMicrobit();
				case PhysicalDevice.RaspberryPi:
					return ParseRaspberryPi();
				default:
					throw new FormatException();
			}
		}

		#region Microbit

		/// <summary>
        /// usia
        /// </summary>
        /// <returns></returns>
        public string ParseMicrobit() {
			var type = this.block.type;
			var value = this.block.value;
			switch (type.name) {
				case "DisplayScroll":
					return "display.scroll(" + value[0].ToString() + ")";
				case "DisplayShow":
					return "display.show(" + value[0].ToString() + ")";
				case "Sleep":
					return "sleep(" + value[0].ToString() + ")";
				case "ButtonPressedA":
					return "microbit.button_a.is_pressed():";
				case "ButtonPressedB":
					return "microbit.button_b.is_pressed():";
				case "ButtonPressedAB":
					return "microbit.button_a.is_pressed() and microbit.button_b.is_pressed():";
                case "Pin0Touched":
                    return "pin0.is_touched():";
                case "Pin1Touched":
                    return "pin1.is_touched()";
                case "Pin2Touched":
                    return "pin2.is_touched()";
                case "PlayMusic":
                    return "music.play(music." + value[0].ToString() + "):";
                case "FrequencySet":
                    return "for freq in range(" + value[0].ToString() + "," + value[0].ToString() + "," + value[0].ToString() + "):";
                case "MusicPitch":
                    return "music.pitch(freq, " + value[0].ToString() + ")";
                case "AccelerometerGetX":
                    return "microbit.accelerometer.get_x()";
                case "AccelerometerGetY":
                    return "microbit.accelerometer.get_y()";
                case "AccelerometerGetZ":
                    return "microbit.accelerometer.get_z()";
                default:
					throw new FormatException();
			}
		}

		#endregion Microbit

		#region Arduino

		public string ParseArduino() {
			return null;
		}

		#endregion Arduino

		#region RaspberryPi

		public string ParseRaspberryPi() {
			return null;
		}

		#endregion RaspberryPi

		public PhysicalDevice checkDevices(Block cmd) {
			if (cmd.type.category != BlockCategory.Hardware) {
				return PhysicalDevice.Unknown;
			}

			PhysicalDevice ret;
			switch (cmd.type.extModule) {
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
