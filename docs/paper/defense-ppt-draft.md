# 水稻病虫害检测系统 - 答辩PPT草稿

> **说明**：本文档为答辩PPT的内容草稿，需人工整理为PPT格式。

---

## 第1页：封面

**标题**：基于ASP.NET Core与ML.NET的水稻病虫害检测系统设计与实现

**副标题**：毕业设计答辩

**答辩人**：XXX
**指导教师**：XXX
**日期**：2026年X月

---

## 第2页：研究背景与意义

### 研究背景
- 水稻是我国最重要的粮食作物，病虫害严重影响产量
- 传统人工诊断效率低、依赖专家、易延误防治时机
- 深度学习与计算机视觉技术为病虫害识别提供新路径

### 研究意义
- 降低农户识别病虫害的技术门槛
- 提高诊断效率，及时防治，减少损失
- 探索AI模型在.NET平台的工程化部署

---

## 第3页：国内外研究现状

### 国外
- Mohanty等（2016）：AlexNet/GoogleNet，PlantVillage数据集，准确率99.35%
- Ferentinos（2018）：VGG/AlexNet，87848张图像，准确率99.53%

### 国内
- 张建华等（2018）：改进VGG，玉米病害识别，95.2%
- 刘耀等（2020）：YOLOv3，水稻病害检测，mAP 82.6%

### 研究空白
- 现有研究多关注算法，较少关注工程化部署
- 缺乏面向普通农户的便捷Web应用

---

## 第4页：系统架构设计

### 四层分层架构

```
表现层 (Controllers + Views + Bootstrap 5)
    ↓
业务逻辑层 (UserService / PestService / DetectionService / StatisticsService / LogService)
    ↓
数据访问层 (AppDbContext + EF Core + SQLite)
    ↓
机器学习推理层 (ML.NET + ONNX模型)
```

### 技术栈
- 后端：ASP.NET Core 8 + C# 12
- ORM：Entity Framework Core 8
- 数据库：SQLite
- 前端：Razor + Bootstrap 5
- 机器学习：ML.NET 5.0 + ONNX
- 图表：ECharts

---

## 第5页：数据库设计

### 4张核心数据表

| 表名 | 说明 | 关键字段 |
|------|------|----------|
| Users | 用户表 | UserName, PasswordHash, Role, IsActive |
| Pests | 病虫害表 | Name, Category, Symptoms, PreventionMethod |
| DetectionRecords | 检测记录 | UserId, PestId, ResultName, Confidence |
| Logs | 系统日志 | UserId, ActionType, Content |

### 关系
- User 1:N DetectionRecord
- User 1:N Log
- Pest 1:N DetectionRecord

---

## 第6页：核心功能模块

### 六大功能模块

1. **用户管理**：注册、登录、角色权限（管理员/普通用户）
2. **病虫害知识库**：浏览、搜索、详情、维护（7种常见病虫害）
3. **图像检测**：上传图片 → ML.NET推理 → 结果+防治建议
4. **检测记录**：历史记录查询、详情、删除
5. **数据统计**：检测趋势折线图、病虫害分布饼图
6. **系统管理**：用户管理、知识维护、操作日志

---

## 第7页：图像检测核心流程

```
上传图片 → 文件校验 → 保存图片
    ↓
加载ONNX模型 → 图像预处理(Resize+Normalize)
    ↓
模型推理 → 获取分类结果与置信度
    ↓
匹配知识库 → 保存检测记录
    ↓
展示结果（病虫害名称、置信度、防治建议）
```

### 关键技术
- ML.NET加载ONNX模型
- LoadImages → ResizeImages → ExtractPixels → ApplyOnnxModel
- 支持真实模型和模拟预测降级模式

---

## 第8页：系统演示

### 演示要点（配截图）

1. **首页**：统计卡片 + 病虫害列表
2. **知识库**：病虫害卡片 + 筛选搜索
3. **图像检测**：拖拽上传 + 检测结果展示
4. **检测记录**：历史记录表格
5. **数据统计**：ECharts折线图 + 饼图
6. **系统管理**：用户管理 + 操作日志

---

## 第9页：系统测试

### 单元测试
- 测试框架：xUnit + Moq + FluentAssertions
- 用例数：26个
- 通过率：100%
- 覆盖：PestService、LogService、DetectionService

### 功能测试
- 首页、登录、知识库、检测、记录、统计页面均正常
- 管理员登录、权限控制正常

### 性能
- 首页加载：~200ms
- 检测响应：500ms-2s

---

## 第10页：总结与展望

### 工作总结
- 实现了基于ASP.NET Core + ML.NET的水稻病虫害检测系统
- 六大功能模块完整，测试通过率100%
- 通过ONNX实现深度学习模型的Web端部署

### 不足与展望
1. **模型精度**：收集更多数据优化模型
2. **检测范围**：扩展更多病虫害类别
3. **移动端**：开发微信小程序/APP
4. **实时预警**：结合地理位置构建预警系统

---

## 第11页：致谢

感谢指导教师的悉心指导！
感谢评审老师的宝贵意见！

**Q & A**

---

> **说明**：本PPT草稿需人工补充系统截图、调整版式和动画效果。
