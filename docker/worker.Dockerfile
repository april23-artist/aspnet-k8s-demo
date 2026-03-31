FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY src/ .
RUN dotnet restore Demo.Worker/Demo.Worker.csproj
RUN dotnet publish Demo.Worker/Demo.Worker.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "Demo.Worker.dll"]