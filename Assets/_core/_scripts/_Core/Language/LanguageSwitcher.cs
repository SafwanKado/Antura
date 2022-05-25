using Antura.Core;
using Antura.Audio;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Antura.Language
{
    public class LanguageSwitcher
    {
        public static LanguageSwitcher I
        {
            get
            {
                if (AppManager.I == null)
                    return null;
                return AppManager.I.LanguageSwitcher;
            }
        }

        public class LanguageData
        {
            public LangConfig config;
            public ILanguageHelper helper;
            public DiacriticsComboData diacriticsComboData;
        }

        private Dictionary<LanguageUse, LanguageCode> useMapping;
        private Dictionary<LanguageCode, LanguageData> loadedLanguageData;

        public LanguageSwitcher()
        {
            useMapping = new Dictionary<LanguageUse, LanguageCode>();
            loadedLanguageData = new Dictionary<LanguageCode, LanguageData>();
        }

        public IEnumerator LoadEditionData()
        {
            yield return LoadLanguage(LanguageUse.Learning, AppManager.I.ContentEdition.LearningLanguage);
            yield return ReloadNativeLanguage();
            yield return LoadLanguage(LanguageUse.Help, AppManager.I.ContentEdition.HelpLanguage);
        }

        public IEnumerator LoadAllLanguageData()
        {
            // We also need to load data for all languages, as they are needed for the selection menu
            foreach (var nativeLanguage in AppManager.I.AppEdition.SupportedNativeLanguages)
            {
                yield return LoadLanguageData(nativeLanguage);
            }

            foreach (var contentEdition in AppManager.I.AppEdition.ContentEditions)
            {
                yield return LoadLanguageData(contentEdition.LearningLanguage);
                yield return LoadLanguageData(contentEdition.HelpLanguage);
                foreach (LanguageCode nativeLanguage in contentEdition.OverridenNativeLanguages)
                {
                    yield return LoadLanguageData(nativeLanguage);
                }
            }

        }

        public IEnumerator ReloadNativeLanguage()
        {
            yield return LoadLanguage(LanguageUse.Native, AppManager.I.AppSettings.NativeLanguage);
        }

        private IEnumerator LoadLanguage(LanguageUse use, LanguageCode language)
        {
            useMapping[use] = language;
            yield return LoadLanguageData(language);
        }

        IEnumerator LoadLanguageData(LanguageCode language)
        {
            if (loadedLanguageData.ContainsKey(language))
                yield break;
            var languageData = new LanguageData();

            yield return AssetLoader.Load<LangConfig>($"{language}/LangConfig", r => languageData.config = r, DebugConfig.I.AddressablesBlockingLoad);
            if (languageData.config == null)
            {
                throw new FileNotFoundException($"Could not find the LangConfig file for {language} in the language resources! Did you setup it correctly?");
            }

            yield return AssetLoader.Load<AbstractLanguageHelper>($"{language}/LanguageHelper", r => languageData.helper = r, DebugConfig.I.AddressablesBlockingLoad);
            if (languageData.helper == null)
            {
                throw new FileNotFoundException($"Could not find the LanguageHelper file in the language resources! Did you setup the {language} language correctly?");
            }
            loadedLanguageData[language] = languageData;

            yield return AssetLoader.Load<DiacriticsComboData>($"{language}/DiacriticsComboData", r => languageData.diacriticsComboData = r, DebugConfig.I.AddressablesBlockingLoad);
            /*if (languageData.diacriticsComboData == null)
            {
                throw new FileNotFoundException($"Could not find the DiacriticsComboData file for {language} in the language resources! Did you setup it correctly?");
            }*/
        }

        public IEnumerator PreloadLocalizedDataCO()
        {
            yield return AudioManager.I.PreloadDataCO();
            yield return AppManager.I.AssetManager.PreloadDataCO();
        }

        public ILanguageHelper GetHelper(LanguageUse use)
        {
            return loadedLanguageData[useMapping[use]].helper;
        }

        public ILanguageHelper GetHelper(LanguageCode code)
        {
            return loadedLanguageData[code].helper;
        }
        /*public DatabaseManager GetDBManager(LanguageUse use)
        {
            return loadedLanguageData[useMapping[use]].dbManager;
        }*/

        public DiacriticsComboData GetDiacriticsComboData(LanguageUse use)
        {
            return loadedLanguageData[useMapping[use]].diacriticsComboData;
        }

        public LangConfig GetLangConfig(LanguageUse use)
        {
            return loadedLanguageData[useMapping[use]].config;
        }

        public LangConfig GetLangConfig(LanguageCode code)
        {
            return loadedLanguageData[code].config;
        }

        #region Shortcuts

        public bool IsLearningLanguageRTL()
        {
            return GetLangConfig(LanguageUse.Learning).IsRightToLeft();
        }

        public static bool LearningRTL => LearningConfig.IsRightToLeft();
        public static LangConfig LearningConfig => I.GetLangConfig(LanguageUse.Learning);
        public static ILanguageHelper LearningHelper => I.GetHelper(LanguageUse.Learning);

        #endregion

    }
}
