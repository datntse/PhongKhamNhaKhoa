FROM mcr.microsoft.com/dotnet/aspnet:6.0 as base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Clinic.Core/Clinic.Core.csproj", "Clinic.Core/"]
COPY ["Clinic.Infracstructure/Clinic.Infracstructure.csproj", "Clinic.Infracstructure/"]
COPY ["Clinic.API/Clinic.API.csproj","Clinic.API/"]
RUN dotnet restore "Clinic.API/Clinic.API.csproj"
COPY . .
WORKDIR "/src/Clinic.API"
RUN dotnet build "Clinic.API.csproj" -c ${BUILD_CONFIGURATION} -o /app/build

FROM build as publish
ARG updateDb
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Clinic.API.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "Clinic.API.dll" ]