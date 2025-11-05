# CloverPit_CommandPromptWithArgs
A library mod for clover pit that allows for custom commands that take arguments

## Usage
include CloverPit_CommandPromptWithArgs as a dependancy for BepInEx
include a reference to the dll in your .csproj file

Add custom commands with function:
CustomCommands.AddCustomCommand(string[] aliases, string description, UnityAction<string[]> action)
