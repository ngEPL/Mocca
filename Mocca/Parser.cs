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
            print("PARSE START");
        }

        void print(string a) {
            if(IS_DEBUGGING)
            Console.WriteLine(a);
        }
    }
}
