using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using static TemplateProject.CylinderShade;

namespace TemplateProject;

public class CylinderShade : IDisposable
{
    public Matrix4 ModelMatrix { get; set; }
    public Mesh Mesh { get; }

    public CylinderShade()
    {
        Vertex[] vertices;
        byte[] indices;
        CylinderTriangularMesh.GenerateMesh(0.8f, 3.0f, 36, out vertices, out indices);


        // Calculate face normals and accumulate them to vertex normals
        for (int i = 0; i < indices.Length; i += 3)
        {
            int i1 = indices[i];
            int i2 = indices[i + 1];
            int i3 = indices[i + 2];

            Vector3 v1 = vertices[i1].Position;
            Vector3 v2 = vertices[i2].Position;
            Vector3 v3 = vertices[i3].Position;

            Vector3 edge1 = v2 - v1;
            Vector3 edge2 = v3 - v1;

            Vector3 faceNormal = Vector3.Cross(edge1, edge2);

            // Accumulate the face normal to each vertex normal
            vertices[i1].Normal += faceNormal;
            vertices[i2].Normal += faceNormal;
            vertices[i3].Normal += faceNormal;
        }

        // Normalize the vertex normals
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].Normal = Vector3.Normalize(vertices[i].Normal);
        }


        var indexBuffer = new IndexBuffer(indices, indices.Length * sizeof(byte),
            DrawElementsType.UnsignedByte, indices.Length);
        var positionAttribute = new VertexBuffer.Attribute(0, 3); // Position attribute
        var normalAttribute = new VertexBuffer.Attribute(1, 3);   // Normal attribute
        var vertexBuffer = new VertexBuffer(vertices, vertices.Length * Marshal.SizeOf<Vertex>(),
            vertices.Length, BufferUsageHint.StaticDraw, positionAttribute, normalAttribute);
        Mesh = new Mesh(PrimitiveType.Triangles, indexBuffer, vertexBuffer);
        ModelMatrix = Matrix4.Identity;
    }

    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;

        public Vertex(Vector3 position, Vector3 normal = default(Vector3))
        {
            Position = position;
        }
    }


    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}