FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["Evently.Api/Evently.Api.csproj", "Evently.Api/"]
COPY ["Common/Evently.Common.Presentation/Evently.Common.Presentation.csproj", "Common/Evently.Common.Presentation/"]
COPY ["Common/Evently.Common.Domain/Evently.Common.Domain.csproj", "Common/Evently.Common.Domain/"]
COPY ["Common/Evently.Common.Application/Evently.Common.Application.csproj", "Common/Evently.Common.Application/"]
COPY ["Common/Evently.Common.Infrastructure/Evently.Common.Infrastructure.csproj", "Common/Evently.Common.Infrastructure/"]
COPY ["Users/Evently.Modules.Users.Features/Evently.Modules.Users.Features.csproj", "Users/Evently.Modules.Users.Features/"]
COPY ["Users/Evently.Modules.Users.Infrastructure/Evently.Modules.Users.Infrastructure.csproj", "Users/Evently.Modules.Users.Infrastructure/"]
COPY ["Users/Evently.Modules.Users.Domain/Evently.Modules.Users.Domain.csproj", "Users/Evently.Modules.Users.Domain/"]
RUN dotnet restore "Evently.Api/Evently.Api.csproj"
COPY . .
WORKDIR "/src/Evently.Api"
RUN dotnet build "./Evently.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Evently.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Evently.Api.dll"]
