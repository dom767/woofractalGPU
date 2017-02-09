using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Drawing;
using Microsoft.Win32;

namespace WooFractal
{
    public enum RS_SCALE
    {
        RS_QUARTER,
        RS_THIRD,
        RS_HALF,
        RS_ONE,
        RS_TWO,
        RS_FOUR
    };

    public class ImageRenderer
    {
        System.Windows.Controls.Image _Image;
        string _XML;
        bool _Continuous;
        int _RenderWidth;
        int _RenderHeight;

        int _Width;
        int _Height;

        public Colour _MinColour { get; set; }
        public Colour _MaxColour { get; set; }

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void StartRender();

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopRender();

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CopyBuffer(float[] buffer);

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SyncRender(float[] buffer);

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitialiseRender(string description);

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCamera(string description);

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetViewport(string description);

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PostProcess(float[] targetBuffer, float[] sourceBuffer, double maxValue, int iterations, float[] kernel, float boostPower, float targetweighting, float sourceweighting, int width, int height);

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GaussianBlur(float[] targetBuffer, float[] sourceBuffer, double maxValue, int size, float boostPower, float targetweighting, float sourceweighting, int width, int height);

        public ImageRenderer(System.Windows.Controls.Image image, string xml, int renderWidth, int renderHeight, bool continuous)
        {
            _Image = image;
            _XML = xml;
            _RenderWidth = renderWidth;
            _RenderHeight = renderHeight;
            _Continuous = continuous;
            _PostProcess = new PostProcess();
            InitialiseRender(_XML);
        }

        public void UpdateCamera(string cameraXML)
        {
            SetCamera(cameraXML);
        }

        private void GetMinMax(float[] renderBuffer, int renderWidth, int renderHeight)
        {
            _MinColour = new Colour(1000000.0f, 1000000.0f, 1000000.0f);
            _MaxColour = new Colour(0.0f, 0.0f, 0.0f);

            for (int y = 0; y < renderHeight; y++)
            {
                for (int x = 0; x < renderWidth; x++)
                {
                    int idx = (x + y * renderWidth) * 3;
                    float red = renderBuffer[idx], green = renderBuffer[idx + 1], blue = renderBuffer[idx + 2];
                    
                    if (red < _MinColour._Red) _MinColour._Red = red;
                    if (green < _MinColour._Green) _MinColour._Green = green;
                    if (blue < _MinColour._Blue) _MinColour._Blue = blue;

                    if (red > _MaxColour._Red) _MaxColour._Red = red;
                    if (green > _MaxColour._Green) _MaxColour._Green = green;
                    if (blue > _MaxColour._Blue) _MaxColour._Blue = blue;
                }
            }

            _MaxValue = Math.Max(_MaxColour._Red, Math.Max(_MaxColour._Green, _MaxColour._Blue));
        }

        public enum Transfer { Ramp, Exposure, Tone, Gamma };
        public Transfer _TransferType;
        public double _MaxValue=1;
        public double _RampValue;
        public double _ExposureFactor;
        public float _ToneFactor;
        public float _GammaFactor;
        public float _GammaContrast;

        public void TransferFloatToInt(byte[] pixels, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int red, green, blue;
                    if (_TransferType == Transfer.Ramp)
                    {
                        red = (int)(_Buffer[(x + y * width) * 3] * 255.99f / _RampValue);
                        green = (int)(_Buffer[(x + y * width) * 3 + 1] * 255.99f / _RampValue);
                        blue = (int)(_Buffer[(x + y * width) * 3 + 2] * 255.99f / _RampValue);
                    }
                    else if (_TransferType == Transfer.Exposure)
                    {
                        red = (int)((1 - Math.Exp(-_Buffer[(x + y * width) * 3] * _ExposureFactor)) * 255.99f);
                        green = (int)((1 - Math.Exp(-_Buffer[(x + y * width) * 3 + 1] * _ExposureFactor)) * 255.99f);
                        blue = (int)((1 - Math.Exp(-_Buffer[(x + y * width) * 3 + 2] * _ExposureFactor)) * 255.99f);
                    }
                    else if (_TransferType == Transfer.Gamma)
                    {
                        float redIn = _Buffer[(x + y * width) * 3];
                        float greenIn = _Buffer[(x + y * width) * 3 + 1];
                        float blueIn = _Buffer[(x + y * width) * 3 + 2];
                        float luminance = redIn * 0.2126f + greenIn * 0.7152f + blueIn * 0.0722f;
                        float luminanceOut = _GammaFactor * (float)Math.Pow((double)luminance, (double)_GammaContrast);
                        float multiplier = luminance>0 ? (luminanceOut / luminance) : 0;
                        red = (int)(redIn * multiplier * 255.99f);
                        green = (int)(greenIn * multiplier * 255.99f);

                        blue = (int)(blueIn * multiplier * 255.99f);
                    }
                    else // Tone
                    {
                        float redIn = _Buffer[(x + y * width) * 3] * _ToneFactor;
                        float greenIn = _Buffer[(x + y * width) * 3 + 1] * _ToneFactor;
                        float blueIn = _Buffer[(x + y * width) * 3 + 2] * _ToneFactor;
                        float luminance = redIn * 0.2126f + greenIn * 0.7152f + blueIn * 0.0722f;
                        float luminanceOut = luminance / (1.0f + luminance);
                        float multiplier = luminance > 0 ? (luminanceOut / luminance) : 0;
                        red = (int)(redIn * multiplier * 255.99f);
                        green = (int)(greenIn * multiplier * 255.99f);
                        blue = (int)(blueIn * multiplier * 255.99f);
                    }

                    red = Math.Min(red, 255);
                    green = Math.Min(green, 255);
                    blue = Math.Min(blue, 255);

                    pixels[(width * y + x) * 4 + 0] = (byte)(blue);
                    pixels[(width * y + x) * 4 + 1] = (byte)(green);
                    pixels[(width * y + x) * 4 + 2] = (byte)(red);
                    pixels[(width * y + x) * 4 + 3] = (byte)(255);
                }
            }
        }

        byte[] _Pixels;
        public void TransferFloatToInt()
        {
            TransferFloatToInt(_Pixels, _Width, _Height);

            Int32Rect rect = new Int32Rect(0, 0, (int)_Width, (int)_Height);

            _WriteableBitmap.WritePixels(rect, _Pixels, _Width * 4, (int)0);

            _Image.Source = _WriteableBitmap;
        }

        float[] _Buffer;

        public void ZoomCopy(float[] srcBuffer, int srcWidth, int srcHeight, float[] destBuffer, int destWidth, int destHeight)
        {
            // lets work out if we're 1:1 on ratio
            float srcRatio = (float)srcWidth/(float)srcHeight;
            float dstRatio = (float)destWidth/(float)destHeight;
            int dstXStart = 0;
            int dstYStart = 0;
            int dstXWidth = destWidth;
            int dstYHeight = destHeight;
            if (Math.Abs(srcRatio-dstRatio)>0.05)
            {
                if (dstRatio>srcRatio)
                {
                    dstXWidth = destHeight*srcWidth/srcHeight;
                    dstXStart = (int)(((float)destWidth - dstXWidth) * 0.5f);
                }
                else
                {
                    dstYHeight = destWidth * srcHeight / srcWidth;
                    dstYStart = (int)(((float)destHeight - dstYHeight) * 0.5f);
                }
            }

            float srcPosX = 0;
            float srcPosY = 0;
            float srcDeltaX = ((float)(srcWidth - 1)) / ((float)(dstXWidth - 1));
            float srcDeltaY = ((float)(srcHeight - 1)) / ((float)(dstYHeight - 1)); 

            for (int y = dstYStart; y < dstYStart+dstYHeight; y++)
            {
                for (int x = dstXStart; x < dstXStart+dstXWidth; x++)
                {
                    destBuffer[3 * (y * destWidth + x)] = srcBuffer[3 * (((int)srcPosY * srcWidth) + (int)srcPosX)];
                    destBuffer[3 * (y * destWidth + x) + 1] = srcBuffer[3 * (((int)srcPosY * srcWidth) + (int)srcPosX) + 1];
                    destBuffer[3 * (y * destWidth + x) + 2] = srcBuffer[3 * (((int)srcPosY * srcWidth) + (int)srcPosX) + 2];

                    srcPosX += srcDeltaX;
                }
                srcPosY += srcDeltaY;
                srcPosX = 0;
            }
        }

        int idx = 0;

/*        public void PostProcess(float[] targetBuffer, float[] sourceBuffer, float[] boostBuffer, float[] kernel, float boostPower, float kernelweighting, float sourceweighting, int iterations, int width, int height)
        {
            if (boostPower != 1)
            {
                double maxV = _MaxValue<0.001f ? 1 : _MaxValue;
                double boostP = _BoostPower;
                for (int i = 0; i < width * height * 3; i++)
                {
                    boostBuffer[i] = (float)Math.Pow(sourceBuffer[i] / maxV, boostP);
                }
            }
            else
            {
                boostBuffer = sourceBuffer;
            }

            for (int iter = 0; iter < _Iterations; iter++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int patchStartX = x - 2;
                        patchStartX = Math.Max(0, patchStartX) - x;
                        int patchEndX = x + 2;
                        patchEndX = Math.Min(patchEndX, width-1) - x;
                        int patchStartY = y - 2;
                        patchStartY = Math.Max(0, patchStartY) - y;
                        int patchEndY = y + 2;
                        patchEndY = Math.Min(patchEndY, height-1) - y;

                        // iterate over a patch
                        float totalr = 0;
                        float totalg = 0;
                        float totalb = 0;
                        float totalweighting = 0;
                        for (int py = patchStartY; py <= patchEndY; py++)
                        {
                            for (int px = patchStartX; px <= patchEndX; px++)
                            {
                                int rx = px + x;
                                int ry = py + y;
                                float kernelW = kernel[(py+2) * 5 + px+2];
                                totalr += boostBuffer[(ry * width + rx) * 3] * kernelW;
                                totalg += boostBuffer[(ry * width + rx) * 3 + 1] * kernelW;
                                totalb += boostBuffer[(ry * width + rx) * 3 + 2] * kernelW;
                                totalweighting += kernelW;
                            }
                        }

                        // divide through
                        targetBuffer[(x + y * width) * 3] = totalr / totalweighting;
                        targetBuffer[(x + y * width) * 3 + 1] = totalg / totalweighting;
                        targetBuffer[(x + y * width) * 3 + 2] = totalb / totalweighting;
                    }
                }
                for (int i = 0; i < width * height * 3; i++)
                {
                    boostBuffer[i] = targetBuffer[i];
                }
            }

            // divide through
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    targetBuffer[(x + y * width) * 3] = _TargetWeight * targetBuffer[(x + y * width) * 3] + _SourceWeight * sourceBuffer[(x + y * width) * 3];
                    targetBuffer[(x + y * width) * 3 + 1] = _TargetWeight * targetBuffer[(x + y * width) * 3 + 1] + _SourceWeight * sourceBuffer[(x + y * width) * 3 + 1];
                    targetBuffer[(x + y * width) * 3 + 2] = _TargetWeight * targetBuffer[(x + y * width) * 3 + 2] + _SourceWeight * sourceBuffer[(x + y * width) * 3 + 2];
                }
            }
        }
*/
        public void SetPostProcess(PostProcess postprocess)
        {
            _PostProcess = postprocess;
        }

        PostProcess _PostProcess;
        
        bool _FixedExposure;
        float _ExposureValue;

        public void SetExposureValue(float exposureValue)
        {
            _ExposureValue = exposureValue;
        }

        public void SetFixedExposure(bool fixedExposure)
        {
            _FixedExposure = fixedExposure;
        }

        public void TransferLatest(bool highQuality)
        {
            float[] renderBuffer = new float[_RenderHeight * _RenderWidth * 3];
            CopyBuffer(renderBuffer);

            if (highQuality)
            {
                GetMinMax(renderBuffer, _RenderWidth, _RenderHeight);

                if (_PostProcess._SettingsFastGaussian._Enabled)
                {
                    float[] targetBuffer = new float[_RenderHeight * _RenderWidth * 3];
                    GaussianBlur(targetBuffer,
                        renderBuffer,
                        _MaxValue,
                        _PostProcess._SettingsFastGaussian._Width,
                        (float)_PostProcess._SettingsFastGaussian._BoostPower,
                        (float)_PostProcess._SettingsFastGaussian._TargetWeight,
                        (float)_PostProcess._SettingsFastGaussian._SourceWeight,
                        _RenderWidth,
                        _RenderHeight);
                    renderBuffer = targetBuffer;
                }

                if (_PostProcess._Settings5x5._Enabled)
                {
                    float[] targetBuffer = new float[_RenderHeight * _RenderWidth * 3];
                    PostProcess(targetBuffer,
                        renderBuffer,
                        _MaxValue,
                        _PostProcess._Settings5x5._Iterations,
                        _PostProcess._Settings5x5._Kernel,
                        (float)_PostProcess._Settings5x5._BoostPower,
                        (float)_PostProcess._Settings5x5._TargetWeight,
                        (float)_PostProcess._Settings5x5._SourceWeight,
                        _RenderWidth,
                        _RenderHeight);
                    renderBuffer = targetBuffer;
                }
            }
            else
            {
                GetMinMax(renderBuffer, _RenderWidth, _RenderHeight);
            }

            ZoomCopy(renderBuffer, _RenderWidth, _RenderHeight, _Buffer, _Width, _Height);

            TransferFloatToInt();
            idx++;
        }
        
        WriteableBitmap _WriteableBitmap;
        public void Render()
        {
            _Width = (int)_Image.Width;
            _Height = (int)_Image.Height;
            _Buffer = new float[_Height * _Width * 3];
            _Pixels = new byte[4 * _Height * _Width];
            _WriteableBitmap = new WriteableBitmap((int)_Width, (int)_Height, 96, 96, PixelFormats.Bgra32, null);
                
            string XML = @"
<VIEWPORT width=" + _RenderWidth + @" height=" + _RenderHeight + @"/>";
            SetViewport(XML);

            if (_Continuous)
            {
                StartRender();
            }
            else
            {
                float[] renderBuffer = new float[_RenderHeight * _RenderWidth * 3];
                SyncRender(renderBuffer);
                ZoomCopy(renderBuffer, _RenderWidth, _RenderHeight, _Buffer, _Width, _Height);

                if (_FixedExposure)
                {
                    _RampValue = _ExposureValue;
                    _TransferType = Transfer.Ramp;
                }
                else
                {
                    GetMinMax(renderBuffer, _RenderWidth, _RenderHeight);
                    _RampValue = _MaxValue;
                    _TransferType = Transfer.Ramp;
                }

                TransferFloatToInt();
            }

        }

        public void Stop()
        {
            StopRender();
        }

        public void Save()
        {
            // Save image
            float[] renderBuffer = new float[_RenderHeight * _RenderWidth * 3];
            SyncRender(renderBuffer);

            GetMinMax(renderBuffer, _RenderWidth, _RenderHeight);

            //postpro
            if (_PostProcess._SettingsFastGaussian._Enabled)
            {
                float[] targetBuffer = new float[_RenderHeight * _RenderWidth * 3];
                GaussianBlur(targetBuffer,
                    renderBuffer,
                    _MaxValue,
                    _PostProcess._SettingsFastGaussian._Width,
                    (float)_PostProcess._SettingsFastGaussian._BoostPower,
                    (float)_PostProcess._SettingsFastGaussian._TargetWeight,
                    (float)_PostProcess._SettingsFastGaussian._SourceWeight,
                    _RenderWidth,
                    _RenderHeight);
                renderBuffer = targetBuffer;
            }

            if (_PostProcess._Settings5x5._Enabled)
            {
                float[] targetBuffer = new float[_RenderHeight * _RenderWidth * 3];
                PostProcess(targetBuffer,
                    renderBuffer,
                    _MaxValue,
                    _PostProcess._Settings5x5._Iterations,
                    _PostProcess._Settings5x5._Kernel,
                    (float)_PostProcess._Settings5x5._BoostPower,
                    (float)_PostProcess._Settings5x5._TargetWeight,
                    (float)_PostProcess._Settings5x5._SourceWeight,
                    _RenderWidth,
                    _RenderHeight);
                renderBuffer = targetBuffer;
            }

            float[] oldBuffer = _Buffer;
            _Buffer = new float[_RenderHeight * _RenderWidth * 3];
            ZoomCopy(renderBuffer, _RenderWidth, _RenderHeight, _Buffer, _RenderWidth, _RenderHeight);

            byte[] pixels = new byte[_RenderHeight * _RenderWidth * 4];

            TransferFloatToInt(pixels, _RenderWidth, _RenderHeight);

            Bitmap bmp = new Bitmap(_RenderWidth, _RenderHeight);
            int x, y;
            for (y = 0; y < _RenderHeight; y++)
            {
                for (x = 0; x < _RenderWidth; x++)
                {
                    bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(pixels[(x + y * _RenderWidth)*4 + 2],
                        pixels[(x + y * _RenderWidth)*4 + 1],
                        pixels[(x + y * _RenderWidth)*4 + 0]));
                }
            }
            _Buffer = oldBuffer;

            string store = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\WooFractal\\Exports";
            if (!System.IO.Directory.Exists(store))
            {
                System.IO.Directory.CreateDirectory(store);
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = store;
            saveFileDialog1.Filter = "PNG (*.png)|*.png";
            saveFileDialog1.FilterIndex = 1;

            if (saveFileDialog1.ShowDialog() == true)
            {
                bmp.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                bmp.Save(saveFileDialog1.FileName.Replace(".png", ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }
    }
}
