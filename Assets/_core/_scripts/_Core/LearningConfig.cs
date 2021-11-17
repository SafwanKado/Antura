﻿using Antura.Database;
using Antura.Language;
using UnityEngine;

namespace Antura.Core
{
    [CreateAssetMenu]
    public class LearningConfig : ScriptableObject
    {
        public Sprite Icon;
        public string Title;

        [Header("Language")]
        public LanguageCode LearningLanguage;
        public LanguageCode NativeLanguage;
        public LanguageCode HelpLanguage;
        public LanguageCode[] SupportedNativeLanguages;
        [Tooltip("try to set the native language to the device language, otherwise use NativeLanguage")]
        public bool DetectSystemLanguage;

        public string GetLearningLangResourcePrefix()
        {
            return $"{LearningLanguage}/";
        }

        [Header("Data - Vocabulary")]
        public LetterDatabase LetterDB;
        public WordDatabase WordDB;
        public PhraseDatabase PhraseDB;
        public LocalizationDatabase LocalizationDB;

        [Header("Data - Journey")]
        public StageDatabase StageDB;
        public LearningBlockDatabase LearningBlockDB;
        public PlaySessionDatabase PlaySessionDB;
        public MiniGameDatabase MiniGameDB;
        public RewardDatabase RewardDB;

        [Header("In-Game Resources")]
        public Sprite HomeLogo;
        public Sprite TransitionLogo;
        public GameObject Flag3D;

        public GameObject GetResource(EditionResourceID id)
        {
            switch (id) {
                case EditionResourceID.Flag: return Flag3D;
            }
            return null;
        }


    }
}