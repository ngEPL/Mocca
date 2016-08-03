using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mocca {
    public enum CompileMode {
        FILE_PASS,
        XML_SOURCE_PASS,
        PURE_SOURCE_PASS
    }

    public class Compiler {
        float COMPILER_MINIMUM_VERSION = 1.0f;

        Dictionary<string, string> fileInfo = new Dictionary<string, string>();
        List<string> extModules = new List<string>();

        string filePath;
        string source = null; 

        public Compiler(String source, CompileMode mode) {
            switch(mode) {
                case CompileMode.FILE_PASS:
                    this.filePath = source;
                    Preprocess(CompileMode.FILE_PASS);
                    break;
                case CompileMode.PURE_SOURCE_PASS:
                    this.source = source;
                    break;
                case CompileMode.XML_SOURCE_PASS:
                    this.source = source;
                    Preprocess(CompileMode.XML_SOURCE_PASS);
                    break;
            }
        }

        private void Preprocess(CompileMode mode) {

            XDocument xml;
            if(mode == CompileMode.FILE_PASS) {
                xml = XDocument.Load(filePath);
            } else {
                xml = XDocument.Parse(source);
            }
            
            // 버전 확인
            var versionCheckQuery = from c in xml.Root.Descendants("mocca")
                                    select c.Attribute("version");
            var version = 0;
            foreach (int i in versionCheckQuery) {
                version = i;
                break;
            }
            if (version > COMPILER_MINIMUM_VERSION) {
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

        public ParseTree Parse() {
            Scanner s = new Scanner();
            Parser p = new Parser(s);
            return p.Parse(source);
        }
    }
}
