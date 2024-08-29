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
        CylinderMesh.GenerateCylinderMesh(1.0f, 3.0f, 12, out vertices, out indices);



        var indexBuffer = new IndexBuffer(indices, indices.Length * sizeof(byte),
            DrawElementsType.UnsignedByte, indices.Length);
        var vertexBuffer = new VertexBuffer(vertices, vertices.Length * Marshal.SizeOf<Vertex>(),
            vertices.Length, BufferUsageHint.StaticDraw,
            new VertexBuffer.Attribute(0, 3) /*positions*/);
        Mesh = new Mesh(PrimitiveType.Triangles, indexBuffer, vertexBuffer);
        ModelMatrix = Matrix4.Identity;
    }

    public struct Vertex
    {
        public Vector3 Position;

        public Vertex(Vector3 position)
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
            if (i == vertexList.Count - 2)
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