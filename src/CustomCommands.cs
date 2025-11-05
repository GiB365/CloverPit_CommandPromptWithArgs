using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace CloverPit_CommandPromptWithArgs;

public class CustomCommands
{
    public class CustomCommand
    {
        public string[] allowedEntries;

        public string description;

        public UnityAction<string[]> action;

        public CustomCommand(string[] allowedEntries, string description, UnityAction<string[]> action)
        {
            this.allowedEntries = allowedEntries;
            this.description = description;
            this.action = action;
            foreach (string key in allowedEntries)
            {
                availableCustomCommands.Add(key, this);
            }
        }

        public bool TryExecute(string[] args)
        {
            if (action != null)
            {
                action(args);
                return true;
            }
            return false;
        }

        internal bool TryExecute(List<string> args)
        {
            throw new NotImplementedException();
        }
    }
    public static Dictionary<string, CustomCommand> availableCustomCommands = [];

    public static void AddCustomCommand(string[] aliases, string description, UnityAction<string[]> action)
    {
        new CustomCommand(aliases, description, action);
    }
}
