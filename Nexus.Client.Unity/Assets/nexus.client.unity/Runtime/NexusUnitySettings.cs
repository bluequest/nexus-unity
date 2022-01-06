using System.IO;
using UnityEngine;

namespace Nexus.Client.Unity
{
    /// <summary>
    /// Stores Nexus.gg API settings as a ScriptableObject for easy editing in the Inspector.
    /// </summary>
    public sealed class NexusUnitySettings : ScriptableObject, INexusSettings
    {
        private const string NexusSettingsName = "NexusSettings";

        /// <remarks>
        /// Using singleton pattern and unity-resources to provide easy access to settings for devs. 
        /// </remarks>
        private static NexusUnitySettings instance;

        [Tooltip("Used to identify the client when making requests to the Nexus API.")] [SerializeField]
        private string sharedSecret = null;
        
        public string SharedSecret => this.sharedSecret;

        /// <summary>Get Nexus's settings.</summary>
        /// <remarks>
        /// Retrieves the settings from Resources.
        /// </remarks>
        public static NexusUnitySettings GetSettings()
        {
            if (NexusUnitySettings.instance == null)
            {
                NexusUnitySettings.instance = Resources.Load<NexusUnitySettings>(NexusUnitySettings.NexusSettingsName);
            }

            return NexusUnitySettings.instance;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Adds "Nexus/Create Settings" to Unity's menu bar.
        /// </summary>
        /// <remarks>
        /// Creates a new NexusUnitySetting's object under {project}/Assets/Resources if no setting file currently exists,
        /// then pings the existing or newly created settings file in the Project window.
        /// </remarks>
        [UnityEditor.MenuItem("Nexus/Create Settings")]
        private static void CreateSettings()
        {
            // try to load settings from resources, only create a new one if not found
            NexusUnitySettings settings = Resources.Load<NexusUnitySettings>(NexusUnitySettings.NexusSettingsName);
            if (settings == null)
            {
                // no settings found, create
                settings = NexusUnitySettings.CreateInstance<NexusUnitySettings>();

                // settings must live under /Resources to ensure that 1) it is shipped as part of the game's build and
                // 2) that it can be loaded without a direct asset reference by this class.
                string path = Path.Combine("Assets", "Resources");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                settings.name = NexusUnitySettings.NexusSettingsName;

                // save the settings to the asset library
                string assetPath = string.Format("{0}/{1}.asset", path, settings.name);
                UnityEditor.AssetDatabase.CreateAsset(settings, assetPath);
            }
            
            // highlight the settings in the project window
            UnityEditor.EditorGUIUtility.PingObject(settings);
        }
#endif
    }
}