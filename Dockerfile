FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root
RUN ln -sf /user/share/zone/zoneinfo/Asia/Shanghai /etc/localtime
RUN echo "Asia/Shanghai" > /etc/timezone
WORKDIR /app
EXPOSE 19001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /main
COPY . .
WORKDIR "/main/src/YayZent.Abp.Web"
RUN dotnet restore "YayZent.Abp.Web.csproj"

FROM build AS publish
WORKDIR "/main/src/YayZent.Abp.Web"
RUN dotnet publish "YayZent.Abp.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YayZent.Abp.Web.dll"]