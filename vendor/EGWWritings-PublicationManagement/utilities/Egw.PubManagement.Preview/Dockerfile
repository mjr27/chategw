ARG SDK_VERSION=7.0-alpine
FROM mcr.microsoft.com/dotnet/sdk:${SDK_VERSION} AS build-env
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
WORKDIR /app/utilities/Egw.PubManagement.Preview
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o /release

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:${SDK_VERSION}
WORKDIR /app
ARG APP_PORT=8080
ARG USER_ID=65534
ENV ASPNETCORE_URLS=http://+:${APP_PORT}
ENV COMPlus_EnableDiagnostics=0
EXPOSE ${APP_PORT}
COPY --from=build-env /release .
USER ${USER_ID}
CMD ["./Egw.PubManagement.Preview"]
