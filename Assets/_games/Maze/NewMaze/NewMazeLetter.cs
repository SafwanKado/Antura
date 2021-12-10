using System.Collections;
using System.Collections.Generic;
using Antura.Core;
using Antura.LivingLetters;
using DG.DeExtensions;
using TMPro;
using UnityEngine;

public class NewMazeLetter : MonoBehaviour
{
    public TextMeshPro[] Texts;
    public GameObject DottedLine;
    public GameObject ContourLine;

    public float ArrowPlacementDelta = 1f;
    public GameObject ArrowPrefabGO;

    public float TutorialPointPlacementDelta = 1f;
    public GameObject TutorialPointPrefabGO;

    public void SetupLetter(LL_LetterData ld)
    {
        var shapeData = AppManager.I.AssetManager.GetShapeLetterData(ld.Id);
        if (shapeData == null) return;
        Debug.LogError("Found shape data: " + shapeData);
        ShapeManager.LoadSplinesOn(DottedLine, shapeData.Strokes);
        ShapeManager.LoadSplinesOn(ContourLine, shapeData.Contour);

        foreach (var textMeshPro in Texts)
        {
            textMeshPro.text = ld.TextForLivingLetter;
        }


        var arrowGos = ShapeManager.SpawnObjectsOnSplines(ArrowPrefabGO, transform, shapeData.Strokes, ArrowPlacementDelta, 0.1f, DottedLine.transform.localScale.x);
        for (var i = 0; i < arrowGos.Count; i++)
        {
            var arrowGo = arrowGos[i];
            arrowGo.transform.rotation = Quaternion.LookRotation(arrowGo.transform.forward, Vector3.up);
            //arrowGo.transform.localEulerAngles.SetY(-90f); // Force pitch
            arrowGo.transform.position += Vector3.up * 0.2f;  // Offset
        }

        var tutorialPointGos = ShapeManager.SpawnObjectsOnSplines(TutorialPointPrefabGO, transform, shapeData.Strokes, TutorialPointPlacementDelta, 0.1f, DottedLine.transform.localScale.x);
        for (var i = 0; i < tutorialPointGos.Count; i++)
        {
            var tutorialPointGo = tutorialPointGos[i];
            tutorialPointGo.transform.position += Vector3.up * 0.2f;  // Offset
        }

    }
}
