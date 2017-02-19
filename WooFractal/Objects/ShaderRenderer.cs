﻿using System;
using System.Collections.Generic;
using GlmNet;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;
using System.Diagnostics;
using System.IO;

namespace WooFractal
{
    /// <summary>
    /// A class that represents the scene for this sample.
    /// </summary>
    public class ShaderRenderer
    {
        uint[] _FrameBuffer = new uint[2];
        uint[] _RaytracerBuffer = new uint[2];
        uint[] _RandomNumbers = new uint[1];
        uint[] _RenderBuffer = new uint[1];

        private void LoadRandomNumbers(OpenGL gl)
        {
            gl.GenTextures(1, _RandomNumbers);

            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, _RandomNumbers[0]);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_GENERATE_MIPMAP_SGIS, OpenGL.GL_FALSE); // automatic mipmap

            byte[] pixels = new byte[4 * 4 * 1024 * 1024]; // sizeof(int)

            var fs = new FileStream(@"randomSequences.vec2", FileMode.Open);
            var len = (int)fs.Length;
            var bits = new byte[len];
            fs.Read(bits, 0, len);
            fs.Close();

            var floats = new float[len/2];
            int byteidx = 0;
            int idx = 0;
            
            for (int y = 0; y < 1024; y++)
            {
                for (int x = 0; x < 1024; x++)
                {
                    floats[idx++] = BitConverter.ToSingle(bits, byteidx);
                    byteidx+=4;
                    floats[idx++] = BitConverter.ToSingle(bits, byteidx);
                    byteidx += 4;
                    floats[idx++] = 0;
                    floats[idx++] = 1;
                }
            }

            Buffer.BlockCopy(floats, 0, pixels, 0, pixels.Length);

            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA32F, 1024, 1024, 0,
                         OpenGL.GL_RGBA, OpenGL.GL_FLOAT, pixels);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
        }

        /// <summary>
        /// Initialises the Scene.
        /// </summary>
        /// <param name="gl">The OpenGL instance.</param>
        public void Initialise(OpenGL gl, int width, int height)
        {
            //  We're going to specify the attribute locations for the position and normal, 
            //  so that we can force both shaders to explicitly have the same locations.
            const uint positionAttribute = 0;
            var attributeLocations = new Dictionary<uint, string>
            {
                {positionAttribute, "Position"}
            };

            //  Create the raymarch shader
            shaderRayMarch = new ShaderProgram();
            if (_Program == null)
            {
                shaderRayMarch.Create(gl,
                    ManifestResourceLoader.LoadTextFile(@"Shaders\RayMarch.vert"),
                    ManifestResourceLoader.LoadTextFile(@"Shaders\RayMarch.frag"), attributeLocations);
            }
            else
            {
                shaderRayMarch.Create(gl,
                    ManifestResourceLoader.LoadTextFile(@"Shaders\RayMarch.vert"),
                    _Program, attributeLocations);
            }

            // Create the transfer shader
            string fragShader = @"
#version 130
in vec2 texCoord;
out vec4 FragColor;
uniform sampler2D renderedTexture;
void main()
{
vec4 rgb = texture(renderedTexture, vec2((texCoord.x+1)*0.5, (texCoord.y+1)*0.5));
FragColor=rgb;
FragColor.rgb /= FragColor.a;
//FragColor.r = FragColor.r*0.1;
//FragColor.g = FragColor.g*0.1;
//FragColor.b = FragColor.b*0.1;
//FragColor=vec4(texCoord.x, texCoord.y, 1, 1);
}
";
            shaderTransfer = new ShaderProgram();
            shaderTransfer.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"Shaders\RayMarch.vert"),
                fragShader, attributeLocations);

            fragShader = @"
#version 130
in vec2 texCoord;
out vec4 FragColor;
void main()
{
FragColor=vec4(0,0,0,0);
}
";
            shaderClean = new ShaderProgram();
            shaderClean.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"Shaders\RayMarch.vert"),
                fragShader, attributeLocations);

            LoadRandomNumbers(gl);

            float[] viewport = new float[4];
            gl.GetFloat(OpenGL.GL_VIEWPORT, viewport);
            
            gl.GenFramebuffersEXT(2, _FrameBuffer);

            gl.GenTextures(2, _RaytracerBuffer);
            for (int i = 0; i < 2; i++)
            {
                gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, _FrameBuffer[i]);

                gl.BindTexture(OpenGL.GL_TEXTURE_2D, _RaytracerBuffer[i]);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_GENERATE_MIPMAP_SGIS, OpenGL.GL_FALSE); // automatic mipmap
//                gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, (int)viewport[2], (int)viewport[3], 0,
  //                           OpenGL.GL_RGBA, OpenGL.GL_FLOAT, null);
                gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA32F, (int)viewport[2], (int)viewport[3], 0,
                             OpenGL.GL_RGBA, OpenGL.GL_FLOAT, null);

                gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, _RaytracerBuffer[i], 0);
                gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_DEPTH_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER_EXT, 0);
            }
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

            stopWatch = new Stopwatch();
            stopWatch.Start();
            
        /*    
            gl.GenRenderbuffersEXT(2, _RaytracerBuffer);
            gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, _RaytracerBuffer[0]);
            gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_RGBA32F, (int)viewport[2], (int)viewport[3]);
            gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, _RaytracerBuffer[1]);
            gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_RGBA32F, (int)viewport[2], (int)viewport[3]);
*/
       //     gl.GenRenderbuffersEXT(1, _RenderBuffer);
            //gl.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER_EXT, _RenderBuffer[0]);
            //gl.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER_EXT, OpenGL.GL_RGBA, (int)viewport[2], (int)viewport[3]);
        }

        public void Destroy(OpenGL gl)
        {
            gl.DeleteRenderbuffersEXT(2, _RaytracerBuffer);
            gl.DeleteFramebuffersEXT(2, _FrameBuffer);
        }

        string _Program = null;

        public void Compile(OpenGL gl, string fragmentShader)
        {
            _Program = fragmentShader;

            const uint positionAttribute = 0;
            var attributeLocations = new Dictionary<uint, string>
            {
                {positionAttribute, "Position"}
            }; 
            
            shaderRayMarch = new ShaderProgram();
            shaderRayMarch.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"Shaders\RayMarch.vert"),
                fragmentShader, attributeLocations);
        }

        private bool _PingPong = false;
        private float _FrameNumber = 0;
        private bool _Progressive = false;
        private int _ProgressiveInterval = 1;
        private int _ProgressiveIntervalIndex = 0;

        public void SetProgressive(bool enabled, int interval)
        {
            _Progressive = enabled;
            _ProgressiveInterval = interval;
            _ProgressiveIntervalIndex = 0;
        }

        Stopwatch stopWatch;
        long _TotalTime;
        int _Frames;


        public void CleanBuffers(OpenGL gl)
        {
            var shader = shaderClean;

            gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, _FrameBuffer[0]);
            shader.Bind(gl);
            shader.SetUniform1(gl, "renderedTexture", 0);
            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, _RaytracerBuffer[1]);
            gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
            shader.Unbind(gl);
            
            gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, _FrameBuffer[1]);
            shader.Bind(gl);
            shader.SetUniform1(gl, "renderedTexture", 0);
            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, _RaytracerBuffer[0]);
            gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
            shader.Unbind(gl);
        }

        /// <summary>
        /// Renders the scene in retained mode.
        /// </summary>
        /// <param name="gl">The OpenGL instance.</param>
        /// <param name="useToonShader">if set to <c>true</c> use the toon shader, otherwise use a per-pixel shader.</param>
        public void Render(OpenGL gl, bool clean)
        {
            float[] viewport = new float[4];
            gl.GetFloat(OpenGL.GL_VIEWPORT, viewport);

            if (clean)
            {
                CleanBuffers(gl);
            }

            //  Get a reference to the raytracer shader.
            var shader = shaderRayMarch;

            int renderBuffer = 0;
            if (_PingPong) renderBuffer = 1;

            uint[] frameBuffer = new uint[1];
            uint[] depthCalcBuffer = new uint[1];

            if (_Depth)
            {
                gl.GenFramebuffersEXT(1, frameBuffer);
                gl.GenTextures(1, depthCalcBuffer);

                gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, frameBuffer[0]);

                gl.BindTexture(OpenGL.GL_TEXTURE_2D, depthCalcBuffer[0]);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_GENERATE_MIPMAP_SGIS, OpenGL.GL_TRUE);
                gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA32F, 1, 1, 0, OpenGL.GL_RGBA, OpenGL.GL_FLOAT, null);
                gl.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, depthCalcBuffer[0], 0);
                gl.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_DEPTH_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER_EXT, 0);
            }
            else
            {
                // setup first framebuffer (RGB32F)
                gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, _FrameBuffer[renderBuffer]);
            }

            shader.Bind(gl);

            shader.SetUniform1(gl, "screenWidth", viewport[2]);
            shader.SetUniform1(gl, "screenHeight", viewport[3]);
            shader.SetUniform1(gl, "frameNumber", _FrameNumber++);
            shader.SetUniform1(gl, "progressiveInterval", _ProgressiveInterval);

            int rt1 = shader.GetUniformLocation(gl, "renderedTexture");
            int rn1 = shader.GetUniformLocation(gl, "randomNumbers");
            Debug.WriteLine("renderedTexture : " + rt1 + " randomNumbers : " + rn1);
            gl.Uniform1(rt1, 0);
            gl.Uniform1(rn1, 1);
            shader.SetUniform1(gl, "renderedTexture", 0);
            shader.SetUniform1(gl, "randomNumbers", 1);
            shader.SetUniform1(gl, "clean", clean ? 0 : 0);
            shader.SetUniform1(gl, "depth", _Depth ? 1 : 0);
            shader.SetUniform1(gl, "mouseX", _MouseX);
            shader.SetUniform1(gl, "mouseY", viewport[3] - _MouseY);

            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, _RaytracerBuffer[_PingPong ? 0 : 1]);
            gl.ActiveTexture(OpenGL.GL_TEXTURE1);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, _RandomNumbers[0]);
            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
            gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
            shader.Unbind(gl);

            if (_Depth)
            {
                gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, depthCalcBuffer[0]);
                int[] pixels = new int[4];
                gl.GetTexImage(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, OpenGL.GL_FLOAT, pixels);
                float valr = BitConverter.ToSingle(BitConverter.GetBytes(pixels[0]), 0);
//                float valg = BitConverter.ToSingle(BitConverter.GetBytes(pixels[1]), 0);
//                float valb = BitConverter.ToSingle(BitConverter.GetBytes(pixels[2]), 0);
//                float vala = BitConverter.ToSingle(BitConverter.GetBytes(pixels[3]), 0);
                _ImageDepth = valr;
                _ImageDepthSet = true;
                _Depth = false;
            }
            else
            {
                gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);

                // get a reference to the transfer shader
                shader = shaderTransfer;

                gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, _RaytracerBuffer[renderBuffer]);

                shader.Bind(gl);
                shader.SetUniform1(gl, "renderedTexture", 0);
                gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
                shader.Unbind(gl);

                gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

                if (_Progressive)
                {
                    if (_ProgressiveIntervalIndex == 0)
                    {
                        _Frames++;
                        stopWatch.Stop();
                        _TotalTime += stopWatch.ElapsedMilliseconds;
                        Debug.WriteLine("Elapsed between pingpong : " + stopWatch.ElapsedMilliseconds + "Average time : " + _TotalTime / _Frames + "Frames : " + _Frames);
                        stopWatch.Reset();
                        stopWatch.Start();
                        _PingPong = !_PingPong;
                    }
                    else
                    {
                        Debug.WriteLine("progIndex : " + _ProgressiveIntervalIndex);
                    }
                    _ProgressiveIntervalIndex++;
                    if (clean)
                    {
                        Debug.WriteLine("Clean");
                        _TotalTime = 0;
                        _Frames = 0;
                        _ProgressiveIntervalIndex = 0;
                    }
                    if (_ProgressiveIntervalIndex > _ProgressiveInterval)
                    {
                        _ProgressiveIntervalIndex = 0;
                    }
                }
                else
                {
                    _PingPong = !_PingPong;
                }
            }
        }

        bool _Depth;
        float _MouseX;
        float _MouseY;
        public float _ImageDepth;
        public bool _ImageDepthSet;

        public void GetDepth(float mousex, float mousey)
        {
            _Depth = true;
            _MouseX = mousex;
            _MouseY = mousey;
        }
        
        //  The shaders we use.
        private ShaderProgram shaderRayMarch;
        private ShaderProgram shaderTransfer;
        private ShaderProgram shaderClean;
    }
}