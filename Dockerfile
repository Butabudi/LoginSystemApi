#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Default value to use if no version is provided
ARG version=1.0.0.0

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS test
WORKDIR /
COPY src src
COPY test test
RUN dotnet restore "test/LoginSystem.Api.Tests/LoginSystem.Api.Tests.csproj"
RUN dotnet test "test/LoginSystem.Api.Tests/LoginSystem.Api.Tests.csproj"

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG version
WORKDIR /src
COPY --from=test src/. .
RUN dotnet restore "LoginSystem.Api/LoginSystem.Api.csproj"
RUN dotnet publish "LoginSystem.Api/LoginSystem.Api.csproj" -c Release -o /app/publish -p:Version=${version}

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LoginSystem.Api.dll"]
