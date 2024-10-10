using System;

namespace AnimatedCybergrindTextures
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UltrakillAsset : Attribute
    {
        public string Path { get; }

        public UltrakillAsset(string path = "")
        {
            Path = path;
        }
    }
}