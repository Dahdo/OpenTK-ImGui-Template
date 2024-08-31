using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using static TemplateProject.WireframeCube;

namespace TemplateProject;

public class WireframeCube : IDisposable
{
    public Matrix4 ModelMatrix { get; set; }
    public Mesh Mesh { get; }

    public WireframeCube()
    {
        Vertex[] vertices;
        byte[] indices;
        CylinderMesh.GenerateCylinderMesh(0.8f, 3f, 10, out vertices, out indices);


        // Calculate face normals and accumulate them to vertex normals
        for (int i = 0; i < indices.Length; i += 3)
        {
            int i1 = indices[i];
            int i2 = indices[i + 1];
            int i3 = indices[i + 2];

            Vector3 v1 = vertices[i1].Position;
            Vector3 v2 = vertices[i2].Position;
            Vector3 v3 = vertices[i3].Position;

            // Edge vectors
            Vector3 edge1 = v2 - v1;
            Vector3 edge2 = v3 - v1;

            // Cross product to find the face normal
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


public class CylinderMesh
{
    public static void GenerateCylinderMesh(float radius, float height, int slices, out Vertex[] vertices, out byte[] indices)
    {
        // List to hold the vertices
        List<Vertex> vertexList = new List<Vertex>();

        // Center vertices for top and bottom
        vertexList.Add(new Vertex(new Vector3(0.0f, height / 2.0f, 0.0f)));  // Top center
        vertexList.Add(new Vertex(new Vector3(0.0f, -height / 2.0f, 0.0f))); // Bottom center

        // Calculate the vertices around the circumference for top and bottom
        for (int i = 0; i <= slices; i++)
        {
            float angle = 2.0f * MathF.PI * i / slices;
            float x = MathF.Cos(angle) * radius;
            float z = MathF.Sin(angle) * radius;

            // Top circle vertices
            vertexList.Add(new Vertex(new Vector3(x, height / 2.0f, z)));

            // Bottom circle vertices
            vertexList.Add(new Vertex(new Vector3(x, -height / 2.0f, z)));
        }

        // List to hold the indices
        List<byte> indexList = new List<byte>();

        // Top circle indices
        byte topCenterIndex = 0;

        for (int i = 2; i < vertexList.Count; i += 2)
        {
            indexList.Add(topCenterIndex);
            indexList.Add((byte)i);
            indexList.Add((byte)(i % (vertexList.Count - 2) + 2));
        }

        // Bottom circle indices
        byte buttonCenterIndex = 1;
        for (int i = 3; i < vertexList.Count; i += 2)
        {
            indexList.Add(buttonCenterIndex);
            indexList.Add((byte)i);
            indexList.Add((byte)((i - 1) % (vertexList.Count - 2) + 3));
        }

        // Side indices
        for (int i = 2; i < vertexList.Count; i += 2)
        {
            indexList.Add((byte)i);
            indexList.Add((byte)(i + 1));

            if (i == vertexList.Count - 2)
                indexList.Add(2);
            else
                indexList.Add((byte)(i + 2));
        }

        for (int i = 3; i < vertexList.Count; i += 2)
        {
            indexList.Add((byte)i);
            if (i >= vertexList.Count - 2)
            {
                indexList.Add(2);
                indexList.Add(3);
            }
            else
            {
                indexList.Add((byte)(i + 1));
                indexList.Add((byte)(i + 2));
            }
        }

        // Convert lists to arrays
        vertices = vertexList.ToArray();
        indices = indexList.ToArray();
    }
}