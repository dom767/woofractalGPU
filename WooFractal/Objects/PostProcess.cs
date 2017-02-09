using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooFractal
{
    public class PostProcess
    {
        public class Settings5x5
        {
            public Settings5x5()
            {
                _Enabled = false;
                _Iterations = 1;
                _SourceWeight = 0.9;
                _TargetWeight = 0.1;
                _BoostPower = 10;

                SetGaussian();
            }

            public void SetGaussian()
            {
                int kidx=0;
                _Kernel[kidx++] = 1; _Kernel[kidx++] = 4; _Kernel[kidx++] = 7; _Kernel[kidx++] = 4; _Kernel[kidx++] = 1;
                _Kernel[kidx++] = 4; _Kernel[kidx++] = 16; _Kernel[kidx++] = 26; _Kernel[kidx++] = 16; _Kernel[kidx++] = 4;
                _Kernel[kidx++] = 7; _Kernel[kidx++] = 26; _Kernel[kidx++] = 41; _Kernel[kidx++] = 26; _Kernel[kidx++] = 7;
                _Kernel[kidx++] = 4; _Kernel[kidx++] = 16; _Kernel[kidx++] = 26; _Kernel[kidx++] = 16; _Kernel[kidx++] = 4;
                _Kernel[kidx++] = 1; _Kernel[kidx++] = 4; _Kernel[kidx++] = 7; _Kernel[kidx++] = 4; _Kernel[kidx++] = 1;
            }

            public void SetLinear()
            {
                int kidx=0;
                _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0;
                _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0;
                _Kernel[kidx++] = 1; _Kernel[kidx++] = 4; _Kernel[kidx++] = 7; _Kernel[kidx++] = 4; _Kernel[kidx++] = 1;
                _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0;
                _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0;
            }

            public void SetStar()
            {
                int kidx=0;
                _Kernel[kidx++] = 1; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 1;
                _Kernel[kidx++] = 0; _Kernel[kidx++] = 4; _Kernel[kidx++] = 0; _Kernel[kidx++] = 4; _Kernel[kidx++] = 0;
                _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 7; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0;
                _Kernel[kidx++] = 0; _Kernel[kidx++] = 4; _Kernel[kidx++] = 0; _Kernel[kidx++] = 4; _Kernel[kidx++] = 0;
                _Kernel[kidx++] = 1; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 0; _Kernel[kidx++] = 1;
            }

            public int _Iterations;
            public bool _Enabled;
            public double _SourceWeight;
            public double _TargetWeight;
            public double _BoostPower;
            public float[] _Kernel = new float[25];
        };

        public class SettingsFastGaussian
        {
            public SettingsFastGaussian()
            {
                _Enabled = false;
                _Width = 10;
                _SourceWeight = 0.5;
                _TargetWeight = 0.5;
                _BoostPower = 6;
            }

            public bool _Enabled;
            public int _Width;
            public double _SourceWeight;
            public double _TargetWeight;
            public double _BoostPower;
        };

        public PostProcess()
        {
        }

        public void SetGaussian5x5() { _Settings5x5.SetGaussian(); }
        public void SetLinear5x5() { _Settings5x5.SetLinear(); }
        public void SetStar5x5() { _Settings5x5.SetStar(); }

        public Settings5x5 _Settings5x5 = new Settings5x5();
        public SettingsFastGaussian _SettingsFastGaussian = new SettingsFastGaussian();
    }
}
