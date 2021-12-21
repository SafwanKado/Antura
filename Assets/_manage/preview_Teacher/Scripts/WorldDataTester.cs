﻿using System.Collections.Generic;
using System.Linq;
using Antura.Core;
using Antura.Database;
using Antura.Language;
using DG.DeInspektor.Attributes;
using UnityEngine;

namespace Antura.Teacher.Test
{
    /// <summary>
    /// Helper class to test DataBase contents related to the vocabulary data assigned to the world data
    /// </summary>
    public class WorldDataTester : MonoBehaviour
    {
        public string[] LetterGroups;

        private DatabaseManager _databaseManager;
        private List<PlaySessionData> _playSessionDatas;
        private List<LetterData> _letterDatas;
        private List<WordData> _wordDatas;

        void Awake()
        {
            _databaseManager = AppManager.I.DB;

            _playSessionDatas = _databaseManager.GetAllPlaySessionData();
            _letterDatas = _databaseManager.GetAllLetterData();
            _wordDatas = _databaseManager.GetAllWordData();
            //_phraseDatas = _databaseManager.GetAllPhraseData();
        }

        // Find all words that only have letters that appear in the Groups
        // Each Group can be used by the words in the subsequent Group
        [DeMethodButton("Words with only letters in Groups")]
        public void DoCheckWordsWithOnlyLetters()
        {
            string s;
            var query = "";
            var report = "";
            report += ($"Report - Analyzing {LetterGroups.Length} groups");
            List<LetterData> previousLetters = new List<LetterData>();
            List<WordData> previousWords = new List<WordData>();
            for (var iGroup = 0; iGroup < LetterGroups.Length; iGroup++)
            {
                var letterGroup = LetterGroups[iGroup];

                query += $" {letterGroup}";
                var desiredLettersParts = LanguageSwitcher.I.GetHelper(LanguageUse.Learning).SplitWord(AppManager.I.DB, new WordData { Text = query }, separateVariations: false);
                var desiredLetters = desiredLettersParts.ConvertAll(ld => AppManager.I.VocabularyHelper.ConvertToLetterWithForcedForm(ld.letter, LetterForm.Isolated));

                /*
                var str = $"Group {(iGroup+1)} letters:\n";
                foreach (var l in desiredLetters)
                {
                    if (previousLetters.Contains(l)) continue;
                    str += $"{l}\n";
                }
                report += "\n" + str;*/

                var correctWords = new List<WordData>();
                var wrongWords = new List<WordData>();
                var letterCount = new Dictionary<LetterData, int>();
                foreach (var wordData in _wordDatas)
                {
                    var lettersInWord = AppManager.I.VocabularyHelper.GetLettersInWord(wordData);
                    lettersInWord = lettersInWord.ConvertAll(ld => AppManager.I.VocabularyHelper.ConvertToLetterWithForcedForm(ld, LetterForm.Isolated));
                    bool isCorrect = true;
                    foreach (var letterInWord in lettersInWord)
                    {
                        if (desiredLetters.All(x => !x.IsSameLetterAs(letterInWord, LetterEqualityStrictness.Letter)))
                        {
                            //Debug.LogError($"Word {wordData.Id} has letter {letterInWord} we do not want.");
                            isCorrect = false;
                        }
                    }

                    if (isCorrect) correctWords.Add(wordData);
                    else wrongWords.Add(wordData);

                    if (isCorrect)
                    {
                        // Check letter count
                        foreach (var desiredLetter in desiredLetters)
                        {
                            if (lettersInWord.Any(x => x.IsSameLetterAs(desiredLetter, LetterEqualityStrictness.Letter)))
                            {
                                //Debug.LogError($"Word {wordData.Id} has letter {desiredLetter} we want.");
                                if (!letterCount.ContainsKey(desiredLetter)) letterCount[desiredLetter] = 0;
                                letterCount[desiredLetter]++;
                            }
                        }
                    }
                }

                s = $"Group {(iGroup+1)} Letters:\n";
                foreach (var desiredLetter in desiredLetters)
                {
                    if (previousLetters.Contains(desiredLetter)) continue;
                    s += $"{desiredLetter.Id} ({desiredLetter.Isolated}) ({(letterCount.ContainsKey(desiredLetter) ? letterCount[desiredLetter].ToString() : "0")})\n";

                    if (!letterCount.ContainsKey(desiredLetter))
                    {
                        Debug.LogError($"Letter <b>{desiredLetter}</b> found ZERO times");
                    }
                    else if (letterCount[desiredLetter] < 3)
                    {
                        Debug.LogError($"Letter <b>{desiredLetter}</b> found only {letterCount[desiredLetter]} times");
                    }
                }
                report += "\n" + s;

                int totCorrectWords = correctWords.Count;
                correctWords = correctWords.Where(x => !previousWords.Contains(x)).ToList();
                s = $"Words for group {iGroup+1}: <b>{correctWords.Count}</b> (tot {totCorrectWords}/{_wordDatas.Count}):\n";
                foreach (var correctWord in correctWords)
                {
                    s += $"{correctWord.Id} ({correctWord.Text})\n";
                }
                report += "\n" + s;

                previousLetters.AddRange(desiredLetters);
                previousWords.AddRange(correctWords);
            }

            s = "\nUNUSED WORDS:\n";
            foreach (var word in AppManager.I.VocabularyHelper.GetAllWords(new WordFilters()))
            {
                if (previousWords.Contains(word)) continue;
                s += $"{word.Id} ({word.Text})\n";
            }
            report += "\n" + s;

            s = "\nUNUSED LETTERS:\n";
            foreach (var letter in AppManager.I.VocabularyHelper.GetAllLetters(new LetterFilters()))
            {
                if (previousLetters.Contains(letter)) continue;
                s += $"{letter.Id} ({letter.Isolated})\n";
            }
            report += "\n" + s;
            Debug.Log(report);
        }

    }
}