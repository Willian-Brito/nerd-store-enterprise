FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0.302 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/ShoppingCart/NSE.ShoppingCart.API/NSE.ShoppingCart.API.csproj", "services/ShoppingCart/NSE.ShoppingCart.API/"]
COPY ["building-blocks/NSE.WebAPI.Core/NSE.WebAPI.Core.csproj", "building-blocks/NSE.WebAPI.Core/"]
COPY ["building-blocks/NSE.Core/NSE.Core.csproj", "building-blocks/NSE.Core/"]
COPY ["building-blocks/NSE.Security/NSE.Security.csproj", "building-blocks/NSE.Security/"]
COPY ["building-blocks/NSE.Queue/NSE.Queue.csproj", "building-blocks/NSE.Queue/"]
RUN dotnet restore "services/ShoppingCart/NSE.ShoppingCart.API/NSE.ShoppingCart.API.csproj"
COPY . .
WORKDIR "services/ShoppingCart/NSE.ShoppingCart.API"
RUN dotnet build "NSE.ShoppingCart.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NSE.ShoppingCart.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NSE.ShoppingCart.API.dll"]
