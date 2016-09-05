using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocca {
    namespace DataType {
        public enum MoccaType {
            NUMBER,
            STRING,
            BOOL,
            ARRAY,
            DICTIONARY,
            UNTYPED
        }

        /*
         * 하나의 블록 그룹을 의미한다.
         * x 좌표과 y 좌표가 포함되어 있으며, MoccaSuite를 포함한다.
         */ 
        public class MoccaBlockGroup {
			public string name = null;
            public int x = 0;
            public int y = 0;
            public List<MoccaSuite> suite = null;
        }

        /*
         * 명령과 논리의 집합을 의미한다.
         * 빈 클래스로써, 단일 명령 단위는 모두 이 클래스를 상속받는다.
         */ 
        public class MoccaSuite { }

        /*
         * 명령을 의미한다.
         * 명령 이름과 인자로 구성되어 있다.
         */ 
        public class MoccaCommand : MoccaSuite {
            public string name = null;
            public List<object> args = null;

			public MoccaCommand(string name, List<object> args) {
				this.name = name;
				this.args = args;
			}
        }

        /*
         * 변수를 의미한다.
         * 이름과 값, 타입이 정해져있다.
         */ 
        public class MoccaVariable : MoccaSuite {
            public string name = null;
            public object value = null;
            public MoccaType type = MoccaType.UNTYPED;
        }

        /*
         * 배열을 의미한다.
         * 이름과 MoccaVariable의 배열을 가진다.
         */ 
        public class MoccaArray : MoccaSuite {
            public string name = null;
			public List<object> value = null;
        }

        /*
         * 튜플을 의미한다.
         * 키와 값의 쌍으로 되어있다. 키는 문자열만이 성립한다.
         */ 
        public class MoccaTuple : MoccaSuite {
            public string key = null;
            public object value = null;

			public MoccaTuple(string key, object value) {
				this.key = key;
				this.value = value;
			}
        }

        /*
         * 사전을 의미한다.
         * 이름과 튜플의 집합을 의미한다.
         */ 
        public class MoccaDictionary : MoccaSuite {
            public string name = null;
            public List<MoccaTuple> value = null;

			public MoccaDictionary() {
				value = new List<MoccaTuple>();
			}
        }

        /*
         * 논리 연산을 의미한다.
         * 전위, 후위 요소와 연산자를 포함한다.
         */ 
        public class MoccaExpression : MoccaSuite {
            public object a = null;
            public object b = null;
            public string logic_op = null;
			public string atom_evaluation = null;

			public MoccaExpression(object a, object b, string logic_op) {
				this.a = a;
				this.b = b;
				this.logic_op = logic_op;
			}

			public MoccaExpression(string atom) {
				this.atom_evaluation = atom;
			}
        }

        /*
         * 논리 분기 요소를 의미한다.
         * if, elif, else가 존재할 수 있다.
         */ 
        public class MoccaLogic : MoccaSuite {
            public string keyword = null;
            public MoccaExpression expression = null;
            public List<MoccaSuite> cmd_list = new List<MoccaSuite>();
        }

        /*
         * while 반복문을 의미한다.
         * 조건문과 그에 해당하는 MoccaSuite를 포함한다.
         */ 
        public class MoccaWhile : MoccaSuite {
            public MoccaExpression expression = null;
            public List<MoccaSuite> cmd_list = null;
        }

        /*
         * for 반복문(Inhanced)을 의미한다.
         * iterator와 그에 해당하는 MoccaSuite를 포함한다.
         */ 
        public class MoccaFor : MoccaSuite {
            public object iter = null;
            public List<MoccaSuite> cmd_list = new List<MoccaSuite>();
        }
        
        /*
         * 계산식을 의미한다.
         * 전위, 후위 요소와 연산자를 포함한다.
         */ 
        public class MoccaEquation : MoccaSuite {
            public object a = null;
            public object b = null;
            public string op = null;

			public MoccaEquation(object a, object b, string op) {
				this.a = a;
				this.b = b;
				this.op = op;
			}
        }
    }
}
