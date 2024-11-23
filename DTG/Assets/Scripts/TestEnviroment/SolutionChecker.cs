using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        yield return new WaitForSeconds(1);
        coll.enabled = false;
    }

    public void GiveSolution(TestObject.Solution test)
    {
        response.text = solution.Equals(test) ? GOOD_RESPONSE : BAD_RESPONSE;
        if (solution.Equals(test))
        {
            score++;
            response.text = GOOD_RESPONSE;
            uiScore.text = $"HighScore: 16 \nScore: {score}";
        }
        else
        {
            response.text = BAD_RESPONSE;
        }
        NextRequest();
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
