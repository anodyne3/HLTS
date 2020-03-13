using MyScriptableObjects;
using UnityEngine;

namespace Core
{
    public class PatternGenerator : MonoBehaviour
    {
        [SerializeField] private PatternInstruction patternInstructions;
        [SerializeField] private GameObject disposableChild; 

        private void Start()
        {
            GeneratePattern();
        }

        private void GeneratePattern()
        {
            patternInstructions.GeneratePattern(disposableChild.transform);
        }

        //for designing/debug
        public void RegeneratePatterns()
        {
            foreach (Transform child in disposableChild.transform)
            {
                Destroy(child.gameObject);
            }
            
            GeneratePattern();
        }
    }
}