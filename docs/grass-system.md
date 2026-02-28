# Grass System Documentation

## Overview
잔디 생성, 자르기, 자라기를 관리하는 시스템입니다.

## File Structure

### GrassData.cs
잔디 하나의 데이터를 저장하는 클래스입니다.
- `position`: 세계 좌표
- `baseAngle`: 초기 기울기 각도
- `currentAngle`: 현재 기울기 각도
- `targetAngle`: 목표 각도
- `heightScale`, `widthScale`: 크기
- `randomRotY`, `randomScale`, `randomHeight`: 렌더링용 랜덤 값
- `isCut`: 깎였는지 여부
- `mowerAngle`: mower 방향으로 눕는 각도
- `cutTime`: 깎인 시간

### GrassManager.cs
GrassManager 인스턴스가 하나만 존재하며, 모든 잔디 데이터를 관리합니다.

**설정 변수:**
- `grassMaterial`: 렌더링용 머티리얼
- `maxGrassCount`: 최대 잔디 개수
- `cutHeightScale`: 깎였을 때 높이 비율 (0.3 = 30%)

### GrassCutter.cs
Mower가 지나가면 잔디를 자르고 자라는 로직을 처리합니다.

**설정 변수:**
- `mower`: 자르는 오브젝트 (Transform)
- `mowerRadius`: 자르는 반경
- `bendAngle`: 눕는 각도
- `standUpSpeed`: 눕는 속도
- `growSpeed`: 자라는 속도
- `growDelay`: 자라기까지 대기 시간(초)

### GrassSpawner.cs
영역 안에 한 번에 잔디를 생성합니다.

**설정 변수:**
- `mower`: GrassCutter에 전달할 mower
- `grassDensity`: 잔디 밀도
- `bendAngle`: 초기 기울기
- `heightScale`, `widthScale`: 크기
- `areaPoint1`, `areaPoint2`: 생성 영역 좌표

## Flow

```
GrassSpawner.Start()
  → GrassCutter 찾고 mower 연결
  → areaPoint1~areaPoint2 영역에 잔디 생성

GrassCutter.Update() (매 프레임)
  → mower 주변 mowerRadius 내 잔디 찾기
  → isCut=true, cutTime 저장
  → growDelay 후 growSpeed로 점차 복구

GrassManager.LateUpdate() (매 프레임)
  → 모든 잔디 렌더링
  → isCut=true면: 크기 줄이고 mower 방향으로 회전
  → isCut=false면: 원래 크기와 랜덤 방향
```

## Setup

1. GrassManager 컴포넌트를 가진 오브젝트 추가
2. 빈 오브젝트에 GrassCutter 컴포넌트 추가
3. GrassCutter에 mower 할당
4. GrassSpawner에 areaPoint1, areaPoint2 할당
5. GrassManager의 grassMaterial에 GreenMat 할당
6. grassMaterial의 "Enable GPU Instancing" 체크

## Parameters

| 변수 | 설명 | 기본값 |
|------|------|--------|
| mowerRadius | 자르는 반경 | 1f |
| cutHeightScale | 깎였을 때 높이 비율 | 0.3 |
| bendAngle | 눕는 각도 | 40f |
| standUpSpeed | 눕는 속도 | 5f |
| growSpeed | 자라는 속도 | 0.5f |
| growDelay | 자라기까지 대기 시간(초) | 3f |
| grassDensity | 잔디 밀도 | 20f |
