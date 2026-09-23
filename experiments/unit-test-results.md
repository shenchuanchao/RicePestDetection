# 实验记录 - 水稻病虫害检测系统单元测试

## 实验基本信息

| 项目 | 内容 |
|------|------|
| 实验编号 | EXP-2026-001 |
| 实验日期 | 2026-09-22 |
| 实验类型 | 单元测试 |
| 测试框架 | xUnit 2.5.3 + Moq 4.20.72 + FluentAssertions 8.11.0 |

## 测试环境

| 项目 | 版本/配置 |
|------|-----------|
| 操作系统 | Windows 10 (10.0.26100) |
| .NET SDK | 9.0.203 |
| 目标框架 | .NET 8.0 |
| 数据库 | EF Core InMemory (8.0.16) |
| ORM | Entity Framework Core 8.0.16 |

## 测试用例与结果

### 一、PestService 测试（11个用例）

| 编号 | 测试用例 | 预期结果 | 实际结果 | 状态 |
|------|----------|----------|----------|------|
| P-01 | GetAllAsync_ShouldReturnAllPests | 返回3条数据 | 返回3条数据 | ✅ 通过 |
| P-02 | GetAllAsync_WithCategoryFilter_ShouldReturnFilteredPests | 返回病害类2条 | 返回2条病害 | ✅ 通过 |
| P-03 | GetAllAsync_WithKeyword_ShouldReturnMatchingPests | 返回匹配"稻瘟"的1条 | 返回1条 | ✅ 通过 |
| P-04 | GetByIdAsync_WithValidId_ShouldReturnPest | 返回稻瘟病 | 返回稻瘟病 | ✅ 通过 |
| P-05 | GetByIdAsync_WithInvalidId_ShouldReturnNull | 返回null | 返回null | ✅ 通过 |
| P-06 | GetByNameAsync_WithValidName_ShouldReturnPest | 返回稻飞虱 | 返回稻飞虱 | ✅ 通过 |
| P-07 | CreateAsync_ShouldAddPest | 新增成功，总数4 | 新增成功，总数4 | ✅ 通过 |
| P-08 | UpdateAsync_WithValidPest_ShouldUpdate | 更新成功 | 更新成功 | ✅ 通过 |
| P-09 | UpdateAsync_WithInvalidId_ShouldReturnFalse | 返回false | 返回false | ✅ 通过 |
| P-10 | DeleteAsync_WithValidId_ShouldDelete | 删除成功 | 删除成功 | ✅ 通过 |
| P-11 | DeleteAsync_WithInvalidId_ShouldReturnFalse | 返回false | 返回false | ✅ 通过 |

### 二、LogService 测试（5个用例）

| 编号 | 测试用例 | 预期结果 | 实际结果 | 状态 |
|------|----------|----------|----------|------|
| L-01 | LogAsync_ShouldAddLog | 日志添加成功 | 添加成功 | ✅ 通过 |
| L-02 | LogAsync_WithNullUserId_ShouldAddLog | 匿名日志添加成功 | 添加成功 | ✅ 通过 |
| L-03 | GetAllAsync_ShouldReturnOrderedByDateDescending | 按时间倒序 | 倒序正确 | ✅ 通过 |
| L-04 | GetAllAsync_WithPaging_ShouldReturnCorrectPage | 分页正确(20+15) | 分页正确 | ✅ 通过 |
| L-05 | GetTotalCountAsync_ShouldReturnCorrectCount | 返回2 | 返回2 | ✅ 通过 |

### 三、DetectionService 测试（8个用例）

| 编号 | 测试用例 | 预期结果 | 实际结果 | 状态 |
|------|----------|----------|----------|------|
| D-01 | DetectAsync_WithValidImage_ShouldReturnRecord | 返回检测记录 | 返回正确记录 | ✅ 通过 |
| D-02 | DetectAsync_WithInvalidExtension_ShouldThrow | 抛出ArgumentException | 抛出异常 | ✅ 通过 |
| D-03 | GetRecordByIdAsync_WithValidId_ShouldReturnRecord | 返回记录 | 返回记录 | ✅ 通过 |
| D-04 | GetRecordsByUserIdAsync_ShouldReturnUserRecords | 返回用户的2条记录 | 返回2条 | ✅ 通过 |
| D-05 | GetAllRecordsAsync_ShouldReturnAllRecords | 返回全部2条 | 返回2条 | ✅ 通过 |
| D-06 | DeleteRecordAsync_WithValidOwner_ShouldDelete | 删除成功 | 删除成功 | ✅ 通过 |
| D-07 | DeleteRecordAsync_WithInvalidOwner_ShouldReturnFalse | 返回false | 返回false | ✅ 通过 |
| D-08 | GetTotalCountAsync_ShouldReturnCorrectCount | 返回2 | 返回2 | ✅ 通过 |

### 四、默认测试（1个用例）

| 编号 | 测试用例 | 预期结果 | 实际结果 | 状态 |
|------|----------|----------|----------|------|
| U-01 | UnitTest1.Test1 | 通过 | 通过 | ✅ 通过 |

## 测试统计

| 指标 | 数值 |
|------|------|
| 测试用例总数 | 26 |
| 通过数 | 26 |
| 失败数 | 0 |
| 跳过数 | 0 |
| 通过率 | 100% |
| 总执行时间 | 517 ms |

## 结论

所有单元测试全部通过，核心业务逻辑（病虫害知识库管理、检测记录管理、系统日志）功能正确。
- PestService 的增删改查、筛选、搜索功能正常
- LogService 的日志记录、分页查询功能正常
- DetectionService 的图像检测、记录查询、删除功能正常
- 文件格式校验功能正常（不支持的格式抛出异常）
