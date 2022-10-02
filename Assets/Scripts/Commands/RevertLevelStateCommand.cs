using System.Collections.Generic;
using Game.Checkpoints;
using qASIC.Console.Commands;

namespace Game.Commands
{
    public class RevertLevelStateCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "revertlevelstate";
        public override string Description { get; } = "Reverts the level's state";
        public override string[] Aliases { get; } = new string[] { "rls" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;
            LevelCheckpointManager.RevertState();
        }
    }
}