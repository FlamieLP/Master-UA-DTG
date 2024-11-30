using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SolutionChecker : MonoBehaviour
{
    private readonly String GOOD_RESPONSE = "Gut Gemacht!";
    private readonly String BAD_RESPONSE = "Das war leider falsch!";
    
    [SerializeField] private TestObject.Solution solution;
    [SerializeField] private TMP_Text request;
    [SerializeField] private TMP_Text response;
    
    [SerializeField] private TMP_Text uiScore;
    [SerializeField] private int score = 0;
    
    
    [SerializeField] private Collider coll;
    
    private bool isResponding = false;

    public void Start()
    {
        NextRequest();
    }

    public void Evaluate()
    {
        coll.enabled = true;
        StartCoroutine(nameof(WaitAndReset));
    }
    
    private IEnumerator WaitAndReset()
    {
        var oldScore = score;
        
        yield return new WaitForSeconds(1);
        coll.enabled = false;

        if (oldScore == score)
        {
            StartCoroutine(DisplayAnswer("Bitte lege ein Objekt rechts in den blauen Bereich.", 0, 3));
        }
    }
    
    private IEnumerator DisplayAnswer(String answer, float debounce = 0.5f, float duration = 5f, [CanBeNull] Action action = null)
    {
        if (isResponding) yield break;
        isResponding = true;
        
        yield return new WaitForSeconds(debounce);
        response.text = answer;
        yield return new WaitForSeconds(duration);
        response.text = "";
        action?.Invoke();
        
        isResponding = false;
    }
    
    private IEnumerator DisplayNextRequest()
    {
        yield return new WaitForSeconds(0.2f);
        request.text = "";
        yield return new WaitForSeconds(0.2f);
        request.text = ".";
        yield return new WaitForSeconds(0.2f);
        request.text = "..";
        yield return new WaitForSeconds(0.2f);
        request.text = "...";
        yield return new WaitForSeconds(0.2f);
        request.text = ".";
        yield return new WaitForSeconds(0.2f);
        request.text = "..";
        yield return new WaitForSeconds(0.2f);
        request.text = "...";
        yield return new WaitForSeconds(0.5f);
        NextRequest();
    }

    public void GiveSolution(TestObject.Solution test)
    {
        String text = solution.Equals(test) ? GOOD_RESPONSE : BAD_RESPONSE;
        if (solution.Equals(test))
        {
            score++;
            uiScore.text = $"HighScore: 16 \nScore: {score}";
        }
        StartCoroutine(DisplayAnswer(text, action: () => StartCoroutine(DisplayNextRequest())));
    }

    private void NextRequest()
    {
        coll.enabled = false;
        TestObject.SColor color =
            (TestObject.SColor)Random.Range(0, Enum.GetValues(typeof(TestObject.SColor)).Length);
        TestObject.Shape shape =
            (TestObject.Shape)Random.Range(0, Enum.GetValues(typeof(TestObject.Shape)).Length);

        solution = new TestObject.Solution(color, shape);

        request.text = $"Ich hätte gerne einen {solution.GetColorText()} {solution.GetShapeText()}, bitte!";
    }
}
