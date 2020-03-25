using System.Collections;
using TMPro;
using UnityEngine;
using Utils;

namespace MyScriptableObjects
{
    [CreateAssetMenu(fileName = "animation", menuName = "MyAssets/TMP_VertexAnimation", order = 15)]
    public class TMP_VertexAnimation : ScriptableObject
    {
        [SerializeField] private bool AnimatePosition;
        [SerializeField] private Vector2 vertexStartPositionOffset;
        [SerializeField] private bool AnimateRotation;
        [SerializeField] private Vector3 vertexRotation;
        [SerializeField] private bool AnimateScale;
        [SerializeField] private Vector3 vertexStartScale;
        [SerializeField] private bool MoveOriginalOffScreen;
        [SerializeField] private Vector2 GetMeOffScreen;
        [SerializeField] private float animTime;
        [SerializeField] private bool animateFromTextPivot;
        [SerializeField] private TMP_Text targetText;

        private float characterAnimTime;
        private Vector3 targetTextStartingPosition;
        private TMP_CharacterInfo[] _messageCharacters;
        private Matrix4x4 _matrix;
        private readonly Vector3[] _vertexOffsets = new Vector3[4];
        private float _inverseAnimTime;
        private Vector3 _characterCenterPivot = new Vector3();
        private TMP_TextInfo _textInfo;
        private TMP_MeshInfo[] _cachedMeshInfo;
        private Vector3 centerPivotOffset;

        public void Init(TMP_Text text)
        {
            targetText = text;
            var localPosition = targetText.transform.localPosition;
            targetTextStartingPosition = new Vector3(localPosition.x, localPosition.y, localPosition.z);
        }

        public void RefreshTargetText()
        {
            //move the text from it's layout position to anim start position
            var targetTextTransform = targetText.transform;

            if (MoveOriginalOffScreen)
            {
                var startPosition = targetTextStartingPosition;
                startPosition.x += GetMeOffScreen.x;
                startPosition.y += GetMeOffScreen.y;
                targetTextTransform.localPosition = startPosition;
            }
            else
            {
                var startPosition = targetTextTransform.localPosition;
                startPosition.x = targetTextStartingPosition.x;
                startPosition.y = targetTextStartingPosition.y;
                startPosition.z = targetTextStartingPosition.z;
                targetTextTransform.localPosition = startPosition;
            }

            //update the message text
            _textInfo = targetText.textInfo;
            //update the text mesh
            targetText.ForceMeshUpdate();
            //create cheap temp variables for looping
            characterAnimTime = animTime / _textInfo.characterCount;
            _inverseAnimTime = 1 / characterAnimTime;
            //create a cache of the vertices
            _cachedMeshInfo = _textInfo.CopyMeshInfoVertexData();
            //create cheap characterInfo reference for looping
            _messageCharacters = _textInfo.characterInfo;
        }

        public IEnumerator AnimateTargetText()
        {
            //create cheap var for looping
            var characterCount = _textInfo.characterCount;
            //get vertices of each visible character to calculate center pivot offset, then animate  
            for (var i = 0; i < characterCount; i++)
            {
                var characterInfo = _messageCharacters[i];

                if (characterInfo.isVisible)
                {
                    var vertexIndex = characterInfo.vertexIndex;
                    var materialIndex = _textInfo.characterInfo[i].materialReferenceIndex;
                    var sourceVertices = _cachedMeshInfo[materialIndex].vertices;

                    _characterCenterPivot.Set(
                        (sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) * 0.5f,
                        (sourceVertices[vertexIndex + 0].y + sourceVertices[vertexIndex + 2].y) * 0.5f, 0.0f);

                    var destinationVertices = _textInfo.meshInfo[materialIndex].vertices;

                    for (var j = 0; j < 4; j++)
                    {
                        _vertexOffsets[j] = sourceVertices[vertexIndex + j] - _characterCenterPivot;
                    }

                    var t = 0.001f;
                    while (t <= characterAnimTime)
                    {
                        var animPercentage = t * _inverseAnimTime;
                        AnimateCharacter(animPercentage, destinationVertices, vertexIndex);

                        t += Time.deltaTime;
                        yield return null;
                    }

                    AnimateCharacter(1.0f, destinationVertices, vertexIndex);
                }

                yield return null;
            }
        }

        //move, rotate, and scale each visible characters vertices around the center pivot
        private void AnimateCharacter(float animPercentage, Vector3[] destinationVertices, int vertexIndex)
        {
            if (animateFromTextPivot)
                centerPivotOffset.Set(_characterCenterPivot.x, _characterCenterPivot.y, 0.0f);
            else
                centerPivotOffset = Vector3.zero;

            for (var j = 0; j < 4; j++)
            {
                destinationVertices[vertexIndex + j] = _vertexOffsets[j];
            }

            var getMeOffScreen = MoveOriginalOffScreen ? GetMeOffScreen : Vector2.zero;

            var positionAmount = AnimatePosition
                ? new Vector3(
                    Mathf.Lerp(vertexStartPositionOffset.x + -centerPivotOffset.x + -getMeOffScreen.x,
                        -getMeOffScreen.x, animPercentage),
                    Mathf.Lerp(vertexStartPositionOffset.y + -centerPivotOffset.y + -getMeOffScreen.y,
                        -getMeOffScreen.y, animPercentage),
                    0.0f)
                : Vector3.zero;

            var rotationAmount = AnimateRotation
                ? Quaternion.Euler(
                    Mathf.Abs(vertexRotation.x) > Constants.WorldSpaceTolerance
                        ? Mathf.Lerp(vertexRotation.x, 0.0f, animPercentage)
                        : 0.0f,
                    Mathf.Abs(vertexRotation.y) > Constants.WorldSpaceTolerance
                        ? Mathf.Lerp(vertexRotation.y, 0.0f, animPercentage)
                        : 0.0f,
                    Mathf.Abs(vertexRotation.z) > Constants.WorldSpaceTolerance
                        ? Mathf.Lerp(vertexRotation.z, 0.0f, animPercentage)
                        : 0.0f)
                : Quaternion.identity;

            var scaleAmount = AnimateScale
                ? new Vector3(
                    vertexStartScale.x > 0.0f ? Mathf.Lerp(vertexStartScale.x, 1.0f, animPercentage) : 1.0f,
                    vertexStartScale.y > 0.0f ? Mathf.Lerp(vertexStartScale.y, 1.0f, animPercentage) : 1.0f,
                    vertexStartScale.z > 0.0f ? Mathf.Lerp(vertexStartScale.z, 1.0f, animPercentage) : 1.0f
                )
                : Vector3.one;

            _matrix = Matrix4x4.TRS(positionAmount, rotationAmount, scaleAmount);

            for (var j = 0; j < 4; j++)
            {
                destinationVertices[vertexIndex + j] = _matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + j]);
                destinationVertices[vertexIndex + j] += _characterCenterPivot;
            }

            for (var j = 0; j < _textInfo.meshInfo.Length; j++)
            {
                _textInfo.meshInfo[j].mesh.vertices = destinationVertices;
                targetText.UpdateGeometry(_textInfo.meshInfo[j].mesh, j);
            }
        }
    }
}