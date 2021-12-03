using UnityEngine;

namespace Utils.Audio
{
    public class AudioGenerator
    {
        private int _sampleRate;
        private float _phase;

        public AudioGenerator(int sampleRate)
        {
            _sampleRate = sampleRate;
            _phase = 0;
        }

        public void WriteSineWave(float[] data, float sineFrequency, float sineGain, int channel, int channels)
        {
            float rate = sineFrequency * Mathf.PI * 2 / _sampleRate;

            for (int i = channel; i < data.Length; i += channels)
            {
                data[i] = Mathf.Sin(_phase) * sineGain;
                _phase += rate;

                if (_phase >= Mathf.PI * 2)
                    _phase -= Mathf.PI * 2;
            }
        }

        public void Reset()
        {
            _phase = 0;
        }
        
        public static void WriteSinWave(float[] samples, float sineFrequency, float sineGain, int sampleRate,
            int sampleOffset)
        {
            float rate = sineFrequency * Mathf.PI * 2 / sampleRate;
            float phase = (sampleOffset * rate) % (Mathf.PI * 2);

            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = Mathf.Sin(phase) * sineGain;
                phase += rate;

                if (phase >= Mathf.PI * 2)
                    phase -= Mathf.PI * 2;
            }
        }
    }
}
