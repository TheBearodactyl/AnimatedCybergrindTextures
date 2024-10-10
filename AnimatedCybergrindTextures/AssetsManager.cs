using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AnimatedCybergrindTextures
{
    [ConfigureSingleton(SingletonFlags.HideAutoInstance)]
    public class AssetsManager : MonoSingleton<AssetsManager>
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        private static readonly Dictionary<string, Object> Prefabs = new Dictionary<string, Object>();
        private AssetBundle _bundle;

        public void LoadAssets()
        {
            _bundle = AssetBundle.LoadFromMemory(Resources.AnimatedCybergrindTextures);
        }

        public void RegisterAssets()
        {
            foreach (var assetName in _bundle.GetAllAssetNames())
                Prefabs.Add(assetName, _bundle.LoadAsset<Object>(assetName));

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                CheckType(type);
        }
        
        private static void CheckType(IReflect type)
        {
            type.GetFields(Flags)
                .ToList()
                .ForEach(ProcessField);
        }

        private static void ProcessField(FieldInfo field)
        {
            if (field.FieldType.IsArray || !field.IsStatic)
                return;

            var externalAssetTag = field.GetCustomAttribute<ExternalAsset>();
            var assetTag = field.GetCustomAttribute<UltrakillAsset>();

            if (externalAssetTag != null)
                field.SetValue(null, Prefabs[externalAssetTag.Path]);
            else if (assetTag != null)
            {
                field.SetValue(null, Addressables
                    .LoadAssetAsync<GameObject>(assetTag.Path)
                    .WaitForCompletion());
            }
        }
    }
}