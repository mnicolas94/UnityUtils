using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Serializables;

namespace Samples.Common
{
    public class SerializableDictionarySample : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<string, int> _dictionary;
    }
}