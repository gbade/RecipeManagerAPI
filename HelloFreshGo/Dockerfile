FROM microsoft/dotnet:2.2-aspnetcore-runtime
FROM microsoft/dotnet:2.2-sdk

COPY dist /app
WORKDIR /app
#COPY dist /app
#COPY "HelloFreshGo.xml" /app

EXPOSE 80/tcp

ENTRYPOINT ["dotnet", "HelloFreshGo.dll"]