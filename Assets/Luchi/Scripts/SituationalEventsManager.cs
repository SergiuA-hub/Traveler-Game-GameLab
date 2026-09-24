using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SituationalEventsManager : MonoBehaviour
{
    public static List<Situation> situations = new List<Situation>();

    private void Awake()
    {
        situations = new List<Situation>(
            Resources.LoadAll<Situation>("Situations")
        );

        Debug.Log($"Loaded {situations.Count} situations.");
    }

    public Situation pickStartSituation()
    {
        if(situations == null || situations.Count == 0)
        {
            Debug.LogError("Situations list is empty or not initialized.");
            return null;
        }

        List<Situation> startSituations = situations.FindAll(
            s => s.phase == Phase.Start
        );
        if (startSituations.Count == 0)
        {
            Debug.LogError("No start situations found.");
            return null;
        }
        int randomIndex = Random.Range(0, startSituations.Count);
        return startSituations[randomIndex];
    }

    //public List<Situation> getSituationsByPhase(Phase phase, List<Context> currentContexts)
    //{
    //    return situations
    //    .Where(s =>
    //        s.phase == phase &&
    //        s.RequiredContexts.All(
    //            required => currentContexts.Contains(required)
    //        )
    //    )
    //    .OrderBy(x => Random.value)
    //    .ToList();
    //}
    public List<Situation> getSituationsByPhase(Phase phase, List<Context> currentContexts)
    {
        var candidates = situations
            .Where(s =>
                s.phase == phase &&
                s.RequiredContexts.All(
                    required => currentContexts.Contains(required)
                )
            )
            .OrderBy(x => Random.value)
            .ToList();

        foreach (Situation situation in candidates)
        {
            if (phase != Phase.Start &&
                situation.RequiredContexts.Count == 0)
            {
                Debug.LogWarning(
                    $"Situation '{situation.situationName}' " +
                    $"has NO required contexts and therefore matches EVERY story."
                );
            }
        }

        return candidates;
    }
}
