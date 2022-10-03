using System.Collections;
using System.Collections.Generic;
using Game.Player;
using qASIC.Console.Commands;

namespace Game.Commands
{
    public class FlightCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "fly";
        public override string Description { get; } = "toggles flight";
        
        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;
            PlayerMovementToggler.FlightActive = !PlayerMovementToggler.FlightActive;
            Log($"Flight has been {(PlayerMovementToggler.FlightActive ? "enabled" : "disabled")}", "cheat");
        }
    }
}