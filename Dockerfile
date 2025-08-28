# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Mashal/lawyer.Api.csproj", "Mashal/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Repo/Repo.csproj", "Repo/"]
COPY ["Serv/Serv.csproj", "Serv/"]
RUN dotnet restore "Mashal/lawyer.Api.csproj"
COPY . .
WORKDIR "/src/Mashal"
RUN dotnet build "lawyer.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "lawyer.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "lawyer.Api.dll"]