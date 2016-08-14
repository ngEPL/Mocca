[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](LICENSE.md)
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

Importing Mocca to yours
------
This project includes ```Mocca.sln``` on root. You can open this with Visual Studio, MonoDevelop, Xamarin Studio, etc. Copy ```.cs``` files to your own project to import.

_Now ```.dll``` release version preparing._

Basic Usage
------
_Now only Parse Tree Printing is available. Still Developing!_

Add this phrase on your ```.cs``` file:

```
import Mocca.Compiler
…
Compiler c = new Compiler("<source file directory>", CompileMode.FILE_PASS);
var result = c.Parse();
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
