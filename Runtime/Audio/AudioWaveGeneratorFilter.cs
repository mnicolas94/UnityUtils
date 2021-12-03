using UnityEngine;

namespace Utils.Audio
{
    public class AudioWaveGeneratorFilter : MonoBehaviour
    {
        public float sineFrequency;
        [Range(0.0f, 1.0f)]
        public float sineGain;
        private AudioGenerator _audioGenerator;

        public int skipIters = 0;
        public int itersDuration = 5;
        private int _currentIter;

        private void Awake()
        {
            _audioGenerator = new AudioGenerator(AudioSettings.outputSampleRate);
            _currentIter = skipIters;
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (_currentIter < itersDuration)
            {
                _audioGenerator.WriteSineWave(data, sineFrequency, sineGain, 0, channels);

                if (_currentIter <= 0)
                {
                    _currentIter = itersDuration + skipIters - 1;
                }
                else
                {
                    _currentIter--;
                }
            }
            else
            {
                _currentIter--;
            }
            
        }
    }
}
