FROM mcr.microsoft.com/dotnet/sdk:5.0 AS publish
WORKDIR /src
COPY ./CasaPopularFamilias/ .
RUN dotnet restore --no-cache --force
RUN dotnet publish -c Release -o /app/publish --no-restore --nologo -v q  /clp:"ErrorsOnly"

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80 
EXPOSE 443
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=https://+:443;http://+:80
ENTRYPOINT ["dotnet", "CasaPopularFamilias.Api.dll"]
