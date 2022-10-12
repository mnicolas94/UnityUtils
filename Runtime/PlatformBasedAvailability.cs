﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class PlatformBasedAvailability : MonoBehaviour
    {
        [SerializeField] private List<RuntimePlatform> _platforms;
        [SerializeField] private bool _availableIfInList;

        private void Awake()
        {
            bool contains = _platforms.Contains(Application.platform);
            bool destroy = contains ^ _availableIfInList;

            if (destroy)
            {
                gameObject.SetActive(false);
            }
        }
    }
}