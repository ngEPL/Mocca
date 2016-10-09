[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](LICENSE.md) [![Travis CI](https://api.travis-ci.org/ngEPL/Mocca.svg)](https://travis-ci.org/ngEPL/Mocca)
Mocca
======
Introduction
------
_한국어로 쓰여진 문서가 존재합니다. [여기를 참조하십시오.](README.md)_

Mocca is an intermediate language which is specialized for making block-coding-based EPL(Educational Programming Language) based on C#. It started for ngEPL, and opened under MIT License.

With internal library, Mocca compiles one ```.mocca``` file to many other languages. Without messing the language structure with lots of ```import```, Mocca compiles with constructing language to nice and clear structure which can work with CS education.

This is no-dependency standalone C# library, and We checked this works on [Xamarin](https://www.xamarin.com), [Mono](http://www.mono-project.com). So you can use this on many C# based environment like [Unity Engine](http://unity3d.com).

Target Languages
------
Currently, Languages that Mocca supports or planned to support is:

* Python
* Javascript
* C#
* Java

_You can make Mocca Extension Module to add some languages. Or with development plan changes, I’ll add other language._

Dependencies
------
No external library required. Mocca works on all environment which C# can run.

Importing Mocca to Your Project
------
You can build this project to `.dll` format. Build this project, and add `.dll` file to your own C# project.

Basic Usage
------
Add this phrase on your ```.cs``` file:

```
import Mocca.Compiler
…
// Make Parser with your .mocca file.
var parser = new MoccaParser(
			"../../../Example/middle_lang.mocca", 
			CompileMode.FILE_PASS
			);

// Make ParseTree.
var tree = parser.Parse();

// Evaluate your tree, and make block list(List<MoccaBlockGroup>).
var eval = tree.Eval();

// Print your ParseTree to String
Console.WriteLine(tree.PrintTree());

// Make Python Compiler.
var compiler = new PythonCompiler(eval);

// Compile to Python.
Console.WriteLine(compiler.Compile());
```

We have solution for using pure Mocca source file. You can find this on [our Wiki.](http://github.com/ngEPL/Mocca)

License
------
Mocca is licensed under the MIT license. (http://opensource.org/licenses/MIT)

Development Information
------
Currently this project is supporting by __2016 Creative-Challenge Software R&D Supporting Business__, managing by Ministry of Science, ICT and Future Planning(Korea, the Republic of.). You can find detail informations in [here.](http://www.swrnd.or.kr/korean/viewtopic.php?t=1715)

_All files are written by Korean._

[Monthly Development Report (April)](./Documentation/Monthly/log_april.md)
[Monthly Development Report (August)](./Documentation/Monthly/log_august.md)
[Mocca Grammar Introduction](./Documentation/Feature/Mocca.md)
