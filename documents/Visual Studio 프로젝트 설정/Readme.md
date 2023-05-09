# Readme

이 문서는 이 프로젝트를 개발하면서 Visual Studio 프로젝트를 변경한 이력을 기록한다.


## 항상 관리자 권한으로 실행하기

 * `솔루션 탐색기` > `Your 프로젝트` 선택, 마우스 우클릭 > `속성` > `보안탭`
   * [v] ClickOnce 보안 설정 사용 체크
 * `솔루션 탐색기` > `Your 프로젝트` > `Properties` > app.manifest

````
<!--변경전-->
<requestedExecutionLevel  level="asInvoker" uiAccess="false" />

<!--변경후-->
<requestedExecutionLevel  level="requireAdministrator" uiAccess="false" />
````

## 안전하지 않은 코드 허용
 * `솔루션 탐색기` > `Your 프로젝트` 선택, 마우스 우클릭 > `빌드탭`
   * [v] 안전하지 않은 코드 허용