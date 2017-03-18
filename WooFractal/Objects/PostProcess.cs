using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;
using SharpGL.Shaders;

namespace WooFractal
{
    public class PostProcess
    {
        public class GaussianShader
        {
            public double GetGaussianFactor(double standardDeviation, double x)
            {
                return (1 / Math.Sqrt(2 * Math.PI * standardDeviation * standardDeviation)) * Math.Exp(-x * x / (2 * standardDeviation * standardDeviation));
            }

            public void GetWeights(float standardDeviation, float maxError, out List<float> weights)
            {
                weights = new List<float>();
                double gaussian = GetGaussianFactor(standardDeviation, 0);
                float total = (float)gaussian;
                weights.Add((float)gaussian);

                int index = 1;
                gaussian = GetGaussianFactor(standardDeviation, index++);
                while (gaussian > maxError*total)
                {
                    total += (float)gaussian*2;
                    weights.Add((float)gaussian);
                    gaussian = GetGaussianFactor(standardDeviation, index++);
                }

                for (int i = 0; i < weights.Count(); i++)
                {
                    weights[i] /= total;
                }
            }

            public string GetShader(float standardDeviation, float angle, float error)
            {
                List<float> weights;
                GetWeights(standardDeviation, error, out weights);

                // Create the transfer shader
                string shader = @"
#version 130
in vec2 texCoord;
out vec4 FragColor;
uniform sampler2D renderedTexture;
uniform float screenWidth;
uniform float screenHeight;

void main()
{
 vec2 offset = vec2(" + Math.Cos(angle) + "*1/screenWidth, " + Math.Sin(angle) + @"*1/screenHeight);
 vec2 xy = (texCoord+vec2(1))*0.5;";
                shader += "vec4 rgb = texture(renderedTexture, xy) * " + weights[0] + ";";
                int weightsIdx = 1;
                while (weightsIdx<weights.Count())
                {
                    if (weights.Count() - weightsIdx == 1)
                    {
                        float w = weights[weightsIdx];
                        shader += "rgb += texture(renderedTexture, xy+offset*"+weightsIdx+") * " + w + ";";
                        shader += "rgb += texture(renderedTexture, xy-offset*"+weightsIdx+") * " + w + ";";
                        weightsIdx++;
                    }
                    else
                    {
                        float w0 = weights[weightsIdx];
                        float w1 = weights[weightsIdx+1];
                        shader += "rgb += texture(renderedTexture, xy+offset*" + ((float)weightsIdx + (w1 / (w0 + w1))) + ") * " + (w0 + w1) + ";";
                        shader += "rgb += texture(renderedTexture, xy-offset*" + ((float)weightsIdx + (w1 / (w0 + w1))) + ") * " + (w0 + w1) + ";";
                        weightsIdx += 2;
                    }
                }
                shader += @"FragColor=rgb;
}
";

                return shader;
            }
        }

        public PostProcess()
        {
            _PostProcessAmount = 0.0;
            _GaussianSD = 10.0;
            _GaussianExposure = 8.0;
            _GammaFactor = 1.0;
            _GammaContrast = 1.0;
            _ToneMappingMode = 0;
            _ToneFactor = 1.0;
        }

        public void Initialise(OpenGL gl)
        {
            const uint positionAttribute = 0;
            var attributeLocations = new Dictionary<uint, string>
            {
                {positionAttribute, "Position"}
            };

            string fragShaderHighlights = @"
#version 130
in vec2 texCoord;
out vec4 FragColor;
uniform sampler2D renderedTexture;
uniform float exponent;

void main()
{
 vec4 rgb = texture(renderedTexture, vec2((texCoord.x+1)*0.5, (texCoord.y+1)*0.5));
 rgb = pow(rgb, vec4(exponent));
 FragColor=rgb;
}
";
            shaderHighlights = new ShaderProgram();
            shaderHighlights.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"Shaders\RayMarch.vert"),
                fragShaderHighlights, attributeLocations);

            PostProcess.GaussianShader gaussianShader = new PostProcess.GaussianShader();
            shaderGaussianX = new ShaderProgram();
            shaderGaussianX.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"Shaders\RayMarch.vert"),
                gaussianShader.GetShader((float)_GaussianSD, 0, 0.005f), attributeLocations);

            shaderGaussianY = new ShaderProgram();
            shaderGaussianY.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"Shaders\RayMarch.vert"),
                gaussianShader.GetShader((float)_GaussianSD, (float)Math.PI / 2, 0.005f), attributeLocations);

            string fragShaderBlend = @"
#version 130
in vec2 texCoord;
out vec4 FragColor;
uniform sampler2D renderedTexture1;
uniform sampler2D renderedTexture2;
uniform float factor1;
uniform float factor2;

void main()
{
 vec4 rgb1 = texture(renderedTexture1, vec2((texCoord.x+1)*0.5, (texCoord.y+1)*0.5));
 rgb1.rgb /= rgb1.a;
 vec4 rgb2 = texture(renderedTexture2, vec2((texCoord.x+1)*0.5, (texCoord.y+1)*0.5));
 FragColor=rgb1*factor1 + rgb2*factor2;
}
";
            shaderBlend = new ShaderProgram();
            shaderBlend.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"Shaders\RayMarch.vert"),
                fragShaderBlend, attributeLocations);

            // Create the transfer shader
            string fragShaderRGBATransfer = @"
#version 130
in vec2 texCoord;
out vec4 FragColor;
uniform sampler2D renderedTexture;

void main()
{
 FragColor = texture(renderedTexture, vec2((texCoord.x+1)*0.5, (texCoord.y+1)*0.5));
 FragColor.rgb /= FragColor.a;
}
";
            shaderRGBA = new ShaderProgram();
            shaderRGBA.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"Shaders\RayMarch.vert"),
                fragShaderRGBATransfer, attributeLocations);
        }

        public void Render(OpenGL gl, int targetWidth, int targetHeight, ref int target, uint[] effectFrameBuffer, uint raytracerBuffer, uint[] effectRaytracerBuffer)
        {
            if (_PostProcessFilter == 0)
            {
                // !!!!!!!!!!!!!!! rgb / a
                gl.Viewport(0, 0, targetWidth, targetHeight);
                gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, effectFrameBuffer[target]);
                var shader = shaderRGBA;
                gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, raytracerBuffer);
                shader.Bind(gl);
                shader.SetUniform1(gl, "renderedTexture", 0);
                gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
                shader.Unbind(gl);
            }
            else if (_PostProcessFilter == 1)
            {
                // !!!!!!!!!!!!!!! rgb / a
                gl.Viewport(0, 0, targetWidth, targetHeight);
                gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, effectFrameBuffer[target]);
                var shader = shaderRGBA;
                gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, raytracerBuffer);
                shader.Bind(gl);
                shader.SetUniform1(gl, "renderedTexture", 0);
                gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
                shader.Unbind(gl);

                // !!!!!!!!!!!!!!! exponent
                target = target > 0 ? 0 : 1;
                gl.Viewport(0, 0, targetWidth, targetHeight);
                gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, effectFrameBuffer[target]);
                shader = shaderHighlights;
                gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, effectRaytracerBuffer[target > 0 ? 0 : 1]);
                shader.Bind(gl);
                shader.SetUniform1(gl, "exponent", (float)_GaussianExposure);
                shader.SetUniform1(gl, "renderedTexture", 0);
                gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
                shader.Unbind(gl);

                // !!!!!!!!!!!!!!! gaussianShaderX
                target = target > 0 ? 0 : 1;
                gl.Viewport(0, 0, targetWidth, targetHeight);
                gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, effectFrameBuffer[target]);
                shader = shaderGaussianX;
                gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, effectRaytracerBuffer[target > 0 ? 0 : 1]);
                shader.Bind(gl);
                shader.SetUniform1(gl, "screenWidth", targetWidth);
                shader.SetUniform1(gl, "screenHeight", targetHeight);
                shader.SetUniform1(gl, "renderedTexture", 0);
                gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
                shader.Unbind(gl);

                // !!!!!!!!!!!!!!! gaussianShaderY
                target = target > 0 ? 0 : 1;
                gl.Viewport(0, 0, targetWidth, targetHeight);
                gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, effectFrameBuffer[target]);
                shader = shaderGaussianY;
                gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, effectRaytracerBuffer[target > 0 ? 0 : 1]);
                shader.Bind(gl);
                shader.SetUniform1(gl, "screenWidth", targetWidth);
                shader.SetUniform1(gl, "screenHeight", targetHeight);
                shader.SetUniform1(gl, "renderedTexture", 0);
                gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
                shader.Unbind(gl);

                // !!!!!!!!!!!!!!! blend
                target = target > 0 ? 0 : 1;
                gl.Viewport(0, 0, targetWidth, targetHeight);
                gl.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, effectFrameBuffer[target]);
                shader = shaderBlend;
                gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, raytracerBuffer);
                gl.ActiveTexture(OpenGL.GL_TEXTURE1);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, effectRaytracerBuffer[target > 0 ? 0 : 1]);
                shader.Bind(gl);
                shader.SetUniform1(gl, "factor1", 1);
                shader.SetUniform1(gl, "factor2", (float)_PostProcessAmount);
                int rte1 = shader.GetUniformLocation(gl, "renderedTexture1");
                int rne1 = shader.GetUniformLocation(gl, "renderedTexture2");
                gl.Uniform1(rte1, 0);
                gl.Uniform1(rne1, 1);
                gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);
                shader.Unbind(gl);
            }
        }

        public double _PostProcessAmount;
        public double _GaussianSD;
        public double _GaussianExposure;
        public double _GammaFactor;
        public double _GammaContrast;
        public int _ToneMappingMode;
        public double _ToneFactor;
        public int _PostProcessFilter;

        private ShaderProgram shaderHighlights;
        private ShaderProgram shaderGaussianX;
        private ShaderProgram shaderGaussianY;
        private ShaderProgram shaderBlend;
        private ShaderProgram shaderRGBA;
    }
}
