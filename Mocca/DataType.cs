using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocca {
    namespace DataType {
        enum MoccaType {
            NUMBER,
            STRING,
            ARRAY,
            DICTIONARY,
            UNTYPED
        }

        class MoccaCommand {
            public string commandName = null;
            public List<object> commandArgs = null;
        }

        class MoccaBlockGroup {
            public int x = -1;
            public int y = -1;
            public List<MoccaSuite> suite = null;
        }

        class MoccaSuite {}

        class MoccaVariable : MoccaSuite {
            public string name = null;
            public object value = null;
            public MoccaType type = MoccaType.UNTYPED;
        }

        class MoccaExpression : MoccaSuite {
            public object a = null;
            public object b = null;
            public string comparer = null;
        }

        class MoccaLogicChain : MoccaSuite {
            public List<MoccaLogic> chain = new List<MoccaLogic>();
        }

        class MoccaLogic {
            public string keyword = null;
            public MoccaExpression expression = null;
            public List<MoccaCommand> cmd_list = new List<MoccaCommand>();
        }

        class MoccaWhile : MoccaSuite {
            public MoccaExpression expression = null;
            public List<MoccaCommand> cmd_list = null;
        }

        class MoccaFor : MoccaSuite {
            public MoccaVariable iter = null;
            public List<MoccaCommand> cmd_list = new List<MoccaCommand>();
        }
        
        class MoccaEquation : MoccaSuite {
            public object a = null;
            public object b = null;
            public string op = null;
        }
    }
}
