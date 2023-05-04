﻿using System;
 using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class TextLink : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI textMessage;
        [SerializeField] private Color32 normalLinkColor;
        [SerializeField] private Color32 highlightedLinkColor;

        private bool _hovered = false;
        private int _lastLinkIndex = -1;

        private void Start()
        {
            SetAllLinksToColor(normalLinkColor);
        }

        private void Update()
        {
            HandleHighlighting();
        }

        public void OnPointerClick (PointerEventData eventData) {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink (textMessage, eventData.position, eventData.pressEventCamera);
            if (linkIndex == -1)
                return;
            TMP_LinkInfo linkInfo = textMessage.textInfo.linkInfo[linkIndex];
            string selectedLink = linkInfo.GetLinkID();
            if (selectedLink != "") {
                Application.OpenURL (selectedLink);
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
        }

        private void HandleHighlighting()
        {
            if (_hovered)
            {
                var mousePosition = Input.mousePosition;
                int linkIndex = TMP_TextUtilities.FindIntersectingLink (textMessage, mousePosition, null);

                bool lastShouldReturnToNormalColor = _lastLinkIndex != linkIndex && _lastLinkIndex != -1;
                if (lastShouldReturnToNormalColor)
                {
                    SetLinkToColor(_lastLinkIndex, normalLinkColor);
                }

                bool newLinkToHighlight = linkIndex != -1;
                if (newLinkToHighlight)
                {
                    SetLinkToColor(linkIndex, highlightedLinkColor);
                }
                _lastLinkIndex = linkIndex;
            }
            else
            {
                if (_lastLinkIndex != -1)
                {
                    SetLinkToColor(_lastLinkIndex, normalLinkColor);
                    _lastLinkIndex = -1;
                }
            }
        }

        private void SetAllLinksToColor(Color32 color)
        {
            var count = textMessage.textInfo.linkCount;
            for (int i = 0; i < count; i++)
            {
                SetLinkToColor(i, color);
            }
        }

        private void SetLinkToColor(int linkIndex, Color32 color) {
            TMP_LinkInfo linkInfo = textMessage.textInfo.linkInfo[linkIndex];

            for( int i = 0; i < linkInfo.linkTextLength; i++ ) { // for each character in the link string
                int characterIndex = linkInfo.linkTextfirstCharacterIndex + i; // the character index into the entire text
                var charInfo = textMessage.textInfo.characterInfo[characterIndex];
                int meshIndex = charInfo.materialReferenceIndex; // Get the index of the material / sub text object used by this character.
                int vertexIndex = charInfo.vertexIndex; // Get the index of the first vertex of this character.

                Color32[] vertexColors = textMessage.textInfo.meshInfo[meshIndex].colors32; // the colors for this character

                if(charInfo.isVisible) {
                    vertexColors[vertexIndex + 0] = color;
                    vertexColors[vertexIndex + 1] = color;
                    vertexColors[vertexIndex + 2] = color;
                    vertexColors[vertexIndex + 3] = color;
                }
            }

            // Update Geometry
            textMessage.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
    }
}