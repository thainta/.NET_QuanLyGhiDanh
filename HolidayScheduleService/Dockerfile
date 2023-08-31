FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HolidayScheduleService/HolidayScheduleService.csproj", "HolidayScheduleService/"]
RUN dotnet restore "HolidayScheduleService/HolidayScheduleService.csproj"
COPY . .
WORKDIR "/src/HolidayScheduleService"
RUN dotnet build "HolidayScheduleService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HolidayScheduleService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HolidayScheduleService.dll"]
