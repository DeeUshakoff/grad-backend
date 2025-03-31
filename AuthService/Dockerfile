FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY AuthService/*.csproj ./AuthService/
COPY GradBackend.ServiceDefaults/*.csproj ./GradBackend.ServiceDefaults/
RUN dotnet restore ./AuthService/AuthService.csproj

COPY AuthService/ ./AuthService/
COPY GradBackend.ServiceDefaults/ ./GradBackend.ServiceDefaults/

WORKDIR /src/AuthService
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app .

EXPOSE 7144

ENTRYPOINT ["dotnet", "AuthService.dll"]