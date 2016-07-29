using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocca {
    namespace Analyser {
        public class SyntaxAnalyser {

            private bool IS_DEBUGGING = true;

            List<Token> token = new List<Token>();

            public SyntaxAnalyser(List<Token> tokenlist) {
                this.token = tokenlist;
            }

            public void Analyse() {
                var root = new TreeNode("translation-unit");
                for(int i = 0; i < token.Count; i++) {
                    var cursor = this.token[i];
                    switch(cursor.GetType()) {
                        
                    }
                }
            }

            /*
             * 제시된 테스트 케이스를 검사하고, 오류 판정일 경우 해당하는 Exception을 던진다.
             * 해당하는 Exception은 호출 부분에서 처리한다.
             */ 
            public void Check(MoccaException e, params bool[] test_cases) {
                foreach(bool test in test_cases) {
                    if(test) {
                        continue;
                    } else {
                        throw e;
                    }
                }
            }

            /*
             * 디버깅용 출력 함수.
             * IS_DEBUGGING이 true일때만 출력한다.
             */
            void print(object a) {
                if (IS_DEBUGGING)
                    Console.WriteLine(a);
            }
        }
    }
}
