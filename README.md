# Story Translator

This project contains a web app that allows people to read a book, see the translation, and compare it with their own translation of the paragraph.

This project uses C#/Dotnet for the backend and React/TypeScript for the frontend.

## Running

0. This project uses postgresql so it's assumed you have this setup.  If not this will need to be done first.
1. After cloning this project add the necessary postgres database and user in the appsettings or update the appsettings file.
2. Run `dotnet ef database update` to ensure the migrations run to setup the database.
3. Go into the `frontend` folder and run `npm run build` to build the react files.
4. 
- a) Go back one directory and run `SecretKey=DhxXPuZsQWsWeqbXFgqt7rtMj5Vd5ZY3xQSGYeMhqUtXli9MvjxeHrgkOVoYFNkY dotnet run` to run the project.  Note that you can update the Secret Key to a different value as along as it has enough characters.
- b) Click the `Run and Debug` option in VSCode and run the `.NET Core Launch` job
