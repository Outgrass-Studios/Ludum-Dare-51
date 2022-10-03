using System.Collections;
using System.Collections.Generic;
using qASIC.Console.Commands;

namespace Game.Commands
{
    public class FreeCameraCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "freecamera";
        public override string Description { get; } = "frees the camera from it's current camera track";
        
        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;
            Camera.CameraController.Main.SetTrack(null);
            Log("Camera has been unlocked", "cheat");
        }
    }
}