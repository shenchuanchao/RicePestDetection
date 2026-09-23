# 构建阶段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 复制项目文件并还原依赖
COPY ["src/RicePestDetection.Web/RicePestDetection.Web.csproj", "src/RicePestDetection.Web/"]
RUN dotnet restore "src/RicePestDetection.Web/RicePestDetection.Web.csproj"

# 复制全部源代码并构建
COPY . .
WORKDIR "/src/src/RicePestDetection.Web"
RUN dotnet build "RicePestDetection.Web.csproj" -c Release -o /app/build

# 发布阶段
FROM build AS publish
RUN dotnet publish "RicePestDetection.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 运行阶段
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

# 复制发布产物
COPY --from=publish /app/publish .

# 创建上传目录
RUN mkdir -p /app/wwwroot/uploads/pests

# 配置环境变量
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "RicePestDetection.Web.dll"]
