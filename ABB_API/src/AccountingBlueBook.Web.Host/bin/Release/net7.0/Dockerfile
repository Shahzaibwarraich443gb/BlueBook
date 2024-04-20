FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src
COPY ["src/AccountingBlueBook.Web.Host/AccountingBlueBook.Web.Host.csproj", "src/AccountingBlueBook.Web.Host/"]
COPY ["src/AccountingBlueBook.Web.Core/AccountingBlueBook.Web.Core.csproj", "src/AccountingBlueBook.Web.Core/"]
COPY ["src/AccountingBlueBook.Application/AccountingBlueBook.Application.csproj", "src/AccountingBlueBook.Application/"]
COPY ["src/AccountingBlueBook.Core/AccountingBlueBook.Core.csproj", "src/AccountingBlueBook.Core/"]
COPY ["src/AccountingBlueBook.EntityFrameworkCore/AccountingBlueBook.EntityFrameworkCore.csproj", "src/AccountingBlueBook.EntityFrameworkCore/"]
WORKDIR "/src/src/AccountingBlueBook.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/AccountingBlueBook.Web.Host", "src/AccountingBlueBook.Web.Host"]
COPY ["src/AccountingBlueBook.Web.Core", "src/AccountingBlueBook.Web.Core"]
COPY ["src/AccountingBlueBook.Application", "src/AccountingBlueBook.Application"]
COPY ["src/AccountingBlueBook.Core", "src/AccountingBlueBook.Core"]
COPY ["src/AccountingBlueBook.EntityFrameworkCore", "src/AccountingBlueBook.EntityFrameworkCore"]
WORKDIR "/src/src/AccountingBlueBook.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "AccountingBlueBook.Web.Host.dll"]
