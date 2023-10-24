using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using Wartermelon.Utils;

namespace Wartermelon.Localization
{
    public static class ILocalizationExt
    {
        public static string GetString(this ILocalization localization, string key, string fallback = null)
        {
            if (localization == null)
            {
                if (fallback == null)
                {
                    return key;
                }
                return fallback;
            }

            var str = localization.GetString(key);
            if (str == null)
            {
                if (fallback == null)
                {
                    return key;
                }
                return fallback;
            }
            return str;
        }
    }

    public interface ILocalization
    {
        event PropertyChangedEventHandler<string> LocaleChanged;
        string Locale
        {
            get;
            set;
        }

        void LoadStringResources(string path);
        string GetString(string key);
    }

    [Serializable]
    public class StringResource
    {
        [XmlAttribute("id")]
        public string Id;
        [XmlAttribute("value")]
        public string Value;
    }

    [Serializable]
    public class StringResources
    {
        public StringResource[] Resources;
    }

    [DefaultExecutionOrder(-100)]
    public class LocalizationManager : MonoBehaviour, ILocalization
    {
        #region Singleton Pattern
        static private LocalizationManager instance = null;

        static public LocalizationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<LocalizationManager>();
                }

                return instance;
            }
        }
        #endregion

        private Dictionary<string, string> idToString;

        [SerializeField]
        private List<string> stringResources = null;

        [SerializeField]
        private string locale = "en-US";

        public event PropertyChangedEventHandler<string> LocaleChanged;
        public string Locale
        {
            get => locale;
            set
            {
                if (locale != value)
                {
                    var oldValue = locale;
                    locale = value;
                    LoadResources();
                    LocaleChanged?.Invoke(this, oldValue, value);
                }
            }
        }

        private void Awake()
        {
            LoadResources();
        }

        private void OnDestroy()
        {
        }

        private void LoadResources()
        {
            idToString = new Dictionary<string, string>();

            var prefix = "." + locale;
            foreach (var res in stringResources)
            {
                var textAsset = Resources.Load<TextAsset>(res + prefix);
                if (textAsset == null)
                {
                    Debug.LogFormat("String resource file {0} was not found", res + prefix);
                    continue;
                }

                LoadStringResources(textAsset);
            }
        }

        public void LoadStringResources(string path)
        {
            var prefix = "." + locale;
            var textAsset = Resources.Load<TextAsset>(path + prefix);
            if (textAsset == null)
            {
                Debug.LogFormat("String resource file {0} was not found", path + prefix);
                return;
            }

            LoadStringResources(textAsset);
        }

        private void LoadStringResources(TextAsset textAsset)
        {
            try
            {
                var stringResources = XmlUtility.FromXml<StringResources>(textAsset.text);
                foreach (var stringResource in stringResources.Resources)
                {
                    if (idToString.ContainsKey(stringResource.Id))
                    {
                        var exisiting = idToString[stringResource.Id];
                        Debug.LogWarning("Duplicate resource found " + stringResource.Id + " " + exisiting + ". Duplicate: " + stringResource.Value);
                        continue;
                    }

                    idToString.Add(stringResource.Id, stringResource.Value);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        public string GetString(string key)
        {
            if (key == null)
            {
                return null;
            }

            key = key.Trim();
            if (idToString.TryGetValue(key, out var str))
            {
                return str;
            }

            return null;
        }
    }

    public delegate void PropertyChangedEventHandler<T>(object sender, T oldValue, T newValue);
}
