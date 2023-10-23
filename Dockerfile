# Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
COPY . .

RUN dotnet restore
RUN dotnet publish -c release -o /app --no-restore

# Serve
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 5001

ENTRYPOINT [ "dotnet", "Presentation.dll" ]