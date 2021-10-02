using System;
using Sandbox.Game.Entities.Cube;

namespace EnhancedUI.Gui.Terminal.ControlPanel
{
    public class BlockInfo
    {
        public readonly string Name;

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public BlockInfo(MyFunctionalBlock block)
        {
            Name = block.CustomName.ToString();
            if (Name == "")
            {
                Name = block.DisplayNameText ?? block.DisplayName;
            }
        }
    }
}