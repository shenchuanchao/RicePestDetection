# 水稻病虫害检测系统 - 系统设计文档

## 一、系统架构设计

### 1.1 总体架构图

系统采用经典的四层分层架构，自上而下分别为表现层、业务逻辑层、数据访问层和机器学习推理层。

```mermaid
graph TB
    subgraph Client[客户端]
        Browser[浏览器<br/>Chrome / Edge / Firefox]
    end

    subgraph Presentation[表现层 Presentation Layer]
        Controllers[Controllers<br/>MVC控制器]
        Views[Views<br/>Razor + Bootstrap 5]
        ViewModels[ViewModels<br/>视图模型]
    end

    subgraph Business[业务逻辑层 Business Layer]
        UserService[UserService<br/>用户服务]
        PestService[PestService<br/>知识库服务]
        DetectionService[DetectionService<br/>检测服务]
        StatisticsService[StatisticsService<br/>统计服务]
        LogService[LogService<br/>日志服务]
    end

    subgraph DataAccess[数据访问层 Data Access Layer]
        AppDbContext[AppDbContext]
        Repositories[Repositories<br/>仓储模式]
        Migrations[EF Core Migrations]
    end

    subgraph ML[机器学习推理层 ML Layer]
        MLModel[ML.NET Pipeline<br/>ONNX模型加载]
        PredictionEngine[PredictionEngine<br/>图像分类推理]
        ModelFile[rice-pest-model.onnx]
    end

    subgraph Infrastructure[基础设施]
        SQLite[(SQLite 数据库)]
        FileStorage[文件存储<br/>上传图片/模型文件]
        Identity[ASP.NET Core Identity<br/>认证授权]
    end

    Browser -->|HTTP请求| Controllers
    Controllers --> Views
    Controllers --> ViewModels
    Controllers -->|调用| UserService
    Controllers -->|调用| PestService
    Controllers -->|调用| DetectionService
    Controllers -->|调用| StatisticsService
    Controllers -->|调用| LogService

    UserService --> AppDbContext
    PestService --> AppDbContext
    DetectionService --> AppDbContext
    StatisticsService --> AppDbContext
    LogService --> AppDbContext

    AppDbContext --> Repositories
    Repositories --> SQLite
    AppDbContext --> Migrations

    DetectionService -->|调用推理| MLModel
    MLModel --> PredictionEngine
    PredictionEngine --> ModelFile
    DetectionService --> FileStorage

    Controllers --> Identity
```

### 1.2 各层职责说明

| 层次 | 职责 | 包含组件 |
|------|------|----------|
| 表现层 | 接收用户请求、返回视图、数据校验 | Controllers、Views、ViewModels |
| 业务逻辑层 | 核心业务逻辑、事务控制、权限校验 | 各Service类 |
| 数据访问层 | 数据库CRUD、迁移管理 | AppDbContext、Repositories |
| 机器学习层 | ONNX模型加载、图像预处理、推理 | ML.NET Pipeline、PredictionEngine |
| 基础设施 | 数据持久化、文件存储、认证 | SQLite、文件系统、Identity |

## 二、数据库设计

### 2.1 E-R 图

```mermaid
erDiagram
    User ||--o{ DetectionRecord : "创建"
    User ||--o{ Log : "操作"
    Pest ||--o{ DetectionRecord : "识别结果"

    User {
        int Id PK
        string UserName UK
        string PasswordHash
        string Email
        string Phone
        string Role
        bool IsActive
        DateTime CreatedAt
    }

    Pest {
        int Id PK
        string Name
        string Category
        string Symptoms
        string HarmDescription
        string PreventionMethod
        string ImagePath
        DateTime CreatedAt
    }

    DetectionRecord {
        int Id PK
        int UserId FK
        int PestId FK
        string ImagePath
        string ResultName
        float Confidence
        DateTime CreatedAt
    }

    Log {
        int Id PK
        int UserId FK
        string ActionType
        string Content
        DateTime CreatedAt
    }
```

### 2.2 数据表详细设计

#### 表1：Users（用户表）

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| Id | int | PK, Identity | 主键 |
| UserName | nvarchar(50) | NOT NULL, UNIQUE | 用户名 |
| PasswordHash | nvarchar(200) | NOT NULL | 密码哈希 |
| Email | nvarchar(100) | NULL | 邮箱 |
| Phone | nvarchar(20) | NULL | 手机号 |
| Role | nvarchar(20) | NOT NULL, DEFAULT 'User' | 角色（Admin/User） |
| IsActive | bit | NOT NULL, DEFAULT 1 | 是否启用 |
| CreatedAt | datetime | NOT NULL | 创建时间 |

#### 表2：Pests（病虫害表）

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| Id | int | PK, Identity | 主键 |
| Name | nvarchar(50) | NOT NULL | 病虫害名称 |
| Category | nvarchar(20) | NOT NULL | 类别（Disease病害/Pest虫害） |
| Symptoms | nvarchar(1000) | NULL | 症状描述 |
| HarmDescription | nvarchar(1000) | NULL | 危害描述 |
| PreventionMethod | nvarchar(1000) | NULL | 防治方法 |
| ImagePath | nvarchar(200) | NULL | 示例图片路径 |
| CreatedAt | datetime | NOT NULL | 创建时间 |

#### 表3：DetectionRecords（检测记录表）

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| Id | int | PK, Identity | 主键 |
| UserId | int | FK → Users.Id | 用户ID |
| PestId | int | FK → Pests.Id, NULL | 识别出的病虫害ID |
| ImagePath | nvarchar(200) | NOT NULL | 上传图片路径 |
| ResultName | nvarchar(50) | NOT NULL | 识别结果名称 |
| Confidence | float | NOT NULL | 置信度（0-1） |
| CreatedAt | datetime | NOT NULL | 检测时间 |

#### 表4：Logs（系统日志表）

| 字段名 | 类型 | 约束 | 说明 |
|--------|------|------|------|
| Id | int | PK, Identity | 主键 |
| UserId | int | FK → Users.Id, NULL | 操作人ID |
| ActionType | nvarchar(50) | NOT NULL | 操作类型 |
| Content | nvarchar(500) | NULL | 操作内容 |
| CreatedAt | datetime | NOT NULL | 操作时间 |

## 三、核心业务流程设计

### 3.1 用户登录流程

```mermaid
graph TD
    Start([用户访问登录页]) --> Input[输入用户名和密码]
    Input --> Submit[提交登录表单]
    Submit --> Validate{模型校验是否通过?}
    Validate -->|否| ShowError[显示校验错误]
    ShowError --> Input
    Validate -->|是| QueryDB[查询用户数据]
    QueryDB --> ExistCheck{用户是否存在?}
    ExistCheck -->|否| FailMsg[提示用户名或密码错误]
    FailMsg --> Input
    ExistCheck -->|是| ActiveCheck{账号是否启用?}
    ActiveCheck -->|否| DisabledMsg[提示账号已被禁用]
    DisabledMsg --> Input
    ActiveCheck -->|是| VerifyPwd{密码哈希是否匹配?}
    VerifyPwd -->|否| FailMsg
    VerifyPwd -->|是| CreateClaims[创建身份认证票据]
    CreateClaims --> SignIn[SignInAsync 登录]
    SignIn --> LogRecord[记录登录日志]
    LogRecord --> Redirect{角色判断}
    Redirect -->|管理员| AdminHome[跳转管理首页]
    Redirect -->|普通用户| UserHome[跳转用户首页]
```

### 3.2 图像检测业务流程

```mermaid
graph TD
    Start([用户进入检测页面]) --> Upload[选择并上传水稻图片]
    Upload --> FileCheck{文件格式/大小校验}
    FileCheck -->|不合法| FileError[提示文件格式错误或过大]
    FileError --> Upload
    FileCheck -->|合法| SaveFile[保存图片到服务器]
    SaveFile --> LoadModel[加载ONNX检测模型]
    LoadModel --> Preprocess[图像预处理<br/>Resize/Normalize]
    Preprocess --> Inference[调用PredictionEngine推理]
    Inference --> GetResult[获取分类结果与置信度]
    GetResult --> QueryPest[根据结果匹配病虫害知识库]
    QueryPest --> SaveRecord[保存检测记录到数据库]
    SaveRecord --> ShowResult[展示检测结果<br/>病虫害名称/置信度/防治建议]
    ShowResult --> End([结束])
```

### 3.3 病虫害知识维护流程（管理员）

```mermaid
graph TD
    Start([管理员进入知识库管理]) --> Action{操作类型}
    Action -->|新增| AddForm[填写病虫害信息]
    Action -->|编辑| EditForm[修改病虫害信息]
    Action -->|删除| ConfirmDelete{确认删除?}

    AddForm --> AddValidate{信息校验}
    AddValidate -->|不通过| AddForm
    AddValidate -->|通过| AddSave[保存到数据库]
    AddSave --> AddLog[记录操作日志]

    EditForm --> EditValidate{信息校验}
    EditValidate -->|不通过| EditForm
    EditValidate -->|通过| EditSave[更新数据库]
    EditSave --> EditLog[记录操作日志]

    ConfirmDelete -->|否| Start
    ConfirmDelete -->|是| DeleteDB[从数据库删除]
    DeleteDB --> DeleteLog[记录操作日志]

    AddLog --> Refresh([刷新列表])
    EditLog --> Refresh
    DeleteLog --> Refresh
```

### 3.4 数据统计流程

```mermaid
graph TD
    Start([用户访问统计页面]) --> QueryRange[选择统计时间范围]
    QueryRange --> QueryDB1[查询检测记录数据]
    QueryDB1 --> GroupByDate[按日期分组统计检测次数]
    GroupByDate --> LineChart[生成检测趋势折线图]

    QueryRange --> QueryDB2[查询病虫害分布数据]
    QueryDB2 --> GroupByPest[按病虫害类型分组统计]
    GroupByPest --> PieChart[生成病虫害分布饼图]

    QueryRange --> QueryDB3[查询用户数据]
    QueryDB3 --> UserStats[统计用户总数与活跃数]
    UserStats --> ShowStats[展示统计数字]

    LineChart --> RenderPage[渲染统计页面]
    PieChart --> RenderPage
    ShowStats --> RenderPage
    RenderPage --> End([结束])
```

## 四、目录结构设计

```
RicePestDetection/
├── RicePestDetection.sln
├── src/
│   └── RicePestDetection.Web/
│       ├── Controllers/
│       │   ├── AccountController.cs        # 登录注册
│       │   ├── HomeController.cs           # 首页
│       │   ├── PestController.cs           # 知识库
│       │   ├── DetectionController.cs      # 图像检测
│       │   ├── RecordController.cs         # 检测记录
│       │   ├── StatisticsController.cs     # 数据统计
│       │   ├── AdminController.cs          # 系统管理
│       │   └── LogController.cs            # 系统日志
│       ├── Views/
│       │   ├── Account/
│       │   ├── Home/
│       │   ├── Pest/
│       │   ├── Detection/
│       │   ├── Record/
│       │   ├── Statistics/
│       │   ├── Admin/
│       │   ├── Shared/
│       │   └── _ViewImports.cshtml
│       ├── Models/
│       │   ├── User.cs
│       │   ├── Pest.cs
│       │   ├── DetectionRecord.cs
│       │   └── Log.cs
│       ├── ViewModels/
│       │   ├── LoginViewModel.cs
│       │   ├── RegisterViewModel.cs
│       │   ├── PestViewModel.cs
│       │   ├── DetectionResultViewModel.cs
│       │   └── StatisticsViewModel.cs
│       ├── Services/
│       │   ├── IUserService.cs
│       │   ├── UserService.cs
│       │   ├── IPestService.cs
│       │   ├── PestService.cs
│       │   ├── IDetectionService.cs
│       │   ├── DetectionService.cs
│       │   ├── IStatisticsService.cs
│       │   ├── StatisticsService.cs
│       │   ├── ILogService.cs
│       │   ├── LogService.cs
│       │   └── MLPredictionService.cs      # ML.NET推理服务
│       ├── Data/
│       │   ├── AppDbContext.cs
│       │   ├── Migrations/
│       │   └── SeedData.cs
│       ├── wwwroot/
│       │   ├── css/
│       │   ├── js/
│       │   ├── images/
│       │   └── uploads/                      # 用户上传图片
│       ├── Program.cs
│       └── appsettings.json
├── models/
│   └── rice-pest-model.onnx                 # ONNX检测模型
├── tests/
│   └── RicePestDetection.Tests/
│       ├── Services/
│       └── RicePestDetection.Tests.csproj
├── experiments/
├── docs/
│   ├── requirements-analysis.md
│   ├── system-design.md
│   └── paper/
├── Dockerfile
└── README.md
```

## 五、接口设计要点

### 5.1 控制器路由设计

| 控制器 | 路由前缀 | 主要Action |
|--------|----------|------------|
| AccountController | /Account | Login, Logout, Register |
| HomeController | / | Index |
| PestController | /Pest | Index, Details, Create, Edit, Delete |
| DetectionController | /Detection | Index, Upload, Result |
| RecordController | /Record | Index, Details, Delete |
| StatisticsController | /Statistics | Index |
| AdminController | /Admin | Users, EditUser, ToggleActive |
| LogController | /Log | Index |

### 5.2 ML推理服务接口

```csharp
public interface IMLPredictionService
{
    // 加载ONNX模型（启动时执行一次）
    void LoadModel(string modelPath);

    // 对图片进行推理，返回预测结果
    Task<DetectionPrediction> PredictAsync(string imagePath);
}

public class DetectionPrediction
{
    public string Label { get; set; }        // 病虫害名称
    public float Confidence { get; set; }    // 置信度
    public Dictionary<string, float> Scores { get; set; }  // 各类别得分
}
```

---

> **说明**：以上系统设计文档为 AI 辅助生成草稿，需经人工审阅和修改后作为正式设计文档。
