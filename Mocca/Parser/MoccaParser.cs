using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Mocca.Compiler;

namespace Mocca {
	/// <summary>
	/// Enum for compile session mode.
	/// </summary>
	public enum CompileMode {
		/// <summary>
		/// This is for passing whole file to compiler.
		/// </summary>
        FILE_PASS,
		/// <summary>
		/// This is for passing string which have both of xml anotation and source.
		/// </summary>
        XML_SOURCE_PASS,
		/// <summary>
		/// This is for passing string which have only pure source.
		/// </summary>
        PURE_SOURCE_PASS
    }

	/// <summary>
	/// Fully-intergrated compiling engine.
	/// </summary>
    public class MoccaParser {
		/// <summary>
		/// Reference for compiling mocca code.
		/// If your code's version is smaller than this, you can't compile your code with this version of compiler.
		/// </summary>
        public readonly float COMPILER_MINIMUM_VERSION = 1.0f;

		/// <summary>
		/// File information.
		/// Initialize when Preprocess() calls.
		/// </summary>
        public Dictionary<string, string> fileInfo = new Dictionary<string, string>();

		/// <summary>
		/// This list have all extension modules on your file.
		/// Initialize when Preprocess() calls.
		/// </summary>
        List<string> extModules = new List<string>();

		/// <summary>
		/// The file path.
		/// </summary>
        string filePath;

		/// <summary>
		/// The source.
		/// </summary>
        string source;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Mocca.Compiler"/> class.
		/// </summary>
		/// <param name="source">The source depends on your CompileMode.</param>
		/// <param name="mode">The Compile session mode.</param>
        public MoccaParser(String source, CompileMode mode) {
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

		/// <summary>
		/// Preprocess your code with the specified mode.
		/// This method will be automatically called when constructor calls.
		/// </summary>
		/// <param name="mode">The specified compile mode.</param>
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

		/// <summary>
		/// Parse your code with given compile mode.
		/// </summary>
        public ParseTree Parse() {
            Scanner s = new Scanner();
            Parser p = new Parser(s);
            ParseTree t = p.Parse(source);
			return t;
        }
    }
}
