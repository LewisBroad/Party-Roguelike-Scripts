using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    public int GlobalDifficulty => Mathf.FloorToInt(globalTimer / 60);
    public Dictionary<string, int> PlanetVisitCounts = new Dictionary<string, int>();

    private float globalTimer;
    private float currentPlanetTimer;
    private string currentPlanet;

    public AnimationCurve difficultyCurve;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        globalTimer += Time.deltaTime;
        currentPlanetTimer += Time.deltaTime;
    }

    public void OnPlanetEnter(string planetName)
    {
        currentPlanet = planetName;
        currentPlanetTimer = 0f;

        if (!PlanetVisitCounts.ContainsKey(planetName))
            PlanetVisitCounts[planetName] = 0;

        PlanetVisitCounts[planetName]++;
    }

    public float GetCurrentPlanetTimer() => currentPlanetTimer;

    public int GetPlanetVisitCount(string planetName)
    {
        return PlanetVisitCounts.TryGetValue(planetName, out int count) ? count : 0;
    }

    public float GetDifficultyModifier()
    {
        return difficultyCurve.Evaluate(GlobalDifficulty / 10f);
    }

    public int GetPlanetDifficulty(string planetName)
    {
        int baseDifficulty = GlobalDifficulty;
        int visitPenalty = GetPlanetVisitCount(planetName);
        return baseDifficulty + visitPenalty;
    }

    public float GetScaledDifficulty()
    {
        return GlobalDifficulty * GetDifficultyModifier();
    }
}
