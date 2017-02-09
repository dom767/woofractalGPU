﻿using System;
using System.Linq;
using SharpGL;
using GlmNet;
using SharpGL.VertexBuffers;

namespace WooFractal
{
    //  Original Source: http://prideout.net/blog/?p=22

    /// <summary>
    /// The TrefoilKnot class creates geometry
    /// for a trefoil knot.
    /// </summary>
    public class TrefoilKnot
    {
        public void GenerateGeometry(OpenGL gl, uint vertexAttributeLocation, uint normalAttributeLocation)
        {

            //  Create the vertex array object. This will hold the state of all of the
            //  vertex buffer operations that follow it's binding, meaning instead of setting the 
            //  vertex buffer data and binding it each time, we can just bind the array and call
            //  DrawElements - much easier.
            vertexBufferArray = new VertexBufferArray();
            vertexBufferArray.Create(gl);
            vertexBufferArray.Bind(gl);

            //  Generate the vertices and normals.
            CreateVertexNormalBuffer(gl, vertexAttributeLocation, normalAttributeLocation);
            CreateIndexBuffer(gl);


            vertexBufferArray.Unbind(gl);
        }
        /// <summary>
        /// Evaluates the trefoil, providing the vertex at a given coordinate.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="t">The t.</param>
        /// <returns>The vertex at (s,t).</returns>
        private static vec3 EvaluateTrefoil(float s, float t)
        {
            const float TwoPi = (float)Math.PI * 2;

            float a = 0.5f;
            float b = 0.3f;
            float c = 0.5f;
            float d = 0.1f;
            float u = (1 - s) * 2 * TwoPi;
            float v = t * TwoPi;
            float r = (float)(a + b * Math.Cos(1.5f * u));
            float x = (float)(r * Math.Cos(u));
            float y = (float)(r * Math.Sin(u));
            float z = (float)(c * Math.Sin(1.5f * u));

            vec3 dv = new vec3();
            dv.x = (float) (-1.5f*b*Math.Sin(1.5f*u)*Math.Cos(u) - (a + b*Math.Cos(1.5f*u))*Math.Sin(u));
            dv.y = (float) (-1.5f*b*Math.Sin(1.5f*u)*Math.Sin(u) + (a + b*Math.Cos(1.5f*u))*Math.Cos(u)); 
            dv.z = (float) (1.5f*c*Math.Cos(1.5f*u));
            
            vec3 q = glm.normalize(dv);
            vec3 qvn = glm.normalize(new vec3(q.y, -q.x, 0.0f));
            vec3 ww =  glm.cross(q, qvn);

            vec3 range = new vec3();
            range.x = (float) (x + d*(qvn.x*Math.Cos(v) + ww.x*Math.Sin(v)));
            range.y = (float)(y + d * (qvn.y * Math.Cos(v) + ww.y * Math.Sin(v)));
            range.z = (float)(z + d * ww.z * Math.Sin(v));
            
            return range;
        }

        private void CreateVertexNormalBuffer(OpenGL gl, uint vertexAttributeLocation, uint normalAttributeLocation)
        {
            var vertexCount = slices * stacks;

            vertices = new vec3[vertexCount];
            normals = new vec3[vertexCount];

            int count = 0;

            float ds = 1.0f / slices;
            float dt = 1.0f / stacks;

            // The upper bounds in these loops are tweaked to reduce the
            // chance of precision error causing an incorrect # of iterations.

            for (float s = 0; s < 1 - ds / 2; s += ds)
            {
                for (float t = 0; t < 1 - dt / 2; t += dt)
                {
                    const float E = 0.01f;
                    vec3 p = EvaluateTrefoil(s, t);
                    vec3 u = EvaluateTrefoil(s + E, t) - p;
                    vec3 v = EvaluateTrefoil(s, t + E) - p;
                    vec3 n = glm.normalize(glm.cross(u, v));
                    vertices[count] = p;
                    normals[count] = n;
                    count++;
                }
            }
            //  Create the vertex data buffer.
            var vertexBuffer = new VertexBuffer();
            vertexBuffer.Create(gl);
            vertexBuffer.Bind(gl);
            vertexBuffer.SetData(gl, vertexAttributeLocation, vertices.SelectMany(v => v.to_array()).ToArray(), false, 3);
         
            var normalBuffer = new VertexBuffer();
            normalBuffer.Create(gl);
            normalBuffer.Bind(gl);
            normalBuffer.SetData(gl, normalAttributeLocation, normals.SelectMany(v => v.to_array()).ToArray(), false, 3);        
        }

        private void CreateIndexBuffer(OpenGL gl)
        {
            const uint vertexCount = slices * stacks;
            const uint indexCount = vertexCount * 6;
            indices = new ushort[indexCount];
            int count = 0;

            ushort n = 0;
            for (ushort i = 0; i < slices; i++)
            {
                for (ushort j = 0; j < stacks; j++)
                {
                    indices[count++] = (ushort)(n + j);
                    indices[count++] = (ushort)(n + (j + 1) % stacks);
                    indices[count++] = (ushort)((n + j + stacks) % vertexCount);

                    indices[count++] = (ushort)((n + j + stacks) % vertexCount);
                    indices[count++] = (ushort)((n + (j + 1) % stacks) % vertexCount);
                    indices[count++] = (ushort)((n + (j + 1) % stacks + stacks) % vertexCount);
                }

                n += (ushort)stacks;
            }

            var indexBuffer = new IndexBuffer();
            indexBuffer.Create(gl);
            indexBuffer.Bind(gl);
            indexBuffer.SetData(gl, indices);
        }

        private vec3[] vertices;
        private vec3[] normals;
        private ushort[] indices;

        /// The number of slices and stacks.
        private const uint slices = 128;
        private const uint stacks = 32;

        //  The vertex buffer array that handles the state of all vertex buffers.
        private VertexBufferArray vertexBufferArray;

        /// <summary>
        /// Gets the vertex buffer array.
        /// </summary>
        public VertexBufferArray VertexBufferArray
        {
            get { return vertexBufferArray; }
        }

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        public vec3[] Vertices
        { 
            get { return vertices; } 
        }

        /// <summary>
        /// Gets the normals.
        /// </summary>
        public vec3[] Normals
        {
            get { return normals; }
        }

        /// <summary>
        /// Gets the indices.
        /// </summary>
        public ushort[] Indices
        {
            get { return indices; }
        }
    }
}
