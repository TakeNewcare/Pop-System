# POP System
   [![Hits](https://hits.seeyoufarm.com/api/count/incr/badge.svg?url=https%3A%2F%2Fgithub.com%2FTakeNewcare&count_bg=%23939DAE&title_bg=%2361ACCD&icon=&icon_color=%23E7E7E7&title=hits&edge_flat=false)](https://hits.seeyoufarm.com)
   
<br>

<p align="center">
  <img src="https://github.com/user-attachments/assets/32f4564c-c945-4606-a887-207be60e5711" width="500px">
</p>


## 프로젝트 소개
이번 프로젝트는 POP 시스템입니다. <br>해당 시스템을 개발하기 전, 통신에 대한 공부를 하고 있었으며 기숙사의 습도 문제로 온습도 측정 장치에 관심이 생겨 작게 구현해보자는 생각에 시작하게 되었습니다.<br><br>
마침 아래의 참고이미지와 같이 POP 시스템에 온습도 그래프를 내장한 디자인을 찾아 비슷하게 구현해 보았습니다.
또한, 첫 구상은 생산량을 버튼을 누르면 생산량이 올라가는 1차원적인 형식으로 개발하려 했으나 추가적으로 알게된 지식들을 이용해 보고 싶었으며, <br>
교내 프로젝트 발표회라는게 있다고 하여 간단한 코드로 컨베이어를 구현하였고 초음파 센서를 활용하여 생산량을 측정할 수 있게 구현하였습니다.<br>

<br>
참고 이미지<br>
<img src="https://github.com/user-attachments/assets/10aee911-5127-4245-abae-a9ff09cca94d" width="200px">
<br>
<br>
Reason for making : C#(winform) and Aduino <br>
Busan Polytechnic High-Tech Course <br>
<br>

## 결과물
https://www.youtube.com/watch?v=3LlXWkyenTc
<br>

## 개발팀 소개
<table>
  <tr>
    <th>정진영</th>
    <td  rowspan="3">
    안녕하십니까!,<br> 물류팀에서 일을 하다 IT 부서 분들과 친해져 해당 분야를 알게 되었고,
    이번 high-tech 과정을 통해 새로운 길을 향해 성장하고 있습니다.
   <br>
   <br>
    처음 접하는 분야라 두려움이 있었지만,<br> 
    오류가 났을 때 스스로 해결해야 직성이 풀리는 저의 성격과 잘 맞아 꾸준하게 성장하고 있습니다. <br> 
   감사합니다.
    </td>
  </tr>
  <tr>
    <td><img src="https://github.com/user-attachments/assets/ea956d90-eef8-44be-ae66-054517fdd8da" width="200px"></td>
  </tr>
  <tr>
    <td align='center'>wlsdud1525@naver.com</td>
  </tr>
</table>
<br>
<hr> 

## Stacks
### Environment
<img src="https://img.shields.io/badge/visualstudio-5C2D91?style=flat-square&logo=visualstudio&logoColor=white"/> <img src="https://img.shields.io/badge/github-181717?style=flat-square&logo=github&logoColor=white"/>

### Development
<img src="https://img.shields.io/badge/.NET-512BD4?style=flat-square&logo=.NET&logoColor=white"/> <img src="https://img.shields.io/badge/-WinForm-FF0000?logo=Csharp&style=flat&logo=csharp&logoColor=white"/> 
![C](https://img.shields.io/badge/c-%2300599C.svg?style=for-the-badge&logo=c&logoColor=white)
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![Aduino](https://img.shields.io/badge/Aduino-60B5CC?style=for-the-badge&logo=Aduino&logoColor=60B5CC)

### 화면 구성
|화면|설명|
|:---:|:---|
|<img src="https://github.com/user-attachments/assets/155e800f-db1e-4e8f-92ab-afe825d35838" width="400px">|<br> 생산라인 1개를 실제 현장처럼 구현하여 데이터를 수집하는 장소를 만들었습니다.<br>2개의 아두이노를 사용하였으며 한 대의 아두이노에 DHT22와 LCD를 연결하여<br>온습도를 측정함과 동시에 LCD 화면으로 출력 및 10초 마다 시리얼 통신을 해 주었습니다.<br><br>다른 한 대의 아두이노는 HC-SR04를 연결하여 생산라인의 컨베이어를 통해 초음파 센서를 지나가면 송수신 거리를 측정하여 8cm 미만의 거리로 측정될 시에만 생산량을 늘려주었습니다. 전송된 데이터는 아래의 화면 시스템에서 수신됩니다.<br><br>|
|<img src = "https://github.com/user-attachments/assets/cd4d3ea9-abf0-4b8b-af1b-c688f21ef8f9" width="500px"></img>|<br> 해당 화면은 데이터를 수집하여 DB(MSSQL)로 전송하는 역할을 합니다. 각 라인별 생산 버튼을 통해 직접적으로 생산량을 올려 줄 수 있는 기능을 포함하고 있습니다.<br><br> ※ 해당 화면을 만든 이유 : 초음파 센서를 활용하기 전 POP 시스템이 주 컨텐츠여서 단순하게 버튼으로 생산량을 올릴 계획이었지만 프로젝트를 진행하고 알아가면서 좀 더 붙여보고 싶어져 만들어진 화면입니다.<br><br>|
|<img src = "https://github.com/user-attachments/assets/32f4564c-c945-4606-a887-207be60e5711" width="800px"></img>|<br> 이번 프로젝트의 주 컨텐츠인 POP SYSTEM 화면입니다.<br>저장된 DB의 데이터를 이용하여 각 라인별 생산목표, 생산량, 작동 시간, 온습도 등의 자료를 활용하여 라인별 달성률, 작동률, 작동 상태 그리고 전체 생산량 기준 라인별 생산비율 등의 그래프를 표현하였습니다.<br><br> 작동 상태는 생산량이 0인 경우 정지, 지정한 시간 동안 생산량에 변화가 없을 경우 생산 지연 그리고 정상적으로 생산되는 상태일 때는 생산 중 3가지 상태로 나누어 빨간색, 주황색, 초록생으로 나누어 표현하였습니다.<br><br>※ winform을 통한 그래프 작업이 처음이었기에 코드를 작성하는 시간보다 그래프를 설정하고 데이터를 만지는 시간이 더 길었던 것 같습니다. <br>|
|<img src = "https://github.com/user-attachments/assets/20348a29-60a3-4305-a64b-54070a3f5cd8" width="800px"></img>|<br> 전체 사진 <br>|
|<img src = "https://github.com/user-attachments/assets/f640a7ac-bd24-4db6-9ce5-cae834c2e942" width="800px"></img>|<br> 프로젝프 발표회 사진 <br>|

<br>
<br>

## 아쉬운 점 및 개선점
- Open CV 기술
현재 생산량 증가는 초음파 센서를 이용한 단편적인 측정에 그치기 때문에 OpenCV를 활용하여 생산품의 모양 및 바코드 등을 인식하여 생산량을 늘리는 시스템을 구축할 계획입니다.

<br>
