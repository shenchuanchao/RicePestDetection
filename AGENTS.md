# 项目概述

这是一个计算机专业毕业设计项目。

## 技术栈（根据当前项目调整）

- 后端：ASP.NET Core 8、C# 12
- 架构：MVC / Web API（根据项目选择）
- ORM：Entity Framework Core（Code First）
- 数据库：SQL Server / SQLite
- 前端：Razor + Bootstrap 5 / Blazor Server
- 图表：ECharts
- 部署：IIS / Docker

## 毕业设计项目通用铁律

1. **代码可运行**：所有项目必须能在本地通过 dotnet run 启动，
   数据库迁移脚本完整。
2. **实验有记录**：每次性能测试或功能测试在 experiments/ 下创建
   记录，包含环境、参数、结果。
3. **论文与代码同步**：代码变更后，同步更新论文中的系统设计章节。
4. **敏感文件不入库**：appsettings.Development.json、.env、
   数据库备份文件不提交到 Git。
5. **不生成任何 Git 命令**：由用户手动执行版本控制操作。

## 工作流程触发

当用户要求“开发毕业设计项目”或“搭建毕设系统”时，
自动加载 .trae/skills/dotnet-graduation-project/SKILL.md。