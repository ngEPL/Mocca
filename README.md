[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](LICENSE.md)
Mocca
======
개요
------
_You can find README written by English. [Check here.](README.en.md)_

Mocca는 C# 기반의 블록 코딩 기반 EPL(Educational Programming Language, 교육용 프로그래밍 언어) 제작에 최적화된 중간 언어입니다. ngEPL을 위하여 개발이 시작되었으며, MIT License로 공개되어 누구나 사용할 수 있습니다.
내부 자체 라이브러리를 통해서, Mocca는 하나의 파일을 다양한 언어로 변환할 수 있습니다. 많은 ```import```로 언어 구조를 망가뜨리지 않고, 교육에 적합한 간단하고 명확한 구조로 언어를 재구성해 변환합니다.
의존성 없이 독립된 C# 라이브러리의 형태를 하고 있으며, [Xamarin](https://www.xamarin.com)과 [Mono](http://www.mono-project.com)에서도 작동하여, [Unity Engine](http://unity3d.com)과 같은 다양한 C# 기반 환경에서 작동합니다.

변환 언어
------
현재, Mocca가 기본적으로 지원하거나, 지원을 목표하는 언어는 다음과 같습니다.

* Python
* Javascript
* C#
* Java

_이외의 언어는 Mocca 확장 모듈을 제작하여 직접 추가하거나, 개발 계획의 수정으로 추가될 수 있습니다._

의존성
------
별도 외부 라이브러리로의 의존성은 존재하지 않으며, C#이 실행 가능한 환경에서 모두 실행될 수 있습니다.

프로젝트 적용 방법
------
이 프로젝트는 루트 폴더에 ```Mocca.sln```을 포함하고 있습니다. 이 파일을 Visual Studio, MonoDevelop, Xamarin Studio 등에서 실행하여 솔루션을 살펴보십시오. ```.cs``` 파일을 적용을 원하는 프로젝트로 복사하여 사용하십시오.

_추후 ```.dll``` Release Version이 배포됩니다._

기본 사용 방법
------
_현재는 Parse Tree의 출력까지가 지원됩니다._
사용을 원하는 C# 파일에 다음 문구를 추가하십시오.
```
import Mocca.Compiler
…
Compiler c = new Compiler("소스 파일 경로", CompileMode.FILE_PASS);
var result = c.Parse();
```
파일을 직접 핸들링하지 않고,소스 자체를 이용한 방법이 있습니다. 자세한 내용은 [위키를 참조하십시오.](http://github.com/ngEPL/Mocca)

라이센스
------
Mocca는 MIT License 하에 배포됩니다. 출처와 저작권을 명시하는 조건 하에 배포, 수정, 상업적 이용이 가능합니다. (http://opensource.org/licenses/MIT)

개발 정보
------
현재 이 프로젝트는 미래창조과학부에서 주관하는 __2016년도 창의도전형 SW R&D 지원 사업__ 의 지원을 받고 있습니다. 자세한 정보는 [링크를 참조하십시오.](http://www.swrnd.or.kr/korean/viewtopic.php?t=1715)

[월간 개발 보고서 (7월)](./Documentation/Monthly/log_april.md)
