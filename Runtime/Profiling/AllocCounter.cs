using System;
using UnityEngine.Profiling;

namespace Utils.Profiling
{
    // I took it from git-amend's YouTube channel: https://www.youtube.com/watch?v=ham_w48aRJ4
    // See UnityEngine.TestTools.Constraints.AllocatingGCMemoryConstraint
    // and https://maingauche.games/devlog/20230504-counting-allocations-in-unity/
    public class AllocCounter
    {
        private Recorder _rec;

        public AllocCounter()
        {
            _rec = Recorder.Get("GC.Alloc");
            _rec.enabled = false;

#if !UNITY_WEBGL
            _rec.FilterToCurrentThread();
#endif
            
            _rec.enabled = true;
            
        }

        public int Stop()
        {
            if (_rec == null) throw new InvalidOperationException("AllocCounter was not started.");
            
            _rec.enabled = false;
            
#if !UNITY_WEBGL
            _rec.CollectFromAllThreads();
#endif

            var result = _rec.sampleBlockCount;
            _rec = null;
            return result;
        }
    }
}