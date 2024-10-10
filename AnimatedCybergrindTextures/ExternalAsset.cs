using System;

namespace AnimatedCybergrindTextures
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ExternalAsset : Attribute
    {
        public string Path { get; }

        public ExternalAsset(string path = "")
        {
            Path = path;
        }
    }
}