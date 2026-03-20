FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY src/ .
RUN dotnet restore Demo.Api/Demo.Api.csproj
RUN dotnet publish Demo.Api/Demo.Api.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "Demo.Api.dll"]