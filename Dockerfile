FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./

RUN apt-get update && apt-get upgrade -y && \
    apt-get install -y nodejs \
    npm

# Build frontend
RUN cd frontend; npm install; npm run build; cd ..

# Restore as distinct layers
RUN dotnet restore

# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/out .
ENV SecretKey=DhxXPuZsQWsWeqbXFgqt7rtMj5Vd5ZY3xQSGYeMhqUtXli9MvjxeHrgkOVoYFNkY
ENTRYPOINT ["dotnet", "StoryTranslatorReactDotnet.dll"]
