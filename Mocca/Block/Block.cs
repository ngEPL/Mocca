using System;
using System.Collections.Generic;
using System.Linq;

namespace Mocca.Blocks {
	public enum BlockCategory {
		Start,
		Command,
		Control,
		Judge,
		Calculate,
		Hardware,
		External,
		Unknown
	}

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

	public class Block {
		public double x = -1;
		public double y = -1;

		public BlockType type;
		public List<object> value = new List<object>();
		public Block parent;

		public Block(BlockType type, params object[] value) {
			this.type = type;
			this.value = value.ToList<object>();
		}

		public Block(BlockType type, List<object> value) {
			this.type = type;
			this.value = value;
		}
	}
}
