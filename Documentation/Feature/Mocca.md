# Mocca Grammar Introduction

개요
------
본 문서에서는 Mocca 파일을 작성하는 기본 문법에 대하여 설명합니다. 본 설명서의 샘플은 [`Example/middle_lang.mocca`](https://github.com/ngEPL/Mocca/blob/master/Example/middle_lang.mocca)를 참조하십시오.

코드
------
```
<mocca version="1">
    <head>
        <meta name="title" field="안녕, 세상아!"/>
        <meta name="author" field="홍승환"/>
        <meta name="last_edit" field="2016. 06. 29. 12:35"/>
        <meta name="default_level" field="2"/>
        <meta name="physical_type" field="none"/>
    </head>
    <code>
        blockgroup(setVariable,1,2) {
            def(name, "홍승환");
            def(isHuman, true);
            def(age, 20);
            def(loop_count, 0);
            def(fruits, ["사과","배","오렌지","수박","참외"]);
            def(students,{("20163178","홍기훈"),("20163179","홍승환"),("20163180","홍자현"),("20163181","황수진")});
        }

        blockgroup(printAll,3,7) {
        	event(onProgramStart);
            cmd(print,"안녕, 세상아!");
            cmd(print,10);
            cmd(print,textgen("당신의 이름은",name,"입니다."));
            cmd(print,textgen("루트 2는",modcall(math,"sqrt(2)"),"입니다."));
        }

        blockgroup(logicAndLoop,5,10) {
        	event(onKeyPressed, 19);
            if(logic_compare(age, 14, NOT_EQUAL)) {
                cmd(print,"아직 많이 어리네요.");
                cmd(print,"When do you grow up?");
            } elif(logic_compare(age, 19, RIGHT_BIG)) {
                cmd(print,"아직 술은 못 마시네요.");
            } else {
                cmd(print,"술을 마실 수 있는 나이네요.");
            }
            while(logic_compare(loop_count,3,NOT_EQUAL)) {
                cmd(print,textgen("아직 루프를",loop_count,"번 돌았습니다."));
                set(loop_count,eq(loop_count, 1, ADD));
            }
            for(fruits) {
                cmd(print, textgen(__iterator, "은(는) 과일입니다."));
            }
        }
    </code>
</mocca>
```
Mocca 파일은 기본적으로 `.mocca` 확장자를 가지고 있으며, XML 형식으로 기술됩니다. 최상위 노드는 `<mocca>`로, `version` 속성을 가집니다. 이 속성은 Mocca 구현 표준 버전을 의미하며, 컴파일러가 정상적으로 컴파일할 수 있는지를 식별하는 역할을 합니다. 내부 XML은 `<head>`와 `<code>`로 구현이 나뉘어져 있습니다. `<head>`의 경우는, 파일과 작성자 등의, 메타데이터들을 갖고 있습니다. `<code>`는 Mocca 문법에 맞는 코드가 내부에 문자열 형태로 기술되어 있습니다.

`<head>`
------
```
<head>
    <meta name="title" field="안녕, 세상아!"/>
    <meta name="author" field="홍승환"/>
    <meta name="last_edit" field="2016. 06. 29. 12:35"/>
    <meta name="default_level" field="2"/>
    <meta name="physical_type" field="none"/>
</head>
```
`title` 속성은 이 Mocca 프로그램의 이름을 의미합니다. 구동되는 프로그램에 의하여 삽입되어집니다.

`author` 속성은 이 Mocca 프로그램의 작성자를 의미합니다. 구동되는 프로그램에 의하여 삽입되어집니다.

`last_edit` 속성은 이 Mocca 프로그램이 언제 마지막으로 저장되었는가를 정해진 날짜 포맷으로 저장합니다. 구동되는 프로그램에 의하여 삽입되어집니다.

`default_level` 속성은 이 Mocca 프로그램이 실행될 때 어느 정도의 코드 추상화를 거쳐  그려져야 할지를 명시합니다. 1에서 3 사이의 값이 들어갈 수 있으며, 구동되는 프로그램에 의하여 삽입되어집니다.

`physical_type` 속성은 이 Mocca 프로그램이 구현부에 외부 장비를 조작하는 내용이 들어있는지를 EPL이 판단하기 위하여 존재합니다. `microbit, arduino, raspberry` 등의 값이 들어가며, 외부 기기 구현이 없는 경우 `none`으로 명시됩니다. 구동되는 프로그램에 의하여 삽입되어집니다.

`<code>`
------
`<code>` 부분은 문법에 따라 끊어서 설명합니다.

### `blockgroup`

`blockgroup`은 하나의 블록 그룹을 의미하며, 아래와 같이 선언됩니다.

```
blockgroup(setVariable,1,2) {
    // Code here…
}
```

총 3개의 인자를 가집니다. 첫째 인자는 식별자로, 여러 개의 블록 그룹이 병치될 때 각각의 그룹을 구분할 수 있게 해주는 역할을 합니다. 둘째, 셋째 인자는 각각 x, y 좌표로, EPL에서 본 코드를 캔버스에 그릴 때 어느 위치를 기준으로 그려야 하는지를 명시합니다.

### `def(…)`

`def(…)`는 변수를 선언할 때 쓰는 함수입니다.
```
def(name, "홍승환");
```
총 2개의 인자를 가집니다. 첫째 인자는 변수의 이름, 둘째 인자는 변수에 대입될 값이 주어집니다. 주어지는 값의 경우, 컴파일 과정에서 타입이 추론되므로 숫자, 문자열, 배열, 사전 등의 자료형이 주어질 수 있습니다.
`def`로 선언되는 변수 설정은 전역 변수로 취급되어 타 언어로 컴파일 시 최상위에 존재하게 됩니다. 이는 후술되는 `set`과의 다른 점입니다.

### `set(…)`
`set(…)`은 선언된 변수의 값을 재대입할 때 쓰는 함수입니다.
```
set(name, “홍길동”);
```
총 2개의 인자를 가지며, 형태는 `def(…)`와 같습니다. `set`으로 선언되는 변수 설정은 이미 선언되어 있는 변수를 불러와 값을 바꿀 때 사용합니다. 코드 스코프 안에서 변수의 값을 수정하거나 재대입할 때 사용되는 함수입니다.

### `event(…)`
`event(…)`는 특정 이벤트가 발생했을 때 코드를 실행하라는 의미로, 플래그의 역할을 수행합니다.
```
event(onProgramStart)
```
1개 혹은 2개의 인자를 가지며, 첫째 인자에 이벤트가 들어가고 둘째 인자에 해당 이벤트의 속성이 주어집니다. 이벤트의 종류에 따라서 둘째 인자가 필요없는 경우도 있습니다. 이벤트의 종류는 Mocca 코드 내부의 `enum MoccaEventType`을 참조하십시오.

### `cmd(…)`
`cmd(…)`는 명령어 그 자체를 말합니다.
```
cmd(print, “Hello!”);
```
2개의 인자를 가지며, 첫째 인자에 명령어의 이름이, 둘째 인자에 그 명령어의 인자가 전달됩니다. 기본적으로 Python의 명령어 이름을 사용하여 구성됩니다. 타 언어로의 변환 시 별도의 이식 과정이 필요할 수 있습니다.

### `eq(…)`
`eq(…)`는 수학 연산을 의미합니다.
```
eq(loop_count, 1, ADD);
```
3개의 인자를 가집니다. 첫째와 둘째 인자가 각각 좌항과 우항이며, 셋째 인자는 연산식을 의미합니다.

### `modcall(…)`
`modcall(…)`은 외부 모듈을 호출할 때 사용됩니다.
```
modcall(math,"sqrt(2)")
```
2개의 인자를 가집니다. 첫째 인자는 모듈의 이름을, 둘째 인자는 모듈에서 쓰이는 코드가 들어갑니다. 코드는 해당 타겟의 코드가 그대로 들어가며, 각 언어마다 파싱 방법이 달라질 수 있습니다. Python의 경우, 저 부분은 `math.sqrt(2)`로 변환되며, 파일 최상위에 `import math` 구문이 추가됩니다.

### `physical(…)`
`physical(…)`은 Physical Device와의 연동을 위한 명령어입니다.
```
physical(microbit, “display.show(‘A’)”);
```
기본적으로 `modcall(…)`과 유사한 구조를 지니지만, 첫째 인자에 모듈 이름 대신 해당 기기의 이름이 들어간다는 차이점이 있습니다. 이는 코드에서 명시되어 컴파일러가 구분하며, `<head>` 내부의 속성으로 EPL에 의해 한 번 더 명시되어 관리됩니다.

### 논리 구조
Mocca의 논리 분기는 `if-elif-else`를 이용합니다.
```
if(logic_compare(age, 14, NOT_EQUAL)) {
    cmd(print,"아직 많이 어리네요.");
} elif(logic_compare(age, 19, RIGHT_BIG)) {
    cmd(print,"아직 술은 못 마시네요.");
} else {
    cmd(print,"술을 마실 수 있는 나이네요.");
}
```

일반 사용 방법은 여느 프로그래밍 언어와 같으나, 조건의 명시에 블록 구조 최적화를 위한 함수가 존재합니다. `logic_compare(…)`는 논리 구조를 판단하여 `true`와 `false`를 구분해주는 역할을 합니다. 첫째 인자는 좌항, 둘째 인자는 우항이며 셋째 인자는 조건을 의미합니다. 예를 들어, `NOT_EQUAL`은 `!=`를 의미하며, `RIGHT_BIG`은 `<`를 의미합니다.

### 반복 구조 : `while`
```
while(logic_compare(loop_count,3,NOT_EQUAL)) {
    cmd(print,textgen("아직 루프를",loop_count,"번 돌았습니다."));
    set(loop_count,eq(loop_count, 1, ADD));
}
```

`while`문 역시 여느 프로그래밍 언어와 동일한 사용법을 지닙니다. 조건은 논리구조와 마찬가지로 `logic_compare(…)`를 사용합니다.

### 반복 구조 : `for`
Mocca의 `for` 문은 Python의 `for`문 형식을 따릅니다 — 다른 언어의 `foreach` 문과 같이 동작합니다.
```
for(fruits) {
    cmd(print, textgen(__iterator, "은(는) 과일입니다."));
}
```
주어진 배열을 하나씩 순환하며 코드를 실행합니다. `__iterator` 예약어를 통해서 반복 중 각 배열 요소에 접근합니다.

비고
------
본 설명은 Mocca의 개발이 진행됨에 따라 지속적으로 바뀔 수 있으며, Version 1이 고정되지 않았기 때문에 문서 역시 지속적으로 갱신됩니다.