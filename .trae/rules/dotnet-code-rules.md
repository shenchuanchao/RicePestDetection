# 代码开发规范

你正在辅助开发一个.NET毕业设计项目。所有代码必须遵循以下规范。

## 一、项目结构规范

- 〖必须〗遵循标准.NET项目结构：
  - src/ 或项目根目录下放置 .sln 和项目文件
  - Controllers/（MVC）或 Endpoints/（Minimal API）
  - Models/（实体模型）
  - Services/（业务逻辑）
  - Data/（DbContext、迁移）
  - Views/（Razor视图）
  - wwwroot/（静态资源）
- 〖必须〗使用依赖注入，所有服务在 Program.cs 中注册。
- 〖推荐〗使用 Repository 模式分离数据访问[reference:9]。

## 二、C# 代码规范

- 〖必须〗使用 C# 10+ 特性（global using、文件范围命名空间、
  记录类型等），但需确认项目目标框架支持。
- 〖必须〗异步方法以 Async 结尾，返回 Task 或 Task<T>。
- 〖必须〗所有数据库操作使用异步方法（ToListAsync、
  FirstOrDefaultAsync 等）。
- 〖推荐〗使用 AutoMapper 处理 DTO 与实体之间的映射。
- 〖必须〗不引入未在 .csproj 中声明的 NuGet 包。

## 三、EF Core 规范

- 〖必须〗使用 Code First 迁移（dotnet ef migrations add），
  不手动修改数据库 Schema。
- 〖必须〗实体配置使用 Fluent API 或数据注解，不混用。
- 〖推荐〗查询使用 AsNoTracking() 优化只读场景。
- 〖必须〗所有连接字符串放在 appsettings.json 中，
  不硬编码。

## 四、异常处理与日志

- 〖必须〗使用全局异常处理中间件，返回统一错误格式。
- 〖推荐〗使用 Serilog 或内置 ILogger 记录关键操作日志。
- 〖必须〗不吞异常（空 catch 块），必须记录或重新抛出。

## 五、测试规范

- 〖推荐〗使用 xUnit + Moq + FluentAssertions 编写单元测试[reference:10]。
- 〖必须〗核心业务逻辑（Service层）必须有单元测试。
- 〖推荐〗使用 WebApplicationFactory 编写集成测试。