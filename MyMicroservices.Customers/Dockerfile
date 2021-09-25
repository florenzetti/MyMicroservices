#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MyMicroservices.Customers/MyMicroservices.Customers.csproj", "MyMicroservices.Customers/"]
RUN dotnet restore "MyMicroservices.Customers/MyMicroservices.Customers.csproj"
COPY . .
WORKDIR "/src/MyMicroservices.Customers"
RUN dotnet build "MyMicroservices.Customers.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyMicroservices.Customers.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyMicroservices.Customers.dll"]