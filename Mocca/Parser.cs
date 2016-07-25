using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Mocca {
    public enum ParseMode {
        INTERPRET,
        SOURCE_FILE
    }

    enum TokenType {
        OPERATOR,
        DIVIDER,
        STRING,
        NORMAL
    }

    public class Parser {
        static bool IS_DEBUGGING = true;
        static int PARSER_MINIMUM_VERSION = 1;

        string source;
        ParseMode mode;

        string filePath;
        Dictionary<string, string> fileInfo = new Dictionary<string, string>();

        List<string> extModules = new List<string>();

        public Parser(string source, ParseMode mode) {
            this.mode = mode;
            switch (mode) {
                case ParseMode.INTERPRET:
                    print("ParseMode: INTERPRET");
                    this.source = source;
                    break;
                case ParseMode.SOURCE_FILE:
                    print("ParseMode: SOURCE_FILE");
                    this.filePath = source;
                    preprocess();
                    break;
            }
        }

        private void preprocess() {
            print("Opening " + filePath);
            var xml = XDocument.Load(filePath);

            var versionCheckQuery = from c in xml.Root.Descendants("mocca")
                                    select c.Attribute("version");
            var version = 0;
            foreach(int i in versionCheckQuery) {
                version = i;
                break;
            }
            if(version > PARSER_MINIMUM_VERSION) {
                Console.WriteLine("[-] File Version is not acceptable.");
                return;
            }

            var fileInfoCheckQuery = from c in xml.Root.Descendants("meta")
                        select c.Attribute("name").Value + "|" +
                               c.Attribute("field").Value;

            foreach(string i in fileInfoCheckQuery) {
                string[] a = i.Split('|');
                fileInfo.Add(a[0], a[1]);
            }

            var extModuleCheckQuery = from c in xml.Root.Descendants("mod")
                                      select c.Attribute("name").Value;
            foreach(string i in extModuleCheckQuery) {
                extModules.Add(i);
            }

            var sourceQuery = from c in xml.Root.Descendants("code")
                              select c.Value;
            foreach(string i in sourceQuery) {
                source = i;
                break;
            }
        }

        public void parse() {
            // 어휘 분석 : 토큰, 식별자, 예약어 인식
            var string_mode = false;
            for(int i = 0; i < source.Length; i++) {
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
                        if (tokenRecognize(source[i]) != TokenType.NORMAL) {
                            if(string_mode) {
                                print("문자열 : " + stack);
                            } else {
                                // 숫자 체크
                                stack = stack.Trim();
                                int tempInt;
                                if(int.TryParse(stack, out tempInt) != false) {
                                    print("숫자 : " + stack);
                                } else if(!stack.Equals("") && stack.Trim().Length != 0 && stack != null) {
                                    switch(stack) {
                                        case "if":
                                            print("예약어_만약 : " + stack);
                                            break;
                                        case "elif":
                                            print("예약어_아니고만약 : " + stack);
                                            break;
                                        case "else":
                                            print("예약어_아니면 : " + stack);
                                            break;
                                        case "while":
                                            print("예약어_조건만족 : " + stack);
                                            break;
                                        case "for":
                                            print("예약어_항목순환 : " + stack);
                                            break;
                                        case "block":
                                            print("예약어_블럭묶음 : " + stack);
                                            break;
                                        default:
                                            print("식별자 : " + stack);
                                            break;
                                    }
                                }
                            }

                            switch (tokenRecognize(source[i])) {
                                case TokenType.STRING:
                                    if (string_mode) {
                                        string_mode = false;
                                        keepLoop = false;
                                    } else {
                                        string_mode = true;
                                        keepLoop = false;
                                    }
                                    break;
                                default:
                                    if (string_mode) {
                                        stack += source[i];
                                        i++;
                                    } else {
                                        print("구분자 : " + source[i]);
                                        keepLoop = false;
                                    }
                                    break;
                            }
                        } else {
                            stack += source[i];
                            i++;
                        }
                    }
                    if(i >= source.Length) {
                        break;
                    }
                }
            }
            
        }

        private TokenType tokenRecognize(char src) {
            if (src.Equals('+') || src.Equals('-') || src.Equals('*') || src.Equals('/') || src.Equals('=')) {
                return TokenType.OPERATOR;
            } else if (src.Equals('(') || src.Equals(')') || src.Equals('{') || src.Equals('}') || src.Equals('[') || src.Equals(']') || src.Equals(',') || src.Equals(';')) {
                return TokenType.DIVIDER;
            } else if (src.Equals('\"')) {
                return TokenType.STRING;
            } else {
                return TokenType.NORMAL;
            }
        }

        public string getSource() {
            return source;
        }

        public void setSource(string source) {
            this.source = source;
        }

        public Dictionary<string, string> getFileInfo() {
            return fileInfo;
        }

        void print(object a) {
            if(IS_DEBUGGING)
            Console.WriteLine(a);
        }
    }
}
