using static TemplateProject.CylinderShade;
using OpenTK.Mathematics;

public class CylinderTriangularMesh
{
    public static void GenerateMesh(float radius, float height, int slices, out Vertex[] vertices, out byte[] indices)
    {
        List<Vertex> vertexList = new List<Vertex>();

        // The vertices around the circumference for top and bottom
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

        // Center vertices for top and bottom
        vertexList.Add(new Vertex(new Vector3(0.0f, height / 2.0f, 0.0f)));
        vertexList.Add(new Vertex(new Vector3(0.0f, -height / 2.0f, 0.0f)));

        // List to hold the indices
        List<byte> indexList = new List<byte>();

        byte topCenterIndex = (byte)(vertexList.Count - 2);
        byte bottomCenterIndex = (byte)(vertexList.Count - 1);


        for (byte i = 0; i < slices * 2; i += 2)
        {
            // First triangle
            indexList.Add(i);
            indexList.Add((byte)((i + 1) % (slices * 2)));
            indexList.Add((byte)((i + 2) % (slices * 2)));

            // Second triangle
            indexList.Add((byte)((i + 2) % (slices * 2)));
            indexList.Add((byte)((i + 1) % (slices * 2)));
            indexList.Add((byte)((i + 3) % (slices * 2)));
        }

        // Indices for the top
        for (byte i = 0; i < slices * 2; i += 2)
        {
            indexList.Add(topCenterIndex);
            indexList.Add(i);
            indexList.Add((byte)((i + 2) % (slices * 2)));
        }

        // Indices for the bottom
        for (byte i = 1; i < slices * 2; i += 2)
        {
            indexList.Add(bottomCenterIndex);
            indexList.Add((byte)((i + 2) % (slices * 2)));
            indexList.Add(i);
        }

        // Convert lists to arrays
        vertices = vertexList.ToArray();
        indices = indexList.ToArray();
    }
}