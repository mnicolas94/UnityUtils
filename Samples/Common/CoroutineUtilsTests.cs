using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Utils.Attributes;

namespace Samples.Common
{
    public class CoroutineUtilsTests : MonoBehaviour
    {
        [SerializeField] private float time;
        [SerializeField] private float speed;

        [SerializeField] [SortingLayer] private string asd;

        private void Start()
        {
            var coroutine = CoroutineUtils.CoroutineSequence(new List<IEnumerator>
            {
                GoDir(Vector3.up),
                GoDir(Vector3.right),
                CoroutineUtils.WaitAll(
                    this,
                    new List<IEnumerator>
                    {
                        GoDir(Vector3.down),
                        GoDir(Vector3.left)
                    }
                )
            });
            StartCoroutine(coroutine);
        }

        private IEnumerator GoDir(Vector3 dir)
        {
            float startTime = Time.time;
            float endTime = startTime + time;
            while (Time.time <= endTime)
            {
                var position = transform.position;
                position += speed * Time.deltaTime * dir;
                transform.position = position;

                yield return null;
            }
        }
    }
}
