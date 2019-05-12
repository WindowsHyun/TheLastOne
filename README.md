# TheLastOne

----------
개발자
----------
+ 2012181042 황성섭
+ 2012180004 권창현
+ 2014184040 송슬기

---------
연구 목적
---------
+ Unity3D 엔진을 이용한 PC 게임 제작
+ IOCP 서버를 이용한 최대 50명의 유저 접속 및 플레이
+ 일정에 맞는 분할 작업으로 팀프로젝트 협업 능력 배양
+ 팀원과의 주 2회 이상 커뮤티케이션을 통해 기획 의도에 맞게 개발 진행 방향을 확인

---------
게임 소개
---------
![Image01](https://i.imgur.com/2erj7tM.png)
+ 장르 : 3인칭 슈팅 게임(TPS)
+ 시점 : 쿼터뷰 + 백뷰
+ 플랫폼 : PC
+ 커스텀 아이템을 통해서 플레이어 마다 개성을 표현할 수 있으며, 최대 50명의 플레이어가 전투, 생존을 하며 최후의 1인으로 살아남는 게임

---------
개임 특징
---------
![Image02](https://i.imgur.com/zLsZ7rJ.png)
1. 이동
    + 캐릭터의 이동속도 : 6unit/s
    + 차량의 이동속도 : 20unit/s
    + 차랑을 이용하면 원하는 지역으로 빠르게 이동 할 수 있다.
    
![Image03](https://i.imgur.com/JcF4WGR.png)

2. 안전지역
    + 게임 시작 시 랜덤으로 안전지역이 설정된다.
    + 안전지역의 중심을 기준으로 원이 줄어든다.
    + 원이 작아질수록 안전지역 바깥의 데미지가 증가한다.

3. 안전지역 크기 및 시간
    + 1번째 원 1500*1500 유지 4m     축소 4m
    + 2번째 원  800*800  유지 2m 30s 축소 2m
    + 3번째 원  400*400  유지 1m 30s 축소 1m
    + 4번째 원  200*200  유지 1m     축소 30s
    + 5번째 원  100*100  유지 1m    축소 20s
    + 6번째 원   50*50   유지 30s   축소 10s

---------
개발 환경
---------
![Image04](https://i.imgur.com/jG7qnw0.png)

---------
기술적 요소 및 중점 연구 분야
---------
 1. 공통
    + Github와 SourceTree를 활용한 팀프로젝트 관리

 2. 클라이언트
    + 유니티를 활용하여 게임 시스템 구현(Physics, NavMesh, Effect 등...)
    + 유니티의 UGUI를 통한 게임 내 UI 구현

 3. 서버
    + IOCP 소켓 모델을 이용해 유니티(C#) 클라이언트와 서버 연동
    + 구글에서 제작한 FlatBuffers를 통한 효율적인 데이터 패킷 처리
    + C++ 서버에서 MySQL을 통하여 골드, 구매의상 데이터를 실시간으로 통신

 4. 그래픽
    + 3D Max를 이용해 3D 캐릭터, 건축물, 차량 등 오브젝트 자체 제작
    + 3D Max를 이용해 캐릭터 및 좀비의 여러 애니메이션 자체 제작
    + 포토샵을 이용한 UI 자체 제작

----------
TheLastOne 웹사이트
----------
+ [http://game.thisisserver.com](http://game.thisisserver.com/)
+ ID : test , PW : 1234
+ ID : qwe, PW : 1234

---------
인게임 화면
---------
![Image05](https://github.com/WindowsHyun/TheLastOne/raw/master/Document/InGameImage/inGame.png)
![Image06](https://github.com/WindowsHyun/TheLastOne/raw/master/Document/InGameImage/ingame1.png)
![Image07](https://github.com/WindowsHyun/TheLastOne/raw/master/Document/InGameImage/ingame2.png)
![Image08](https://github.com/WindowsHyun/TheLastOne/raw/master/Document/InGameImage/ingame3.png)
![Image09](https://github.com/WindowsHyun/TheLastOne/raw/master/Document/InGameImage/ingame4.png)
![Image10](https://github.com/WindowsHyun/TheLastOne/raw/master/Document/InGameImage/ingame5.png)
![Image11](https://github.com/WindowsHyun/TheLastOne/raw/master/Document/InGameImage/inventory.png)
![Image12](https://github.com/WindowsHyun/TheLastOne/raw/master/Document/InGameImage/Car.png)
![Image13](https://github.com/WindowsHyun/TheLastOne/raw/master/Document/InGameImage/map.png)

