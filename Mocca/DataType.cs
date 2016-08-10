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
            public string commandName = null;
            public List<object> commandArgs = null;
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
            public MoccaVariable[] value = null;
        }

        /*
         * 튜플을 의미한다.
         * 키와 값의 쌍으로 되어있다. 키는 문자열만이 성립한다.
         */ 
        public class MoccaTuple : MoccaSuite {
            public string key = null;
            public object value = null;
        }

        /*
         * 
         */ 
        public class MoccaDictionary : MoccaSuite {
            public string name = null;
            public MoccaTuple[] value = null;
        }

        /*
         * 논리 연산을 의미한다.
         * 전위, 후위 요소와 연산자를 포함한다.
         */ 
        public class MoccaExpression : MoccaSuite {
            public object a = null;
            public object b = null;
            public string logic_op = null;
        }

        /*
         * if-elif-else 체인을 의미한다. 
         * 체인이 무조건 완성되지 않아도 성립한다.
         */ 
        public class MoccaLogicChain : MoccaSuite {
            public List<MoccaLogic> chain = new List<MoccaLogic>();
        }

        /*
         * 논리 분기 요소를 의미한다.
         * if, elif, else가 존재할 수 있으며, MoccaLogicChain으로 연결된다.
         * MoccaSuite를 상속하지 않아, 블록 구조에 바로 들어갈 수 없다.
         */ 
        public class MoccaLogic {
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
            public MoccaVariable iter = null;
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
        }
    }
}
