FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

COPY *.sln .
COPY Colegio/*.csproj ./Colegio/
RUN dotnet restore

COPY . ./Colegio/
WORKDIR /source/
RUN dotnet publish -c Release -o /app --self-contained true --runtime linux-x64 --framework net6.0 /p:RuntimeFrameworkVersion=6.0.0

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app/out
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Student.dll"]
