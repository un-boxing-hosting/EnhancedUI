using System;
using System.Collections.Generic;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI.Interfaces;
using SharpDX;
using VRageMath;

namespace EnhancedUI.Gui.Terminal.ControlPanel
{
    public class BlockState
    {
        public readonly string ClassName;
        public readonly string TypeId;
        public readonly string SubtypeName;

        public readonly long EntityId;
        public readonly string Name;
        public readonly string DetailedInfo;
        public readonly string CustomData;

        // ReSharper disable once CollectionNeverQueried.Global
        public readonly Dictionary<string, BlockPropertyState> PropertyStates = new();

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public BlockState(MyTerminalBlock block)
        {
            ClassName = block.GetType().Name;
            TypeId = block.BlockDefinition.Id.TypeId.ToString();
            SubtypeName = block.BlockDefinition.Id.SubtypeName;

            EntityId = block.EntityId;
            Name = block.CustomName.ToString();
            if (Name == "")
            {
                Name = block.DisplayNameText ?? block.DisplayName;
            }

            DetailedInfo = block.DetailedInfo.ToString();
            CustomData = block.CustomData;

            var properties = new List<ITerminalProperty>();
            block.GetProperties(properties, null);
            foreach (var property in properties)
            {
                PropertyStates[property.Id] = new BlockPropertyState(block, property);
            }
        }
    }

    public class BlockPropertyState
    {
        public readonly string Id;
        public readonly string TypeName;
        public object? Value;

        public BlockPropertyState(MyTerminalBlock block, ITerminalProperty property)
        {
            Id = property.Id;
            TypeName = property.TypeName;

            switch (property.TypeName)
            {
                case "Boolean":
                    Value = property.AsBool().GetValue(block);
                    break;

                case "Single":
                    Value = property.AsFloat().GetValue(block);
                    break;

                case "Int64":
                    Value = property.As<Int64>().GetValue(block);
                    break;

                case "Color":
                    Value = property.As<Color>().GetValue(block).ToString();
                    break;
            }
        }
    }
}