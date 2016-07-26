using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocca {
    namespace Analyser {
        public class SemanticAnalyser {
            List<Token> token_list;

            public SemanticAnalyser(List<Token> token_list) {
                this.token_list = token_list;
            }

            public void SemanticAnalyse() {
                foreach (Token i in token_list) {
                    Console.WriteLine(i.GetType().ToString() + " 타입의 " + i.GetValue().ToString());
                }
            }
        }
    }
}
