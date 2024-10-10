using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static AnimatedCybergrindTextures.Utils.GenericUtils;

namespace AnimatedCybergrindTextures.Components
{
    public class AsyncTexturePreviewLoader: MonoBehaviour
    {
        public Dictionary<string, Texture2D> OriginalCache;

        public void LoadPreview(string key, GameObject button)
        {
            StartCoroutine(HasVideoExtension(key)
                ? LoadVideoPreview(key, Callback)
                : LoadTexture(key, Callback));
            
            void Callback(Texture2D tex)
            {
                if (tex == null)
                    return;
                
                var sprite = Sprite.Create(
                    tex,
                    new Rect(0.0f, 0.0f, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f),
                    100f);
                sprite.texture.filterMode = FilterMode.Point;
                button.GetComponent<Image>().sprite = sprite;
            }
        }

        private IEnumerator LoadTexture(string key, Action<Texture2D> callback)
        {
            if (OriginalCache.TryGetValue(key, out var texture))
            {
                callback.Invoke(texture);
                yield break;
            }
            
            using (var uwr = UnityWebRequestTexture.GetTexture(KeyToUrl(key)))
            {
                yield return uwr.SendWebRequest();

                if (uwr.error != null)
                    Debug.Log(uwr.error);
                else
                {
                    var result = DownloadHandlerTexture.GetContent(uwr);
                    result.filterMode = FilterMode.Point;
                    OriginalCache[key] = result;
                    callback.Invoke(result);
                }
            }
        }
        
        private IEnumerator LoadVideoPreview(string key, Action<Texture2D> callback)
        {
            var timeout = Configuration.PreviewLoadingTimeout.Value;
            var previewPlayer = InitVideoPlayer(transform);
            if (previewPlayer.url != null)
                previewPlayer.Stop();
            previewPlayer.url = KeyToUrl(key);
            previewPlayer.Prepare();

            while (!previewPlayer.isPrepared)
            {
                yield return null;
                timeout -= Time.deltaTime;
                if (timeout <= 0)
                    break;
            }

            if (previewPlayer != null && !previewPlayer.isPrepared)
            {
                Destroy(previewPlayer.gameObject);
                yield break;
            }

            var playerTexture = previewPlayer.texture;
            var width = playerTexture.width;
            var height = playerTexture.height;

            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var renderTexture = new RenderTexture(width, height, 24);
            previewPlayer.targetTexture = renderTexture;
            previewPlayer.sendFrameReadyEvents = true;
            previewPlayer.frame = (int)previewPlayer.frameCount / 2;
            previewPlayer.Play();
            previewPlayer.Pause();
            previewPlayer.frameReady += (source, idx) =>
            {
                RenderTexture.active = renderTexture;
                texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                texture.Apply();
                RenderTexture.active = null;
                Destroy(previewPlayer.gameObject);
                callback.Invoke(texture);
            };
        }
    }
}