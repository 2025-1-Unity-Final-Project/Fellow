# Fellow - Unity Live2D AI Companion Game
> Unity 기반 Live2D 캐릭터와 AI 채팅 시스템을 결합한 인터랙티브 게임 <br>

## 🎮 *****Contributors*****
| 성규현 <br> [@dmp100](https://github.com/dmp100) | koyy418 <br> [@koyy418](https://github.com/koyy418) |
|:---:|:---:|
| <img width="150" src="https://github.com/user-attachments/assets/daa04602-4d2d-456e-a11b-0c97509ea0c1"/> | <img width="150" src="https://github.com/user-attachments/assets/placeholder-profile-image"/> |
<br/>

## ✨ *****Key Features*****

### 🤖 AI-Powered Character Interaction
- **OpenAI Assistant API** 기반 자연스러운 대화 시스템
- **Frieren 캐릭터** 설정으로 몰입감 있는 상호작용
- **컨텍스트 기반 응답** 생성으로 일관된 캐릭터성 유지

### 💖 Dynamic Affinity System
- **6단계 친밀도 시스템** (Level 0-5)
- **친밀도별 차별화된 대화 톤**과 캐릭터 반응
- **실시간 감정 분석**을 통한 자동 친밀도 조절

### 🎭 Live2D Integration
- **친밀도 연동 표정 시스템** - 12가지 표정 변화
- **실시간 립싱크** 및 음성 동기화
- **터치 상호작용** 및 제스처 인식
- **부드러운 모션 블렌딩**으로 자연스러운 애니메이션

### 🎮 Interactive Gameplay
- **다중 씬 구성** - 메인, 게임플레이, NPC, 히스토리
- **플레이어 선택 기반** 스토리 진행
- **실시간 시간 표시** 및 상태 관리
- **성공/실패 피드백 시스템**
<br/>

## 🟨 *****SCREENSHOT*****
| 메인화면 | AI 채팅 | Live2D 상호작용 | 게임플레이 | NPC 대화 | 친밀도 시스템 |
|:---:|:---:|:---:|:---:|:---:|:---:|
| <img width="200" src="https://github.com/user-attachments/assets/placeholder-main"/> | <img width="200" src="https://github.com/user-attachments/assets/placeholder-chat"/> | <img width="200" src="https://github.com/user-attachments/assets/placeholder-live2d"/> | <img width="200" src="https://github.com/user-attachments/assets/placeholder-gameplay"/> | <img width="200" src="https://github.com/user-attachments/assets/placeholder-npc"/> | <img width="200" src="https://github.com/user-attachments/assets/placeholder-affinity"/> |
<br/>

## 📗 *****Convention*****
[📱 Unity 개발 문서](https://docs.google.com/document/d/1hYzwONkhU0kB6pfipcdS6Ax6zguBCO7nJdch3NEX038/edit?tab=t.0)


## 🔧 *****TECH STACKS*****
| **Category** | **TechStack** |
| --- | --- |
| Game Engine | Unity 2022.3 LTS |
| Graphics | Live2D Cubism SDK |
| AI Integration | OpenAI Assistant API |
| Language | C# |
| Input System | Unity Input System |
| Rendering | Universal Render Pipeline (URP) |
| UI Framework | Unity UI Toolkit |
<br/>

## 📁 *****Foldering*****
```
📂 Assets
┣ 📂 Script
┃ ┣ 📂 AI
┃ ┃ ┣ 📄 OpenAIAssistantAPI.cs
┃ ┃ ┣ 📄 AIChatAffinitySystem.cs
┃ ┃ ┗ 📄 ChatDataManager.cs
┃ ┣ 📂 Live2D
┃ ┃ ┣ 📄 Live2DAffinityExpression.cs
┃ ┃ ┣ 📄 Live2DLipSyncManager.cs
┃ ┃ ┣ 📄 Live2DPinchZoomController.cs
┃ ┃ ┗ 📄 UnifiedLive2DLifeSystem.cs
┃ ┣ 📂 Manager
┃ ┃ ┣ 📄 GameManager.cs
┃ ┃ ┣ 📄 ChatManager1.cs
┃ ┃ ┗ 📄 KeyboardChatManager.cs
┃ ┣ 📂 Player
┃ ┃ ┣ 📄 SimplePlayerController.cs
┃ ┃ ┣ 📄 PlayerHealth.cs
┃ ┃ ┗ 📄 PlayerChoice.cs
┃ ┣ 📂 NPC
┃ ┃ ┣ 📄 NPCDialogue.cs
┃ ┃ ┣ 📄 NPCAffinityUI.cs
┃ ┃ ┗ 📄 EnemyPatrol.cs
┃ ┗ 📂 UI
┃   ┣ 📄 HeartUI.cs
┃   ┣ 📄 TimeDisplay.cs
┃   ┗ 📄 SuccessPanelController.cs
┣ 📂 Scenes
┃ ┣ 📄 MainScene1.unity
┃ ┣ 📄 GamePlayScene.unity
┃ ┣ 📄 NPCScene.unity
┃ ┗ 📄 HistoryScene.unity
┣ 📂 CubismModels
┃ ┗ 📂 Live2D Characters
┣ 📂 Live2D
┃ ┣ 📂 Cubism
┃ ┗ 📂 SDK
┣ 📂 Prefab
┃ ┣ 📂 UI
┃ ┣ 📂 Character
┃ ┗ 📂 Environment
┣ 📂 Resources
┃ ┣ 📂 Audio
┃ ┣ 📂 Data
┃ ┗ 📂 Materials
┗ 📂 Settings
  ┣ 📂 Rendering
  ┗ 📂 Input
```