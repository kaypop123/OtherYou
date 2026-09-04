#  Other You

> **2D 사이드스크롤 액션 RPG**  
> 캐릭터 전환과 보스 AI 중심의 몰입감 있는 전투 시스템 구현 프로젝트  
> 2024 PlayX4 게임쇼 전시작

---

##  프로젝트 개요
- **개발 목표:** FSM 기반의 AI 시스템과 캐릭터 전환 플레이를 결합한 2D 액션 RPG  
- **주요 특징:**
  - 캐릭터 전환 시스템 (Adam ↔ Deva)
  - HP/MP/경험치 기반 스탯 성장 구조
  - FSM 기반 보스 AI (패턴, 인식, 스킬 전이)
  - 코루틴을 이용한 공격, 쿨타임, 스킬 시스템
  - DOTween 기반 UI 애니메이션 / Panel 전환
  - ShaderGraph + Effekseer 기반 VFX 연출
- **사용 기술:** `Unity`, `C#`, `URP`, `ShaderGraph`, `Effekseer`, `DOTween`, `Git/GitHub`

---

##  주요 구현 내용
- **AI 설계:** FSM(Finite State Machine) 기반 보스 AI 설계 및 구현  
- **플레이어 시스템:** 캐릭터 전환, HP/MP/스탯, 경험치 및 레벨업 시스템  
- **GUI 시스템:** 스탯창, 스킬쿨타임, 옵션 메뉴, 해상도 UI 등 완성도 높은 인게임 UI 설계  
- **VFX:** ShaderGraph + Effekseer 조합으로 공격/피격/보스 연출 제작  
---

## 🔎 주요 구현 코드

Other You 개발 이후 기존 보스 AI 구조를 다시 분석하여  
상태와 행동 로직이 하나의 클래스에 집중되어 있던 구조를  
**State Pattern 기반 FSM 구조로 리팩토링했습니다.**

### 🧠 Boss AI Core

📄 [AngryGodAiCoreRE.cs](https://github.com/kaypop123/OtherYou/blob/main/Assets/Scripts/Boss/BossCodeRefactoring/AngryGodAiCoreRE.cs)

- 보스 AI의 핵심 상태 및 전투 흐름 관리
- State 기반으로 보스 행동 전환
- 개별 행동 로직을 각 State로 분리하여 AI Core의 책임 축소

---

### 💤 Idle State

📄 [BossIdleState.cs](https://github.com/kaypop123/OtherYou/blob/main/Assets/Scripts/Boss/BossCodeRefactoring/BossIdleState.cs)

- 보스 대기 상태 관리
- 플레이어 및 전투 상황에 따라 다음 행동으로 전환

---

### 🏃 Chase State

📄 [BossChaseState.cs](https://github.com/kaypop123/OtherYou/blob/main/Assets/Scripts/Boss/BossCodeRefactoring/BossChaseState.cs)

- 플레이어 추적 행동 관리
- 전투 상황에 따라 공격 State로 전환

---

### ⚔️ Attack State

📄 [BossAttackState.cs](https://github.com/kaypop123/OtherYou/blob/main/Assets/Scripts/Boss/BossCodeRefactoring/BossAttackState.cs)

- 보스 기본 공격 행동 관리
- 공격 행동과 상태 전이 로직을 독립적인 State로 분리

---

### 💨 Back Dash State

📄 [BossBackDashState.cs](https://github.com/kaypop123/OtherYou/blob/main/Assets/Scripts/Boss/BossCodeRefactoring/BossBackDashState.cs)

- 플레이어와의 거리를 확보하기 위한 회피 행동
- 회피 이후 다음 공격 및 스킬 State와 연계

---

### 🔥 Active Skill State

📄 [BossActiveSkill1State.cs](https://github.com/kaypop123/OtherYou/blob/main/Assets/Scripts/Boss/BossCodeRefactoring/BossActiveSkill1State.cs)

- 보스 액티브 스킬 행동 관리
- 스킬 실행과 다른 행동 상태를 분리하여 관리

---

### 🔥 Flame State

📄 [BossFlameState.cs](https://github.com/kaypop123/OtherYou/blob/main/Assets/Scripts/Boss/BossCodeRefactoring/BossFlameState.cs)

- 보스 특수 공격 패턴 관리
- 특수 패턴을 독립적인 State로 분리

---

### 👹 Awakening State

📄 [BossAwakeningState.cs](https://github.com/kaypop123/OtherYou/blob/main/Assets/Scripts/Boss/BossCodeRefactoring/BossAwakeningState.cs)

- 보스 체력 조건에 따른 각성 상태 관리
- 페이즈 전환 및 이후 전투 패턴과 연계

---

## 🔄 Boss AI 리팩토링

### Before

기존 보스 AI는 하나의 AI Core에서 상태 판단, 행동 실행,
다수의 Flag 및 Coroutine을 함께 관리했습니다.

행동 패턴이 증가하면서 상태 조건과 우선순위가 복잡해졌고,
새로운 행동을 추가하거나 기존 행동을 수정할 때
AI Core의 여러 조건을 함께 확인해야 하는 문제가 발생했습니다.

### After

각 보스 행동을 독립적인 State 클래스로 분리하고,
State Pattern 기반 FSM 구조로 변경했습니다.

`Idle`, `Chase`, `Attack`, `BackDash`, `ActiveSkill`,
`Awakening` 등의 행동을 각각 독립적인 State로 관리하여
각 클래스가 자신의 행동과 상태 전이를 담당하도록 구성했습니다.

### 📊 구조 비교

| 항목 | Before | After |
|---|---|---|
| 상태 관리 | Flag / 조건문 중심 | State 객체 중심 |
| 행동 로직 | AI Core에 집중 | State별 분리 |
| 상태 전이 | 여러 조건에서 처리 | State 기반 명시적 전환 |
| 기능 추가 | 기존 AI Core 수정 필요 | 새로운 State 추가 중심 |
| 유지보수 | 행동 증가 시 복잡도 증가 | 상태별 독립적인 수정 가능 |

---

##  성과
- **전시:** 2024 PlayX4 게임쇼 전시 및 현장 피드백 수집  
- **보도:** 경향게임스 외 전문 매체 보도  
  👉 [기사 보기](https://www.khgames.co.kr/news/articleView.html?idxno=240192)
- **팀 역할:** 팀 리더 및 시스템 담당 (AI, 데이터, 플레이어 구조 전담)

---

##  기여도
- FSM 기반 보스 AI 설계 및 구현  
- 캐릭터 전환/스탯/저장 시스템 구조 개발  
- ShaderGraph 기반 VFX 및 UI 애니메이션 (DOTween)  
- Git 협업 환경 세팅 및 브랜치 관리  

---

## 📸 전시 사진 / 영상
<table>
  <tr>
<td align="center" width="50%"><b>PlayX4 현장 사진</b></td>
    <td align="center" width="50%"><b>보도 자료 이미지</b></td>
  </tr>
  <tr>
    <td><img src="https://github.com/user-attachments/assets/4da24aec-666e-42c1-b66d-294707129304" width="100%"></td>
    <td><img src="https://github.com/user-attachments/assets/d7b43009-4167-4fdb-9d15-47324591bd25" width="100%"></td>
  </tr>
</table>

OtherYou 영상
https://drive.google.com/file/d/1Un-N45bDUc6DDo8PEFB9ufL-ptPPMmy7/view?usp=sharing

---
## 🎮 게임 장면

<table width="100%">
  <tr>
    <td width="50%"><img src="https://github.com/user-attachments/assets/eb2f4657-b8f2-4190-a6ba-60ef10fc57c0" width="100%"></td>
    <td width="50%"><img src="https://github.com/user-attachments/assets/4d25ac86-0718-41f5-9890-93fee3da72f9" width="100%"></td>
  </tr>
  <tr>
    <td><img src="https://github.com/user-attachments/assets/08c71a24-46f6-4956-aa8a-2e089859bee2" width="100%"></td>
    <td><img src="https://github.com/user-attachments/assets/19c7f4ba-1db9-41ba-a950-17ef1039bc9f" width="100%"></td>
  </tr>
  <tr>
    <td><img src="https://github.com/user-attachments/assets/e1c373c5-8901-4f61-8b77-2198a5285897" width="100%"></td>
    <td><img src="https://github.com/user-attachments/assets/39e99891-bfae-4c24-9a8c-7375a57fd4c4" width="100%"></td>
  </tr>
  <tr>
    <td><img src="https://github.com/user-attachments/assets/d9e44c60-e84e-4950-a00b-08cdca3b9ad6" width="100%"></td>
    <td><img src="https://github.com/user-attachments/assets/8ad29f66-3069-499f-ac8f-461bed51fe6e" width="100%"></td>
  </tr>
  <tr>
    <td><img src="https://github.com/user-attachments/assets/033d7b31-571e-4061-b82d-adeb0a3bbc7e" width="100%"></td>
  </tr>
</table>



