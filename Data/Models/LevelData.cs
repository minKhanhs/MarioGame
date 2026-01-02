using MarioGame.Core;
using System.Collections.Generic;

namespace MarioGame.Data.Models
{
    public class LevelData
    {
        public int LevelId { get; set; }
        public string Name { get; set; }
        public float TimeLimit { get; set; }
        public string Background { get; set; }
        public string Music { get; set; }
        public CameraMode CameraMode { get; set; }
        public MapData MapData { get; set; }
        public List<EntityData> Entities { get; set; }
    }

    public class MapData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int[] TileMatrix { get; set; }
    }

    public class EntityData
    {
        public string Type { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}