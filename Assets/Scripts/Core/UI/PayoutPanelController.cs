using System.Collections;
using Enums;
using TMPro;
using UnityEngine;
using Utils;

namespace Core.UI
{
    public class PayoutPanelController : PanelController
    {
        [SerializeField] private TMP_Text payoutWords;
        [SerializeField] private TMP_Text payoutMessage;
        
        private TMP_CharacterInfo[] _messageCharacters;
        private Matrix4x4 _matrix;
        private const float GetMeOffScreen = 1000.0f;
        private readonly Vector3[] _vertexOffsets = new Vector3[4];
        private float _inverseAnimTime;
        private Vector3 _characterCenterPivot = new Vector3();
        private TMP_TextInfo _textInfo;
        private TMP_MeshInfo[] _cachedMeshInfo;

        //anim vars, maybe export to scriptableObject?
        [SerializeField] private float animTime;
        [SerializeField] private float vertexRotation;
        [SerializeField] private float vertexStartScale;
        [SerializeField] private Vector2 vertexStartPositionOffset;
        
        public override void OpenPanel()
        {
            base.OpenPanel();

            RefreshPanel();
        }

        private void RefreshPanel()
        {
            payoutMessage.text = SlotMachine.payout == FruitType.Bar || SlotMachine.payout == FruitType.Banana ||
                                 SlotMachine.payout == FruitType.Barnana
                ? Constants.JackpotMessage
                : Constants.YouWinMessage;

            payoutWords.text = SlotMachine.payout.ToString();

            RefreshPayoutMessage();
        }

        private void Awake()
        {
            //move the text from it's layout position to anim start position
            var payoutTransform = payoutMessage.transform;
            var startPosition = payoutTransform.localPosition;
            startPosition.y += GetMeOffScreen;
            payoutTransform.localPosition = startPosition;
        }

        private void RefreshPayoutMessage()
        {
            //create cheap temp variable for looping
            _inverseAnimTime = 1 / animTime;
            //update the message text
            _textInfo = payoutMessage.textInfo;
            //update the text mesh
            payoutMessage.ForceMeshUpdate();
            //create a cache of the vertices
            _cachedMeshInfo = _textInfo.CopyMeshInfoVertexData();
            //create cheap characterInfo reference for looping
            _messageCharacters = _textInfo.characterInfo;
            StartCoroutine(nameof(AnimatePayoutMessage));
        }

        private IEnumerator AnimatePayoutMessage()
        {
            //create cheap int for looping
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
                    while (t <= animTime)
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
            for (var j = 0; j < 4; j++)
            {
                destinationVertices[vertexIndex + j] = _vertexOffsets[j];
            }

            var positionAmount = new Vector3(
                Mathf.Lerp(-_characterCenterPivot.x, vertexStartPositionOffset.x, animPercentage),
                Mathf.Lerp(vertexStartPositionOffset.y + -GetMeOffScreen, -GetMeOffScreen, animPercentage), 0.0f);
            var rotationAmount = Mathf.Lerp(vertexRotation, 0.0f, animPercentage);
            var scaleAmount = Mathf.Lerp(vertexStartScale, 1.0f, animPercentage);

            _matrix = Matrix4x4.TRS(positionAmount, Quaternion.Euler(0.0f, 0.0f, rotationAmount),
                new Vector3(scaleAmount, scaleAmount, scaleAmount));

            for (var j = 0; j < 4; j++)
            {
                destinationVertices[vertexIndex + j] =
                    _matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + j]);
                destinationVertices[vertexIndex + j] += _characterCenterPivot;
            }

            for (var j = 0; j < _textInfo.meshInfo.Length; j++)
            {
                _textInfo.meshInfo[j].mesh.vertices = destinationVertices;
                payoutMessage.UpdateGeometry(_textInfo.meshInfo[j].mesh, j);
            }
        }

        public void AnimatePayoutWords()
        {
            var wordsCharacters = payoutWords.textInfo.characterInfo;
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();

            EventManager.payoutFinish.Raise();
        }
    }
}