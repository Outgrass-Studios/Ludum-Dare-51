using System.Collections;
using System.Collections.Generic;
using Game.Player;
using qASIC.Console.Commands;

namespace Game.Commands
{
    public class CheatShortcutCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "g";
        public override string Description { get; } = "-";
        
        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;
            PlayerCheatShortcuts.EnableShortcuts = !PlayerCheatShortcuts.EnableShortcuts;
            Log($"Cheats {(PlayerCheatShortcuts.EnableShortcuts ? "activated" : "deactivated")}", "cheat");
        }
    }
}