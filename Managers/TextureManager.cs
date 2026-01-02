using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MarioGame.Managers
{
    public class TextureManager
    {
        private static TextureManager instance;
        public static TextureManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new TextureManager();
                return instance;
            }
        }

        private Dictionary<string, Texture2D> textures;
        private GraphicsDevice graphicsDevice;

        private TextureManager()
        {
            textures = new Dictionary<string, Texture2D>();
        }

        public void Initialize(GraphicsDevice device)
        {
            graphicsDevice = device;
            CreateDefaultTextures();
        }

        // Load all game textures
        public void LoadContent(ContentManager content)
        {
            try
            {
                // Load player sprites
                LoadTexture(content, "mario_small", "Sprites/mario_small");
                LoadTexture(content, "mario_big", "Sprites/mario_big");
                LoadTexture(content, "mario_fire", "Sprites/mario_fire");

                // Load enemy sprites
                LoadTexture(content, "goomba", "Sprites/goomba");
                LoadTexture(content, "koopa", "Sprites/koopa");
                LoadTexture(content, "piranha_plant", "Sprites/piranha_plant");

                // Load item sprites
                LoadTexture(content, "coin", "Sprites/coin");
                LoadTexture(content, "mushroom", "Sprites/mushroom");
                LoadTexture(content, "fire_flower", "Sprites/fire_flower");
                LoadTexture(content, "1up_mushroom", "Sprites/1up_mushroom");
                LoadTexture(content, "star", "Sprites/star");
                LoadTexture(content, "fireball", "Sprites/fireball");

                // Load tile sprites
                LoadTexture(content, "tiles", "Sprites/tiles");
                LoadTexture(content, "ground", "Sprites/ground");
                LoadTexture(content, "brick", "Sprites/brick");
                LoadTexture(content, "question_block", "Sprites/question_block");
                LoadTexture(content, "pipe", "Sprites/pipe");

                // Load backgrounds
                LoadTexture(content, "bg_overworld", "Backgrounds/bg_overworld");
                LoadTexture(content, "bg_underground", "Backgrounds/bg_underground");
                LoadTexture(content, "bg_castle", "Backgrounds/bg_castle");

                // Load UI elements
                LoadTexture(content, "heart", "UI/heart");
                LoadTexture(content, "coin_icon", "UI/coin_icon");

                Console.WriteLine($"Loaded {textures.Count} textures successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading textures: {ex.Message}");
                Console.WriteLine("Using default colored rectangles instead");
            }
        }

        // Load a single texture
        private void LoadTexture(ContentManager content, string key, string assetPath)
        {
            try
            {
                textures[key] = content.Load<Texture2D>(assetPath);
                Console.WriteLine($"Loaded: {key}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load {key}: {ex.Message}");
                // Create default colored texture
                textures[key] = CreateColoredTexture(GetDefaultColor(key), 32, 32);
            }
        }

        // Get texture by key
        public Texture2D GetTexture(string key)
        {
            if (textures.ContainsKey(key))
                return textures[key];

            Console.WriteLine($"Texture '{key}' not found, using default");
            return GetDefaultTexture();
        }

        // Check if texture exists
        public bool HasTexture(string key)
        {
            return textures.ContainsKey(key);
        }

        // Create default placeholder textures
        private void CreateDefaultTextures()
        {
            if (graphicsDevice == null) return;

            // Default white pixel
            textures["pixel"] = CreateColoredTexture(Color.White, 1, 1);

            // Player placeholders
            textures["mario_small"] = CreateColoredTexture(Color.Red, 32, 32);
            textures["mario_big"] = CreateColoredTexture(Color.Red, 32, 64);
            textures["mario_fire"] = CreateColoredTexture(Color.Orange, 32, 64);

            // Enemy placeholders
            textures["goomba"] = CreateColoredTexture(Color.Brown, 32, 32);
            textures["koopa"] = CreateColoredTexture(Color.Green, 32, 48);
            textures["piranha_plant"] = CreateColoredTexture(Color.DarkGreen, 32, 48);

            // Item placeholders
            textures["coin"] = CreateColoredTexture(Color.Gold, 32, 32);
            textures["mushroom"] = CreateColoredTexture(Color.Red, 32, 32);
            textures["fire_flower"] = CreateColoredTexture(Color.Orange, 32, 32);
            textures["1up_mushroom"] = CreateColoredTexture(Color.Green, 32, 32);
            textures["star"] = CreateColoredTexture(Color.Yellow, 32, 32);
            textures["fireball"] = CreateColoredTexture(Color.White, 16, 16);

            // Tile placeholders
            textures["ground"] = CreateColoredTexture(new Color(139, 69, 19), 32, 32);
            textures["brick"] = CreateColoredTexture(new Color(184, 92, 46), 32, 32);
            textures["question_block"] = CreateColoredTexture(Color.Yellow, 32, 32);
            textures["pipe"] = CreateColoredTexture(Color.Green, 32, 32);

            // Particle
            textures["particle"] = CreateColoredTexture(Color.White, 4, 4);

            Console.WriteLine("Created default placeholder textures");
        }

        // Create a solid colored texture
        public Texture2D CreateColoredTexture(Color color, int width, int height)
        {
            if (graphicsDevice == null)
            {
                Console.WriteLine("Cannot create texture: GraphicsDevice is null");
                return null;
            }

            Texture2D texture = new Texture2D(graphicsDevice, width, height);
            Color[] data = new Color[width * height];

            for (int i = 0; i < data.Length; i++)
                data[i] = color;

            texture.SetData(data);
            return texture;
        }

        // Create a texture with a border
        public Texture2D CreateBorderedTexture(Color fillColor, Color borderColor,
                                               int width, int height, int borderWidth = 2)
        {
            if (graphicsDevice == null) return null;

            Texture2D texture = new Texture2D(graphicsDevice, width, height);
            Color[] data = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;

                    // Border pixels
                    if (x < borderWidth || x >= width - borderWidth ||
                        y < borderWidth || y >= height - borderWidth)
                    {
                        data[index] = borderColor;
                    }
                    else
                    {
                        data[index] = fillColor;
                    }
                }
            }

            texture.SetData(data);
            return texture;
        }

        // Get default texture (white pixel)
        public Texture2D GetDefaultTexture()
        {
            return textures.ContainsKey("pixel") ? textures["pixel"] : null;
        }

        // Get default color for entity type
        private Color GetDefaultColor(string key)
        {
            switch (key.ToLower())
            {
                case "mario_small":
                case "mario_big":
                case "mario_fire":
                    return Color.Red;
                case "goomba":
                    return Color.Brown;
                case "koopa":
                    return Color.Green;
                case "piranha_plant":
                    return Color.DarkGreen;
                case "coin":
                    return Color.Gold;
                case "mushroom":
                    return Color.Red;
                case "fire_flower":
                    return Color.Orange;
                case "ground":
                    return new Color(139, 69, 19);
                case "brick":
                    return new Color(184, 92, 46);
                case "question_block":
                    return Color.Yellow;
                case "pipe":
                    return Color.Green;
                default:
                    return Color.Magenta; // Bright pink for missing textures
            }
        }

        // Assign textures to game objects after loading
        public void AssignTexturesToObjects()
        {
            // This will be called after all objects are created
            // to assign the loaded textures
        }

        // Clear all textures
        public void Unload()
        {
            foreach (var texture in textures.Values)
            {
                texture?.Dispose();
            }
            textures.Clear();
        }
    }

    // Extension methods to easily set textures on GameObjects
    public static class GameObjectTextureExtensions
    {
        public static void SetTexture(this Entities.Base.GameObject obj, string textureKey)
        {
            var texture = TextureManager.Instance.GetTexture(textureKey);
            if (texture != null)
            {
                // Use reflection to set private texture field
                var field = obj.GetType().BaseType?.GetField("texture",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);
                field?.SetValue(obj, texture);
            }
        }
    }
}