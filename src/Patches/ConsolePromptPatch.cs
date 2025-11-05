using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using CloverPit_CommandPromptWithArgs;

namespace CloverPit_CommandPromptWithArgs.Patches;

[HarmonyPatch(typeof(ConsolePrompt), "TryExecuteCommand")]
class ConsolePromptPatch : HarmonyPatch
{
    static bool Prefix(ConsolePrompt __instance)
    {
        Type type = __instance.GetType();

        FieldInfo inputStringField = AccessTools.Field(type, "inputString");
        FieldInfo executionIndexField = AccessTools.Field(type, "executionIndex");

        if (inputStringField == null || executionIndexField == null)
        {
            Plugin.Logger.LogError("I don't know why or how this happened but its bad.");
            return false;
        }

        string inputString = (string)inputStringField.GetValue(__instance);
        int executionIndex = (int)executionIndexField.GetValue(__instance);

        if (string.IsNullOrEmpty(inputString))
        {
            return false;
        }
        
        if (inputString.Length == 0) return false;

        executionIndex++;
        inputString = inputString.ToLowerInvariant  ();

        string[] words = inputString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string command = words[0].Trim();
        List<string> args = [];

        if (words.Length > 1) 
        {
            args.AddRange(words[1..]);
        }

        Plugin.Logger.LogInfo($"Command is : {command}");

        CustomCommands.CustomCommand commandObj = null;

        if (CustomCommands.availableCustomCommands.ContainsKey(command))
            commandObj = CustomCommands.availableCustomCommands[command];

        if (commandObj != null)
        {
            bool success = false;

            try
            {
                success = commandObj.TryExecute(args.ToArray());
            }
            catch (Exception execption)
            {
                Plugin.Logger.LogError($"Failed to call TryExecute: {execption}");
            }

            if (!success)
            {
                Plugin.Logger.LogError($"Failed to run command {command}");
            }
        }
        else
        {
            AccessTools.Field(type, "inputString").SetValue(__instance, command);
            return true;
        }

        AccessTools.Field(type, "inputStringOld").SetValue(__instance, inputString);
        AccessTools.Field(type, "inputString").SetValue(__instance, "");
        AccessTools.Field(type, "executionIndex").SetValue(__instance, executionIndex);

        return false;
    }
}

