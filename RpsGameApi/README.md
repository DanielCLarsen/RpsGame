# Disclaimer
- This project is the result of a code challenge and should be considered in that scope. This projects purpose is to showcase clean, maintanable, robust, scalable and readable code. The code-structure and naming should be the focus of attention.

# How to run?
- Postman setup has been exported into file "PostmanExport.json" and can be imported into postman.
- Unit tests can be run by navigating to the RpsGame\RpsGameApi.Tests directory and executing "dotnet test" in the terminal
- API can be run by navigating to the RpsGame\RpsGameApi directory and executing "dotnet run" in the terminal

# Areas of improvement?
- Inform who you are playing against when joining the game
- Nicer responses that are not showing the game object json, but instead gives a nice formatted message
- Multiple rounds (best out of..)
- UI frontend
- RPS Tournaments
- Logging
- Persistance mechanic

# Technologies used?
- Github and VSCode git extension
- C# .net core
- Postman
- VScode with the extensions:
    - .NET Install Tool
    - C#
    - C# Dev Kit
    - Git Graph
    - Intellicode for C# Dev Kit
    - Todo Tree
    - .NET Core Test Explorer
- ChatGPT

# Example flow
- P1 creates game
- P2 joins game
- P1 and P2 sends their moves
- A winner or tie is announced
- Gamestate can always be gotten in any state of this flow.