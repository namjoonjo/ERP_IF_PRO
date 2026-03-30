# ERP_IF_PRO - INTEROJO ERP Interface System

INTEROJO 사내 ERP 인터페이스 시스템.
각 공정/영역별 업무 화면을 DLL 기반으로 동적 로딩하여 운영하는 Windows Forms 애플리케이션입니다.

## 기술 스택

- **Framework**: .NET Framework 4.8 (Windows Forms)
- **UI**: DevExpress v23.2
- **DB**: MSSQL Server (Stored Procedure 기반)
- **배포**: IIS ClickOnce (포트 30083)
- **DLL 배포**: IIS FTP 서버를 통한 동적 DLL 다운로드

## 프로젝트 구조

```
ERP_IF_PRO/
├── ERP_IF_PRO/                    # 메인 프로젝트 (실행파일)
│   ├── Main.cs                    # 메인 화면 (메뉴, 탭, DLL 로딩)
│   ├── LoginForm.cs               # 로그인 화면 (일반/Admin 선택)
│   ├── Program.cs                 # 진입점
│   └── Modules/
│       ├── MSSQL.cs               # DB 접속 모듈
│       ├── CommonModule.cs        # 공통 유틸리티
│       ├── FTPModule.cs           # FTP 다운로드 모듈
│       └── CustomMessageBox.cs    # 커스텀 메시지 박스
│
└── Projects/                      # DLL 프로젝트 (각 화면)
    ├── Admin/                     # 관리자 메뉴
    │   ├── MENUSET/               # 메뉴 관리
    │   ├── ACCESSLOG/             # 로그인/메뉴 접속정보
    │   └── PATCH_NOTE/            # 패치노트
    │
    ├── area_C/                    # C관 (제품창고)
    │   ├── DANPLA_COLLECTOR_C/    # 단프라 스캔(C관)
    │   ├── Excel_QR/              # Excel Packing List
    │   ├── EXCEL_QR_MAPPER/       # Excel QR 매퍼
    │   └── UDI_COLLECTOR_EMAX/    # UDI 스캔
    │
    ├── area_Combi/                # 원료배합
    │   ├── Regi_Combi/            # 배합실적등록
    │   ├── Regi_Combi_Half_Lot_Manage/  # 반제품 LOT 관리
    │   ├── Regi_Combi_PR_Label/   # 배합 라벨 출력
    │   ├── PRODUCTION_ANALYSIS/   # 생산실적조회 및 라벨출력
    │   └── COMBI_USER_MANAGE/     # 공정별 사용자 관리
    │
    └── area_L/                    # L관 (반품창고)
        ├── DANPLA_COLLECTOR/      # 단프라 스캔(L관)
        ├── DANPLA_COLLECTOR_MATE/ # 단프라 스캔(자재)
        ├── EXCEL_FOR_MATE/        # 자재 엑셀 출력
        ├── RE_INS/                # 반품검수프로그램
        ├── RE_INVEST/             # 반품2창고전수조사
        ├── REG_IN_ITEM/           # 입고등록무선바코드
        └── UDI_RE_STORE/          # UDI 업데이트 프로그램
```

## 주요 기능

### 메인 시스템 (ERP_IF_PRO)
- **동적 DLL 로딩**: FTP 서버에서 DLL을 다운로드하여 런타임에 로드
- **메뉴 시스템**: DB 기반 메뉴 구성 (BarManager + AccordionControl + XtraTabControl)
- **메뉴 검색**: 자동완성 기능이 있는 메뉴 검색 팝업
- **권한 관리**: 일반 사용자 / Admin 로그인 분리 (ADMIN_YN 기반)
- **폼별 비밀번호 보호**: PASSWORD_YN/PASSWORD 컬럼 기반
- **패널 제어**: PANEL_YN 컬럼으로 폼별 상단 패널 표시/숨김
- **도킹 제어**: DOCK_OR_NOT 컬럼으로 폼 꽉참/원본크기 선택
- **접속 로그**: 로그인 로그(TB_ERP_IF_USER_LOGIN_LOG), 메뉴 접속 로그(TB_ERP_IF_USER_MENU_LOG)

### DLL 프로젝트 공통
- `UpdateStatus` (Action\<string\>) 프로퍼티를 통해 메인 화면 상태바에 메시지 표시
- 네임스페이스: `RAZER_C` (배합 계열), `ERP_IF_PRO` (기타)

## DB 테이블

| 테이블 | 용도 |
|--------|------|
| `TB_MENU_MST` | 메뉴 마스터 (메뉴 구성, 권한, 패널, 도킹 설정) |
| `TB_ERP_IF_USER_LOGIN_LOG` | 로그인 접속 로그 |
| `TB_ERP_IF_USER_MENU_LOG` | 메뉴 접속 로그 |
| `COMBOTBL` | 콤보박스 데이터 (공정별 사용자 등) |

## 환경 설정

### 필수 요구사항
- Windows 10/11
- .NET Framework 4.8
- DevExpress v23.2 (라이선스 필요)
- MSSQL Server 접속 가능

### App.config 설정
```xml
<connectionStrings>
    <add name="ERP_2" connectionString="SERVER=서버IP;DATABASE=ERP_2;UID=계정;PWD=비밀번호" />
</connectionStrings>
<appSettings>
    <add key="FTPHost" value="FTP서버IP" />
    <add key="FTPPort" value="50021" />
    <add key="FTPUser" value="계정" />
    <add key="FTPPassword" value="비밀번호" />
    <add key="FTPRemotePath" value="/dlls/" />
    <add key="DLLLocalPath" value="C:\ERP_DLL_Cache\" />
    <add key="logfilePath" value="C:\ERP_Logs\" />
</appSettings>
```

## 배포

### ClickOnce 배포 (IIS)
- IIS 사이트: `ERP_IF_ClickOnce` (포트 30083)
- 게시 경로: `\\서버IP\ERP_IF_PRO\` (네트워크 공유)
- 클라이언트 설치: `http://서버IP:30083/publish.htm`

### DLL 배포 (FTP)
- 각 프로젝트를 Release 빌드 후 생성된 DLL을 FTP 서버의 `/dlls/` 경로에 업로드
- 메인 프로그램이 실행 시 FTP에서 최신 DLL을 다운로드하여 로컬 캐시에 저장

## 브랜드

- **Primary Blue**: #1B5091
- **Orange Accent**: #F58220
