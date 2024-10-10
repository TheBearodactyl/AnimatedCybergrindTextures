using UnityEngine;
using UnityEngine.Video;
using static AnimatedCybergrindTextures.Utils.GenericUtils;
using static AnimatedCybergrindTextures.Configuration;

namespace AnimatedCybergrindTextures.Components
{
    public class AnimatedTexturesManager: MonoBehaviour
    {
        public Material[] gridMaterials;
        public RenderTexture[] gridTextures;
        public Material skyboxMaterial;
        public RenderTexture skyboxTexture;
        public VideoPlayer[] gridPlayers;
        public VideoPlayer skyboxPlayer;

        public OutdoorLightMaster olm;
        public GameObject skyboxRotationToggle;

        private void Awake()
        {
            gridTextures = new RenderTexture[3];
            gridPlayers = new VideoPlayer[3];
            skyboxTexture = new RenderTexture(SkyboxWidth.Value, SkyboxHeight.Value, SkyboxColorDepth.Value);
            skyboxPlayer = InitVideoPlayer(transform);
            skyboxPlayer.targetTexture = skyboxTexture;

            for (var i = 0; i < 3; i++)
            {
                gridTextures[i] = new RenderTexture(TileSize.Value, TileSize.Value, TileColorDepth.Value);
                gridPlayers[i] = InitVideoPlayer(transform);
                gridPlayers[i].targetTexture = gridTextures[i];
            }
        }
        
        public void SetTexture(string key, string editMode, bool editBase, bool editTop, bool editTopRow)
        {
            if (!HasVideoExtension(key))
                return;

            switch (editMode)
            {
                case "Grid":
                {
                    for (var i = 0; i < gridMaterials.Length; i++)
                    {
                        if (gridPlayers[i].url != null)
                            gridPlayers[i].Stop();

                        if (!(!editBase && i == 0
                              || !editTop && i == 1
                              || !editTopRow && i == 2))
                        {
                            PrefsManager.Instance.SetStringLocal($"{CustomGridKey}_{i}", key);
                            gridPlayers[i].url = KeyToUrl(key);
                            gridPlayers[i].Prepare();
                            gridPlayers[i].frame = 0;
                            gridMaterials[i].mainTexture = gridTextures[i];
                        }
                
                        if (gridPlayers[i].url != null)
                            gridPlayers[i].Play();
                    }
                    break;
                }
                case "Skybox":
                {
                    SetSkyboxTexture(key);
                    break;
                }
            }
        }

        public void SetGridTexture(string key, int index)
        {
            if (!HasVideoExtension(key))
                return;

            if (gridPlayers[index].url != null)
                gridPlayers[index].Stop();
            
            gridPlayers[index].url = KeyToUrl(key);
            gridPlayers[index].Prepare();
            gridPlayers[index].frame = 0;
            gridMaterials[index].mainTexture = gridTextures[index];
            
            if (gridPlayers[index].url != null)
                gridPlayers[index].Play();
        }        
        
        public void SetSkyboxTexture(string key)
        {
            if (!HasVideoExtension(key))
                return;
            
            if (skyboxPlayer != null)
                skyboxPlayer.Stop();
            
            skyboxPlayer.url = KeyToUrl(key);
            skyboxPlayer.Prepare();
            skyboxPlayer.frame = 0;
            skyboxMaterial.mainTexture = skyboxTexture;
            skyboxPlayer.Play();
            olm.UpdateSkyboxMaterial();
            PrefsManager.Instance.SetStringLocal(CustomSkyboxKey, key);
        }
    }
}