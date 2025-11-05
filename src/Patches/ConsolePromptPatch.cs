using System;
using System.Collections.Generic;
using HarmonyLib;

namespace CloverPit_CommandPromptWithArgs.Patches;

[HarmonyPatch(typeof(ConsolePrompt), "TryExecuteCommand")]
class ConsolePromptPatch : HarmonyPatch
{
    static bool Prefix(ConsolePrompt __instance, ref string ___inputString, ref string ___inputStringOld, ref int ___executionIndex)
    {
        string inputString = ___inputString;
        int executionIndex = ___executionIndex;

        if (string.IsNullOrEmpty(inputString))
        {
            return false;
        }
        
        if (inputString.Length == 0) return false;

        executionIndex++;
        inputString = inputString.ToLowerInvariant();

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
            ___inputString = command;
            return true;
        }

        ___inputStringOld = inputString;
        ___inputString = "";
        ___executionIndex = executionIndex;

        return false;
    }
}

