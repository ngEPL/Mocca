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
            LexicalAnalyser source_file = new LexicalAnalyser("..\\..\\..\\Example\\middle_lang.mocca", ParseMode.SOURCE_FILE);
            LexicalAnalyser interpret = new LexicalAnalyser("name = \"홍승환\"; age = 20;", ParseMode.INTERPRET);

            var syntax = source_file.Analyse(); // List<Token>

            SyntaxAnalyser semantic = new SyntaxAnalyser(syntax);
            semantic.Analyse();
        }
    }
}
