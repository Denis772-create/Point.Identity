FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Point.Services.Identity.Api/Point.Services.Identity.Web.csproj", "Point.Services.Identity.Api/"]
COPY ["Point.Services.Identity.Application/Point.Services.Identity.Application.csproj", "Point.Services.Identity.Application/"]
COPY ["Point.Services.Identity.Domain/Point.Services.Identity.Domain.csproj", "Point.Services.Identity.Domain/"]
COPY ["Point.Services.Identity.Shared/Point.Services.Identity.Shared.csproj", "Point.Services.Identity.Shared/"]
COPY ["Point.Services.Identity.Infrastructure/Point.Services.Identity.Infrastructure.csproj", "Point.Services.Identity.Infrastructure/"]
RUN dotnet restore "Point.Services.Identity.Api/Point.Services.Identity.Web.csproj"
COPY . .
WORKDIR "/src/Point.Services.Identity.Api"
RUN dotnet build "Point.Services.Identity.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Point.Services.Identity.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Point.Services.Identity.Web.dll"]