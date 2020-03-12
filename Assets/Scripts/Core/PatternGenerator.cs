using System;
using UnityEngine;

namespace Core
{
    public class PatternGenerator : MonoBehaviour
    {
        [SerializeField] private Transform patternPrefab;
        [SerializeField] private Vector2 patternOffset;
        [SerializeField] private Vector2 patternScale;
        [SerializeField] private PatternInstruction[] patternInstructions;

        private void Start()
        {
            GeneratePattern();
        }

        private void GeneratePattern()
        {
            //instantiate the prefab for every instruction
            foreach (var instruction in patternInstructions)
            {
                //create a new parent for the pattern, and set position to instruction startingOffset
                var patternParent = new GameObject();
                patternParent.transform.SetParent(transform, false);
                //instantiate the pattern as wide as the width of the instruction vector, adding the patternOffset between each instance
                for (var i = 0; i < instruction.widthHeight.x; i++)
                {
                    var newPattern = Instantiate(patternPrefab, patternParent.transform);
                    var newPatternLocalPosition = new Vector3(patternOffset.x * i, 0.0f);
                    newPattern.localPosition = newPatternLocalPosition;
                    newPattern.transform.localScale = patternScale;
                }

                //if the pattern is filled
                if (instruction.isFilled)
                {
                    //create a parent for the row, and parent the current patternParent
                    var fillParent = new GameObject();
                    fillParent.transform.SetParent(transform, false);
                    patternParent.transform.SetParent(fillParent.transform, false);
                    //instantiate the prefab/patternParent as tall as the height of the instruction vector
                    for (var i = 1; i < instruction.widthHeight.y; i++)
                    {
                        var newPattern = Instantiate(patternParent, fillParent.transform);
                        var newPatternLocalPosition = new Vector3(0.0f, patternOffset.y * i);
                        newPattern.transform.localPosition = newPatternLocalPosition;
                    }

                    ResetAlignment(instruction, fillParent.transform);
                }
                else
                {
                    //instantiate the prefab/patternParent as tall as the height of the instruction vector
                    for (var i = 1; i < instruction.widthHeight.y; i++)
                    {
                        var newPattern = Instantiate(patternPrefab, patternParent.transform);
                        var newPatternLocalPosition = new Vector3(0.0f, patternOffset.y * i);
                        newPattern.localPosition = newPatternLocalPosition;
                        newPattern.transform.localScale = patternScale;
                    }
                    
                    ResetAlignment(instruction, patternParent.transform);
                }
            }
        }
        
        private void ResetAlignment(PatternInstruction instruction, Transform patternParent)
        {
            var translateAmountX = (instruction.widthHeight.x + 1) * 0.5f * patternOffset.x;
            patternParent.Translate(-translateAmountX, 0.0f, 0.0f);
            var translateAmountY = (instruction.widthHeight.y + 1) * 0.5f * patternOffset.y;
            patternParent.Translate(0.0f, -translateAmountY, 0.0f);
            patternParent.parent.localPosition = instruction.startingOffset;
        }
    }
}

[Serializable]
public class PatternInstruction
{
    public Vector3 startingOffset;
    public Vector2 widthHeight;
    public bool isFilled;
}