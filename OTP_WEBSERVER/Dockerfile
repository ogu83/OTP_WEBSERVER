FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["OTP_WEBSERVER/OTP_WEBSERVER.csproj", "OTP_WEBSERVER/"]
RUN dotnet restore "OTP_WEBSERVER/OTP_WEBSERVER.csproj"
COPY . .
WORKDIR "/src/OTP_WEBSERVER"
RUN dotnet build "OTP_WEBSERVER.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "OTP_WEBSERVER.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OTP_WEBSERVER.dll"]