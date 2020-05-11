FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

#Set locale 
RUN apt-get clean && \
	apt-get update && \
	apt-get install -y locales && \
	sed -i 's/^# *\(en_AU.UTF-8\)/\1/' /etc/locale.gen && \
    locale-gen
ENV LANG en_AU.UTF-8

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/DanielBogdan.Passwordless/DanielBogdan.Passwordless.csproj", "DanielBogdan.Passwordless/"]
RUN dotnet restore "DanielBogdan.Passwordless/DanielBogdan.Passwordless.csproj"
COPY ./src .
WORKDIR "/src/DanielBogdan.Passwordless"
RUN dotnet build "DanielBogdan.Passwordless.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DanielBogdan.Passwordless.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DanielBogdan.Passwordless.dll"]
