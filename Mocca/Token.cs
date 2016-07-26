using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocca {
    public enum TokenType {
        OPERATOR,    // 0 연산자
        ASSIGNER,    // 1 대입 연산자
        LOGIC,    // 2 논리 연산자
        LOGIC_2,  // 3 2자 논리 연산자(파싱 중 임시로 사용됨, COMPARER로 합쳐져 출력됨)
        DIVIDER,     // 4 구분자
        STRING,      // 5 문자열
        NUMBER,      // 6 숫자
        IF,          // 7 만약
        ELIF,        // 8 아니고 만약
        ELSE,        // 9 아니면
        FOR,         // 10 항목순환
        WHILE,       // 11 조건순환
        BLOCK_GROUP, // 12 블럭 묶음
        IDENTIFIER,  // 13 식별자
        UNTYPED      // 14 구분 안됨 (파싱 중 임시로 사용됨, 출력 결과에 미포함)
    }

    public class Token {
        private TokenType type;
        private object value;

        /*
         * 생성자 (2개의 오버로드)
         * 1. 타입만 받는 토큰의 경우 타입만 받음
         * 2. 타입과 값이 같이 오는 토큰의 경우 둘 다 받음
         */
        public Token(TokenType type) {
            if (type == TokenType.IF || type == TokenType.ELIF || type == TokenType.ELSE || type == TokenType.FOR
            || type == TokenType.WHILE || type == TokenType.BLOCK_GROUP) {
                this.type = type;
                this.value = "None";
            } else throw new Exception(); // TODO: Exception 정의 후 추가해야 함
        }

        public Token(TokenType type, object value) {
            this.type = type;
            this.value = value;
        }

        /*
         * 멤버 변수 Getter, Setter
         */
        public void SetType(TokenType type) {
            this.type = type;
        }

        public new TokenType GetType() {
            return type;
        }

        public void SetValue(object value) {
            this.value = value;
        }

        /*
         * 반환이 object 형태이므로 지역 변수에서 var 키워드로 받아야 한다.
         */
        public object GetValue() {
            return value;
        }
    }
}
