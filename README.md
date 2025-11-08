# CloverPit_CommandPromptWithArgs
A library mod for CloverPit that allows for custom commands that take arguments.

## Usage
Include CloverPit_CommandPromptWithArgs as a dependency for BepInEx.
Include a reference to the dll in your .csproj file.

Add custom commands with function:
CustomCommands.AddCustomCommand(string[] aliases, string description, UnityAction<string[]> action)

## To-do
-[ ] Add help command
-[ ] Categorize commands based on mod in help command
