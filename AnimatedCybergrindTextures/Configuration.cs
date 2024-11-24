using System.IO;
using BepInEx;
using BepInEx.Configuration;

namespace AnimatedCybergrindTextures
{
    public static class Configuration
    {
        private static readonly ConfigFile Config = new ConfigFile(Path.Combine(Paths.ConfigPath, "AnimatedCybergrindTextures", "config.cfg"), true);
        
        // Video player
        public static readonly ConfigEntry<float> PreviewLoadingTimeout = Config.Bind("VideoPlayer", 
            "PreviewTimeout",
            5.0f,
            "Timeout for a video preview loading attempt in seconds");
        
        // Skybox
        public static readonly ConfigEntry<int> SkyboxWidth = Config.Bind("Skybox", 
            "Width",
            1920,
            "Width of a skybox texture");
        
        public static readonly ConfigEntry<int> SkyboxHeight = Config.Bind("Skybox", 
            "Height",
            960,
            "Height of a skybox texture");
        
        public static readonly ConfigEntry<int> SkyboxColorDepth = Config.Bind("Skybox", 
            "Depth",
            24,
            "Color depth used by skybox video player");
        
        // Tiles
        public static readonly ConfigEntry<int> TileSize = Config.Bind("Tiles", 
            "Size",
            100,
            "Width and height of a texture of a single tile on the grid");

        public static readonly ConfigEntry<int> TileColorDepth = Config.Bind("Tiles", 
            "Depth",
            24,
            "Color depth used by tiles video player");

        public static readonly ConfigEntry<bool> EnableAudio = Config.Bind("VideoPlayer",
                "EnableAudio",
                false,
                "Enable audio from video files");
    }
}
