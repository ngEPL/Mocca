using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocca.Analyser;

namespace Mocca {
    class Program {
        static void Main(string[] args) {
            /*
             * SyntaxAnalyser 객체를 선언함
             * 1. SOURCE_FILE 모드
             * 2. INTERPRET   모드
             */
            SyntaxAnalyser source_file = new SyntaxAnalyser("..\\..\\..\\Example\\middle_lang.mocca", ParseMode.SOURCE_FILE);
            SyntaxAnalyser interpret = new SyntaxAnalyser("name = \"홍승환\"; age = 20;", ParseMode.INTERPRET);

            var syntax = source_file.SyntaxAnalyse(); // List<Token>

            SemanticAnalyser semantic = new SemanticAnalyser(syntax);
            semantic.SemanticAnalyse();
        }
    }
}
