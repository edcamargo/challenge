FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy csproj and restore as distinct layers
COPY ./Presentation.Api/Presentation.Api.csproj ./Presentation.Api/
COPY ./Application/Application.csproj ./Application/
COPY ./Domain/Domain.csproj ./Domain/
COPY ./InfraStructure.Data/InfraStructure.Data.csproj ./InfraStructure.Data/
COPY ./InfraStructure.Ioc/InfraStructure.Ioc.csproj ./InfraStructure.Ioc/

RUN dotnet restore "Presentation.Api/Presentation.Api.csproj"
COPY . .
WORKDIR "/src/Presentation.Api"
RUN dotnet build "./Presentation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Presentation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /src
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Presentation.Api.dll"]

