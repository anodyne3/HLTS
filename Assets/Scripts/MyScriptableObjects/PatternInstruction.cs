using UnityEngine;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "Pattern", menuName = "MyAssets/PatternInstructions", order = 50)]
    public class PatternInstruction : ScriptableObject
    {
        public Transform patternPrefab;
        public float patternScale;
        public int sortingOrder;
        public PatternDetails[] patternInstructions;

        private Vector2 _patternTileOffset;

        public void GeneratePattern(Transform parent)
        {
            var spriteRenderer = (SpriteRenderer) patternPrefab.GetComponent(typeof(SpriteRenderer));
            _patternTileOffset = spriteRenderer.bounds.size;
            spriteRenderer.sortingOrder = sortingOrder;

            foreach (var instruction in patternInstructions)
            {
                //create a new parent for the pattern, and parent it to the generator 
                var patternParent = new GameObject();
                patternParent.transform.SetParent(parent, false);
                //instantiate the pattern as wide as the width of the instruction vector, adding the patternOffset between each instance
                for (var i = 0; i < instruction.widthHeight.x; i++)
                {
                    var newPattern = Instantiate(patternPrefab, patternParent.transform);
                    var newPatternLocalPosition = new Vector3(_patternTileOffset.x * i, 0.0f);
                    newPattern.localPosition = newPatternLocalPosition;
                }

                //if the pattern is filled
                if (instruction.isGrid)
                {
                    //create a parent for the row, and parent the current patternParent
                    var fillParent = new GameObject();
                    fillParent.transform.SetParent(parent, false);
                    patternParent.transform.SetParent(fillParent.transform, false);
                    //instantiate the patternParent as tall as the height of the instruction vector
                    for (var i = 1; i < instruction.widthHeight.y; i++)
                    {
                        var newPattern = Instantiate(patternParent, fillParent.transform);
                        var newPatternLocalPosition = new Vector3(0.0f, _patternTileOffset.y * i);
                        newPattern.transform.localPosition = newPatternLocalPosition;
                    }

                    AlignAndScale(instruction, fillParent.transform);
                }
                else
                {
                    //instantiate the prefab as tall as the height of the instruction vector
                    for (var i = 1; i < instruction.widthHeight.y; i++)
                    {
                        var newPattern = Instantiate(patternPrefab, patternParent.transform);
                        var newPatternLocalPosition = new Vector3(0.0f, _patternTileOffset.y * i);
                        newPattern.localPosition = newPatternLocalPosition;
                    }
                    //recenter or move the parent to the instruction starting offset, and scale the object
                    AlignAndScale(instruction, patternParent.transform);
                }
            }
        }

        private void AlignAndScale(PatternDetails instruction, Transform patternParent)
        {
            patternParent.transform.localScale = new Vector2(patternScale, patternScale);

            if (instruction.isCentered)
            {
                var translateAmountX = instruction.widthHeight.x * 0.5f * _patternTileOffset.x;
                var translateAmountY = instruction.widthHeight.y * 0.5f * _patternTileOffset.y;
                patternParent.localPosition =
                    new Vector2(-translateAmountX * patternScale, -translateAmountY * patternScale);
            }

            patternParent.localPosition += instruction.startingOffset;
        }
    }
}