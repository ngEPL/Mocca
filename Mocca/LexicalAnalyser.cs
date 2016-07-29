using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Mocca {
    namespace Analyser {
        public enum ParseMode {
            INTERPRET,
            SOURCE_FILE
        }

        public class LexicalAnalyser {
            static bool IS_DEBUGGING = true; // 디버깅 모드 활성화 여부
            static int PARSER_MINIMUM_VERSION = 1; // 분석기 최하위 호환 버전

            string source; // 원본 소스
            ParseMode mode; // 파싱 모드

            string filePath; // 파일의 경로
            Dictionary<string, string> fileInfo = new Dictionary<string, string>(); // 파일 정보

            List<string> extModules = new List<string>(); // 외부 모듈 정보

            /*
             * 생성자.
             */
            public LexicalAnalyser(string source, ParseMode mode) {
                this.mode = mode;
                switch (mode) {
                    case ParseMode.INTERPRET:
                        print("ParseMode: INTERPRET");
                        this.source = source;
                        break;
                    case ParseMode.SOURCE_FILE:
                        print("ParseMode: SOURCE_FILE");
                        this.filePath = source;
                        Preprocess();
                        break;
                }
            }

            /*
             * ParseMode가 SOURCE_FILE일 때,
             * 파일을 가져와서 소스를 추출하고 파일 정보를 객체에 등록시킨다.
             * 생성자에서 자동으로 호출된다.
             */
            private void Preprocess() {
                print("Opening " + filePath);
                var xml = XDocument.Load(filePath);

                // 버전 확인
                var versionCheckQuery = from c in xml.Root.Descendants("mocca")
                                        select c.Attribute("version");
                var version = 0;
                foreach (int i in versionCheckQuery) {
                    version = i;
                    break;
                }
                if (version > PARSER_MINIMUM_VERSION) {
                    Console.WriteLine("[-] File Version is not acceptable.");
                    return;
                }

                // 파일 정보 추출
                var fileInfoCheckQuery = from c in xml.Root.Descendants("meta")
                                         select c.Attribute("name").Value + "|" +
                                                c.Attribute("field").Value;

                foreach (string i in fileInfoCheckQuery) {
                    string[] a = i.Split('|');
                    fileInfo.Add(a[0], a[1]);
                }

                // 외부 모듈 정보 추출
                var extModuleCheckQuery = from c in xml.Root.Descendants("mod")
                                          select c.Attribute("name").Value;
                foreach (string i in extModuleCheckQuery) {
                    extModules.Add(i);
                }

                // 원본 소스 추출
                var sourceQuery = from c in xml.Root.Descendants("code")
                                  select c.Value;
                foreach (string i in sourceQuery) {
                    source = i;
                    break;
                }
            }

            /*
             * 객체 정보를 바탕으로 어휘 분석을 실시한다.
             * 토큰 정보가 담긴 객체 배열을 반환한다.
             */
            public List<Token> Analyse() {
                var token = new List<Token>();
                // 어휘 분석 : 토큰, 식별자, 예약어 인식
                var string_mode = false;
                for (int i = 0; i < source.Length; i++) {
                    // 문자 스택
                    var stack = "";
                    var keepLoop = true;
                    while (keepLoop) {
                        // 문자열 진행 체크
                        if (string_mode && !source[i].Equals('\"')) {
                            stack += source[i];
                            i++;
                        } else {
                            // 토큰 체크
                            if (TokenRecognize(source[i], i) != TokenType.UNTYPED) {
                                if (string_mode) {
                                    print("문자열 : " + stack);
                                    token.Add(new Token(TokenType.STRING, stack));
                                } else {
                                    // 숫자 체크
                                    stack = stack.Trim();
                                    int tempInt;
                                    if (int.TryParse(stack, out tempInt) != false) {
                                        print("숫자 : " + int.Parse(stack));
                                        token.Add(new Token(TokenType.NUMBER, stack));
                                    } else if (!stack.Equals("") && stack.Trim().Length != 0 && stack != null) {
                                        // 예약어 체크
                                        switch (stack) {
                                            case "if":
                                                print("예약어_만약 : " + stack);
                                                token.Add(new Token(TokenType.IF));
                                                break;
                                            case "elif":
                                                print("예약어_아니고만약 : " + stack);
                                                token.Add(new Token(TokenType.ELIF));
                                                break;
                                            case "else":
                                                print("예약어_아니면 : " + stack);
                                                token.Add(new Token(TokenType.ELSE));
                                                break;
                                            case "while":
                                                print("예약어_조건만족 : " + stack);
                                                token.Add(new Token(TokenType.WHILE));
                                                break;
                                            case "for":
                                                print("예약어_항목순환 : " + stack);
                                                token.Add(new Token(TokenType.FOR));
                                                break;
                                            case "blockgroup":
                                                print("예약어_블럭묶음 : " + stack);
                                                token.Add(new Token(TokenType.BLOCK_GROUP));
                                                break;
                                            case "true":
                                                print("진리값 : " + stack);
                                                token.Add(new Token(TokenType.BOOL, true));
                                                break;
                                            case "false":
                                                print("진리값 : " + stack);
                                                token.Add(new Token(TokenType.BOOL, false));
                                                break;
                                            default:
                                                print("식별자 : " + stack);
                                                token.Add(new Token(TokenType.IDENTIFIER, stack));
                                                break;
                                        }
                                    }
                                }

                                switch (TokenRecognize(source[i], i)) {
                                    case TokenType.STRING:
                                        if (string_mode) {
                                            string_mode = false;
                                            keepLoop = false;
                                        } else {
                                            string_mode = true;
                                            keepLoop = false;
                                        }
                                        break;
                                    case TokenType.OPERATOR:
                                        if (string_mode) {
                                            stack += source[i];
                                            i++;
                                        } else {
                                            print("연산자 : " + source[i]);
                                            token.Add(new Token(TokenType.OPERATOR, source[i]));
                                            keepLoop = false;
                                        }
                                        break;
                                    case TokenType.ASSIGNER:
                                        if (string_mode) {
                                            stack += source[i];
                                            i++;
                                        } else {
                                            print("대입 연산자 : " + source[i]);
                                            token.Add(new Token(TokenType.ASSIGNER, source[i]));
                                            keepLoop = false;
                                        }
                                        break;
                                    case TokenType.LOGIC:
                                        if (string_mode) {
                                            stack += source[i];
                                            i++;
                                        } else {
                                            print("논리 연산자 : " + source[i]);
                                            token.Add(new Token(TokenType.LOGIC, source[i]));
                                            keepLoop = false;
                                        }
                                        break;
                                    case TokenType.LOGIC_2:
                                        if (string_mode) {
                                            stack = stack + source[i] + source[i + 1];
                                            i += 2;
                                        } else {
                                            print("논리 연산자 : " + source[i] + source[i + 1]);
                                            token.Add(new Token(TokenType.LOGIC, source[i] + "" + source[i + 1]));
                                            keepLoop = false;
                                            i++;
                                        }
                                        break;
                                    default:
                                        if (string_mode) {
                                            stack += source[i];
                                            i++;
                                        } else {
                                            print("구분자 : " + source[i]);
                                            token.Add(new Token(TokenType.DIVIDER, source[i]));
                                            keepLoop = false;
                                        }
                                        break;
                                }
                            } else {
                                stack += source[i];
                                i++;
                            }
                        }
                        if (i >= source.Length) {
                            break;
                        }
                    }
                }

                return token;
            }

            /*
             * 토큰을 분석하고 연산자, 대입 연산자, 비교 연산자, 구분자, 문자열 토큰을 걸러낸다. 나머지는 우선 UNTYPED(12)로 처리한다. 
             */
            private TokenType TokenRecognize(char src, int count) {
                if (src.Equals('+') || src.Equals('-') || src.Equals('*') || src.Equals('/')) {
                    return TokenType.OPERATOR;
                } else if (src.Equals('(') || src.Equals(')') || src.Equals('{') || src.Equals('}') || src.Equals('[') || src.Equals(']') || src.Equals(',') || src.Equals(';')) {
                    return TokenType.DIVIDER;
                } else if (src.Equals('\"')) {
                    return TokenType.STRING;
                } else if (src.Equals('>') || src.Equals('<') || src.Equals('=') || src.Equals('!')) {
                    if (src.Equals('=') && !source[count + 1].Equals('=')) {
                        return TokenType.ASSIGNER;
                    } else if (source[count + 1].Equals('=')) {
                        return TokenType.LOGIC_2;
                    } else {
                        return TokenType.LOGIC;
                    }
                } else if((src.Equals('|') && source[count + 1].Equals('|')) || (src.Equals('&') && source[count + 1].Equals('&'))) {
                    return TokenType.LOGIC_2;
                } else {
                    return TokenType.UNTYPED;
                }
            }

            /*
             * 원본 소스를 가져온다.
             */
            public string GetSource() {
                return source;
            }

            /*
             * 원본 소스를 수정한다.
             */
            public void SetSource(string source) {
                this.source = source;
            }

            /*
             * 파일 정보를 Dictionary로 돌려받는다.
             * Key와 Value 모두 문자열로 반환된다.
             */
            public Dictionary<string, string> GetFileInfo() {
                return fileInfo;
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
