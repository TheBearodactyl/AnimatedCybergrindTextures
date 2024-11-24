using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace AnimatedCybergrindTextures.Utils
{
    public static class GenericUtils
    {
        public const string CustomGridKey = "cyberGrind.customGrid";
        public const string CustomSkyboxKey = "cyberGrind.customSkybox";
        
        private static readonly List<string> VideoExtensions = new List<string>
        {
            ".asf", ".avi", ".dv", ".m4v", ".mov", ".mp4", ".mpg", ".mpeg", ".ogv", ".vp8", ".webm", ".wmv"
        };
        
        public static VideoPlayer InitVideoPlayer(Transform parent)
        {
            var go = Object.Instantiate(new GameObject("AnimatedTexturesPlayer"), parent, true);
            var vp = go.AddComponent<VideoPlayer>();
            vp.isLooping = true;
            vp.SetDirectAudioMute(0, !Configuration.EnableAudio.Value);
            return vp;
        }
        
        public static bool HasVideoExtension(string key) =>
            VideoExtensions.Contains(new FileInfo(key).Extension.ToLower());

        public static string KeyToUrl(string key) => "file://" + new FileInfo(key).FullName;
    }
}
