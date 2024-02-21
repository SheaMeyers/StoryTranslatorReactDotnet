# Story Translator

This project contains a web app that allows people to read a book, see the translation, and compare it with their own translation of the paragraph.

This project uses C#/Dotnet for the backend and React/TypeScript for the frontend.

## Running

0. This project uses postgresql so it's assumed you have this setup.  If not this will need to be done first.
Create a database and a user with the password with the values `storytranslatorreactdotnet` for each.
1. After cloning this project ensure the necessary database and database user are set up or update the appsettings 
files with the correct settings for the database you setup
2. Run `dotnet ef database update` to ensure the migrations run to setup the database.
3. Go into the `frontend` folder and run `npm install` and then `npm run build` to build the react files.
4. 
- a) Go back one directory and run `SecretKey=DhxXPuZsQWsWeqbXFgqt7rtMj5Vd5ZY3xQSGYeMhqUtXli9MvjxeHrgkOVoYFNkY dotnet run` to run the project.  Note that you can update the Secret Key to a different value as along as it has enough characters.
- b) Click the `Run and Debug` option in VSCode and run the `.NET Core Launch` job

## Running with Docker

This project does container a Dockerfile and docker-compose.yml.  To run the project you can simply run `docker compose up` in order to run the project and skip the steps above.

## Initialized Data

This project will add initial test data by default to help demonstrate the functionality of this application.  To turn this off set `InitializeData` in the `appsettings.json` files

## Unit tests

This project contains unit tests.  To run them first create a database `storytranslatorreactdotnettest` which the tests will use and then run `dotnet test` to run the tests.
