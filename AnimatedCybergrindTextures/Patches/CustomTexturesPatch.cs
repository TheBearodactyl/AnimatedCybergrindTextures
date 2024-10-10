using System;
using System.Collections.Generic;
using System.IO;
using AnimatedCybergrindTextures.Components;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using static AnimatedCybergrindTextures.Utils.ReflectionUtils;
using static AnimatedCybergrindTextures.Utils.GenericUtils;
using Object = UnityEngine.Object;
// ReSharper disable InconsistentNaming

#pragma warning disable CS0649

namespace AnimatedCybergrindTextures.Patches
{
    [HarmonyPatch(typeof(CustomTextures))]
    public class CustomTexturesPatch
    {
        private const string AdditionalOptions =
            "/FirstRoom/Room/CyberGrindSettings/Canvas/CustomTextures/Panel/AdditionalOptions";
        
        private const string DisableSkyboxRotationKey = "cyberGrind.customSkybox.disableSpin";
        
        [ExternalAsset("Assets/Textures/animated_icon.png")]
        private static Texture2D _animatedIcon;
        
        [ExternalAsset("Assets/Elements/SkyboxToggle.prefab")]
        private static GameObject _skyboxToggleTemplate;
        
        [UltrakillAsset("b433c919446768d4a9e64494c6bfd084")]
        private static GameObject _shopClick;
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CustomTextures), "Start")]
        public static bool CustomTextures_Start_Prefix(
            CustomTextures __instance,
            Material[] ___gridMaterials,
            Material ___skyMaterial,
            OutdoorLightMaster ___olm,
            GameObject ___itemButtonTemplate,
            Dictionary<string, Texture2D> ___imageCache)
        {
            var grid = EndlessGrid.Instance.gameObject;
            if (grid.TryGetComponent(out AnimatedTexturesManager animatedTextures))
                return true;
            
            animatedTextures = grid.AddComponent<AnimatedTexturesManager>();
            var asyncTextureLoader = grid.AddComponent<AsyncTexturePreviewLoader>();
            animatedTextures.gridMaterials = ___gridMaterials;
            animatedTextures.skyboxMaterial = ___skyMaterial;
            animatedTextures.olm = ___olm;
            asyncTextureLoader.OriginalCache = ___imageCache;

            AddAnimatedIcon(___itemButtonTemplate);
            animatedTextures.skyboxRotationToggle = AddSkyboxRotationToggle(___olm);
            return true;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomTextures), "Start")]
        public static void CustomTextures_Start_Postfix(
            CustomTextures __instance,
            Material[] ___gridMaterials,
            Material ___skyMaterial)
        {
            var prefs = PrefsManager.Instance;
            var animatedTextures = AnimatedTextures();
            for (var i = 0; i < 3; i++)
            {
                var gridKey = prefs.GetStringLocal($"{CustomGridKey}_{i}");
                if (!string.IsNullOrEmpty(gridKey))
                    animatedTextures.SetGridTexture(gridKey, i);
            }
            
            var skyboxKey = prefs.GetStringLocal(CustomSkyboxKey);
            if (!string.IsNullOrEmpty(skyboxKey))
                animatedTextures.SetSkyboxTexture(skyboxKey);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CustomTextures), "BuildLeaf")]
        public static bool CustomTextures_BuildLeaf_Prefix(
            CustomTextures __instance,
            FileInfo file,
            GameObject ___itemButtonTemplate,
            Transform ___itemParent,
            ref Action __result)
        {
            var btn = Object.Instantiate(___itemButtonTemplate, ___itemParent, false);
            btn.GetComponent<Button>().onClick.RemoveAllListeners();
            btn.GetComponent<Button>().onClick.AddListener(() => __instance.SetTexture(file.FullName));
            btn.SetActive(true);
            
            if (!HasVideoExtension(file.FullName))
                foreach (Transform child in btn.transform)
                    child.gameObject.SetActive(false);
            
            if (AsyncTextureLoader() != null)
                AsyncTextureLoader().LoadPreview(file.FullName, btn);

            __result = () => Object.Destroy(btn);
            return false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CustomTextures), "SetTexture")]
        public static bool CustomTextures_SetTexture_Prefix(
            string key, 
            CustomTextures __instance,
            bool ___editBase,
            bool ___editTop,
            bool ___editTopRow)
        {
            if (!HasVideoExtension(key))
                return true;
            
            var editMode = GetPrivate(__instance, typeof(CustomTextures), "currentEditMode").ToString();
            AnimatedTextures().SetTexture(key, editMode, ___editBase, ___editTop, ___editTopRow);
            return false;
        }        
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CustomTextures), "SetEditMode")]
        public static void CustomTextures_SetEditMode_Postfix(int m, CustomTextures __instance)
        {
            AnimatedTextures().skyboxRotationToggle.SetActive(m == 2);
        }

        private static void AddAnimatedIcon(GameObject template)
        {
            var icon = new GameObject("Animated Icon")
            {
                transform =
                {
                    parent = template.transform
                }
            };
            var iconImage = icon.AddComponent<Image>();
            var iconTransform = icon.GetComponent<RectTransform>();
            
            var sprite = Sprite.Create(
                _animatedIcon,
                new Rect(0.0f, 0.0f, _animatedIcon.width, _animatedIcon.height), 
                new Vector2(0.5f, 0.5f), 100f);
            iconImage.sprite = sprite;
            icon.transform.localPosition = new Vector3(15, 20, 0);
            iconTransform.sizeDelta = new Vector2(32, 32);
            iconTransform.localScale = new Vector2(1, 1);
        }

        private static GameObject AddSkyboxRotationToggle(OutdoorLightMaster olm)
        {
            var go = Object.Instantiate(_skyboxToggleTemplate, GameObject.Find(AdditionalOptions).transform);
            go.AddComponent<HudOpenEffect>();
            go.SetActive(false);
            var dontRotate = PrefsManager.Instance.GetBoolLocal(DisableSkyboxRotationKey, true);
            olm.dontRotateSkybox = dontRotate;
            
            var toggle = go.GetComponentInChildren<Toggle>();
            var shopButton = go.gameObject.AddComponent<ShopButton>();
            shopButton.clickSound = _shopClick;
            
            toggle.isOn = olm.dontRotateSkybox;
            toggle.onValueChanged.AddListener(value =>
            {
                Object.Instantiate(_shopClick);
                PrefsManager.Instance.SetBoolLocal(DisableSkyboxRotationKey, value);
                olm.dontRotateSkybox = value;
            });
            return go;
        }

        private static AnimatedTexturesManager AnimatedTextures() =>
            EndlessGrid.Instance.gameObject.GetComponent<AnimatedTexturesManager>();        
        
        private static AsyncTexturePreviewLoader AsyncTextureLoader() =>
            EndlessGrid.Instance.gameObject.GetComponent<AsyncTexturePreviewLoader>();
    }
}