using System;
using System.Collections.Generic;
using Mocca.DataType;
using Mocca.Physical;

namespace Mocca.Simulator {
	/// <summary>
	/// Superclass for simulators.
	/// </summary>
	public class Simulator {
		public DeviceState state = DeviceState.Unknown;
		public PhysicalDevice device = PhysicalDevice.Unknown;
		public List<MoccaBlockGroup> code = null;

		public void run() {

		}
	}

	/// <summary>
	/// Describing states for physical devices.
	/// </summary>
	public enum DeviceState {
		Standby,    // When simulator is ready for getting code
		Starting,	// When simulator gets code to run
		Running,	// When simulator is running
		Stopping,	// When simulator completes running the code
		Unknown		// When link to simulator is dead, or external causes
	}
}
