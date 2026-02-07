FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0.302 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/Order/NSE.Order.API/NSE.Order.API.csproj", "services/Order/NSE.Order.API/"]
COPY ["building-blocks/NSE.WebAPI.Core/NSE.WebAPI.Core.csproj", "building-blocks/NSE.WebAPI.Core/"]
COPY ["building-blocks/NSE.Core/NSE.Core.csproj", "building-blocks/NSE.Core/"]
COPY ["building-blocks/NSE.Security/NSE.Security.csproj", "building-blocks/NSE.Security/"]
COPY ["building-blocks/NSE.Queue/NSE.Queue.csproj", "building-blocks/NSE.Queue/"]
COPY ["services/Order/NSE.Order.Application/NSE.Order.Application.csproj", "services/Order/NSE.Order.Application/"]
COPY ["services/Order/NSE.Order.Domain/NSE.Order.Domain.csproj", "services/Order/NSE.Order.Domain/"]
COPY ["services/Order/NSE.Order.Infra/NSE.Order.Infra.csproj", "services/Order/NSE.Order.Infra/"]
RUN dotnet restore "services/Order/NSE.Order.API/NSE.Order.API.csproj"
COPY . .
WORKDIR "services/Order/NSE.Order.API"
RUN dotnet build "NSE.Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NSE.Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NSE.Order.API.dll"]
