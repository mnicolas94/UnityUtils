﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LinkButton : MonoBehaviour
    {
        [SerializeField] private string link;
        [SerializeField] private Button button;

        private void Start()
        {
            button.onClick.AddListener(GoToLink);
        }

        private void GoToLink()
        {
            Application.OpenURL(link);
        }
    }
}