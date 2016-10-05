using System;
using System.Collections.Generic;
using System.Linq;

namespace Mocca.Blocks {
	public enum BlockCategory {
		Start,		// Starting blocks include Event block
		Command,	// Normal Commands
		Control,	// Flow controls incluce logic, loop
		Judge,		// true-false-judgement
		Calculate,	// calculating number thingy
		Hardware,	// managing hardware
		External,	// blocks from other modules
		Unknown		// Unknown
	}

	/// <summary>
	/// The type of block.
	/// Includes category, external module name, block name.
	/// If block dosen't have external module, extModule will be null.
	/// </summary>
	public class BlockType {
		public BlockCategory category = BlockCategory.Unknown;
		public string extModule = null;
		public string name = null;

		public BlockType(BlockCategory category, string name) {
			this.category = category;
			this.name = name;
		}

		public BlockType(BlockCategory category, string extModule, string name) {
			this.category = category;
			this.extModule = extModule;
			this.name = name;
		}
	}

	public partial class Block {
		// x, y coordination
		public double x = -1;
		public double y = -1;

		// Blocktype
		public BlockType type;
		// Value on blocks
		public List<object> value = new List<object>();
		// Parent block
		public Block parent;
	}
}
