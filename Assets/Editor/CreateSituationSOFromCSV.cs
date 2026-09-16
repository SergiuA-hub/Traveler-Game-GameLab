#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

public static class CreateSituationSOFromCSV
{
    private const string CSV_PATH = "Assets/Luchi/TraderData/Situations.csv";
    private const string OUTPUT_FOLDER = "Assets/Resources/Situations";

    [MenuItem("Tools/SeasonOfTrade/Create Situations From CSV")]
    public static void CreateSituations()
    {
        if (!File.Exists(CSV_PATH))
        {
            Debug.LogError($"Situations CSV not found at: {CSV_PATH}");
            return;
        }

        if (!AssetDatabase.IsValidFolder(OUTPUT_FOLDER))
        {
            Debug.LogError($"Output folder does not exist: {OUTPUT_FOLDER}");
            return;
        }

        string[] lines = File.ReadAllLines(CSV_PATH);

        if (lines.Length <= 1)
        {
            Debug.LogWarning("Situations CSV contains no data.");
            return;
        }

        int processed = 0;
        int failed = 0;

        // Start from 1 because row 0 is the header
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line))
                continue;

            List<string> columns = ParseCsvLine(line);

            if (columns.Count < 10)
            {
                Debug.LogError(
                    $"Invalid row {i + 1}. Expected 10 columns but found {columns.Count}."
                );

                failed++;
                continue;
            }

            string situationNumber = columns[0].Trim();
            string phaseText = columns[1].Trim();
            string nextPhaseText = columns[2].Trim();
            string situationName = columns[3].Trim();
            string act = columns[4].Trim();
            string situationDescription = columns[5].Trim();

            string requiredContextText = columns[6].Trim();
            string givenContextText = columns[7].Trim();
            string removedContextText = columns[8].Trim();

            string outcomeText = columns[9].Trim();

            // PHASE
            if (!Enum.TryParse(
                    phaseText,
                    true,
                    out Phase phase))
            {
                Debug.LogError(
                    $"Invalid Phase '{phaseText}' at row {i + 1}."
                );

                failed++;
                continue;
            }

            // NEXT PHASE
            if (!Enum.TryParse(
                    nextPhaseText,
                    true,
                    out Phase nextPhase))
            {
                Debug.LogError(
                    $"Invalid Next Phase '{nextPhaseText}' at row {i + 1}."
                );

                failed++;
                continue;
            }

            // OUTCOME
            EventOutcome outcome = EventOutcome.NONE;

            if (!string.IsNullOrWhiteSpace(outcomeText) &&
                outcomeText != "-")
            {
                if (!Enum.TryParse(
                        outcomeText,
                        true,
                        out outcome))
                {
                    Debug.LogError(
                        $"Invalid Outcome '{outcomeText}' at row {i + 1}. " +
                        $"Add it to the EventOutcome enum."
                    );

                    failed++;
                    continue;
                }
            }

            Situation situation = ScriptableObject.CreateInstance<Situation>();

            situation.phase = phase;
            situation.nextPhase = nextPhase;

            situation.situationName = situationName;
            situation.act = act;
            situation.situationDescription = situationDescription;

            situation.RequiredContexts =
                ParseContexts(requiredContextText, i + 1);

            situation.GivenContexts =
                ParseContexts(givenContextText, i + 1);

            situation.RemovedContexts =
                ParseContexts(removedContextText, i + 1);

            situation.outcome = outcome;

            string safeName = MakeSafeFileName(situationName);

            string assetPath =
                $"{OUTPUT_FOLDER}/{situationNumber}_{safeName}.asset";

            // If the asset already exists, replace its data instead
            Situation existing =
                AssetDatabase.LoadAssetAtPath<Situation>(assetPath);

            if (existing != null)
            {
                EditorUtility.CopySerialized(situation, existing);

                UnityEngine.Object.DestroyImmediate(situation);

                EditorUtility.SetDirty(existing);

                Debug.Log($"Updated Situation: {assetPath}");
            }
            else
            {
                AssetDatabase.CreateAsset(situation, assetPath);

                Debug.Log($"Created Situation: {assetPath}");
            }

            processed++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            $"Situation import finished. Processed: {processed}, Failed: {failed}"
        );
    }

    private static List<Context> ParseContexts(
        string text,
        int rowNumber)
    {
        List<Context> result = new List<Context>();

        if (string.IsNullOrWhiteSpace(text) || text == "-")
            return result;

        string[] values = text.Split(',');

        foreach (string value in values)
        {
            string contextText = value.Trim();

            if (string.IsNullOrEmpty(contextText))
                continue;

            if (Enum.TryParse(
                    contextText,
                    true,
                    out Context context))
            {
                if (!result.Contains(context))
                {
                    result.Add(context);
                }
            }
            else
            {
                Debug.LogError(
                    $"Unknown Context '{contextText}' at CSV row {rowNumber}. " +
                    $"Add it to the Context enum."
                );
            }
        }

        return result;
    }

    private static string MakeSafeFileName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }

        return name.Replace(" ", "_");
    }

    private static List<string> ParseCsvLine(string line)
    {
        List<string> result = new List<string>();

        bool insideQuotes = false;
        string current = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (insideQuotes &&
                    i + 1 < line.Length &&
                    line[i + 1] == '"')
                {
                    current += '"';
                    i++;
                }
                else
                {
                    insideQuotes = !insideQuotes;
                }
            }
            else if (c == ',' && !insideQuotes)
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }

        result.Add(current);

        return result;
    }
}

#endif