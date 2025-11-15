using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace CloverPit_CommandPromptWithArgs;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]

public class Plugin : BaseUnityPlugin
{
    public static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        Harmony harmony = new("CloverPit_CommandPromptWithArgs");
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        CustomCommands.AddCustomCommand(["help"], "List all available commands with description.", Help);
    }

    public void Help(string[] args)
    {
        ConsolePrompt.Log("--------------------");
        ConsolePrompt.Log("Available commands:");

        FieldInfo availableCommandsField = AccessTools.Field(typeof(ConsolePrompt), "availableCommands");

        IDictionary availableCommands = (IDictionary)availableCommandsField.GetValue(ConsolePrompt.instance);

        List<object> commandsList = [];

        Type commandType = AccessTools.Inner(typeof(ConsolePrompt), "Command");
        FieldInfo allowedEntriesField = AccessTools.Field(commandType, "allowedEntries");
        FieldInfo descriptionField = AccessTools.Field(commandType, "description");

        foreach (DictionaryEntry entry in availableCommands)
        {
            object cmdObj = entry.Value;

            if (!commandsList.Contains(cmdObj))
            {
                string[] allowedEntries = (string[])allowedEntriesField.GetValue(cmdObj);
                string description = (string)descriptionField.GetValue(cmdObj);

                ConsolePrompt.Log($"- {allowedEntries[0]} : {description}");

                commandsList.Add(cmdObj);
            }
        }

        ConsolePrompt.Log("--------------------");
        ConsolePrompt.Log("Available custom commands:");
        List<CustomCommands.CustomCommand> customCommandsList = [];

        foreach (KeyValuePair<string, CustomCommands.CustomCommand> availableCustomCommand in CustomCommands.availableCustomCommands)
        {
            if (!customCommandsList.Contains(availableCustomCommand.Value))
            {
                ConsolePrompt.Log("- " + availableCustomCommand.Value.allowedEntries[0] + " : " + availableCustomCommand.Value.description);
                customCommandsList.Add(availableCustomCommand.Value);
            }
        }

        ConsolePrompt.Log("--------------------");
        ConsolePrompt.Log("- Scroll up console: Ctrl + Up arrow");
        ConsolePrompt.Log("- Scroll down console: Ctrl + Down arrow");
        ConsolePrompt.Log("- Scroll ALL up console: Ctrl + Page Up");
        ConsolePrompt.Log("- Scroll ALL down console: Ctrl + Page Down");
    }
}
