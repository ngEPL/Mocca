using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocca {
    public enum MoccaError {
        SYNTAX_ERROR,          // 0 문법 오류
        ARITHMETICS_ERROR,     // 1 수학적 연산 오류
        NULL_ERROR             // 2 참조 값 없음 오류
        // TODO: 발견하는 대로 계속 추가할 것
    }

    /*
     * 전체 Exception의 최상위 클래스.
     * Mocca에서 발생하는 모든 예외는 이 클래스를 상속한다.
     */ 
    public class MoccaException : Exception {
        public string error_name;
        public MoccaError error_type;
        public string error_desc; 
    }

    /*
     * 블록 묶음 구조 선언 오류.
     */ 
    public class MoccaSyntaxException : MoccaException {
        public MoccaSyntaxException(string error_char) {
            error_name = "문법 오류";
            error_type = MoccaError.SYNTAX_ERROR;
            error_desc = "'" + error_char + "'이(가) 필요합니다.";
        }
    }

    /*
     * // 샘플
     * public class [내가 준 예외 이름] : MoccaException {
     *      public [내가 준 예외 이름]() {
     *             error_name = "에러 이름";
     *             error_type = MoccaError.적절한에러타입 // MoccaError enum 참고
     *             error_desc = "에러 상세 설명";
     *      }
     * }
     */ 
}
