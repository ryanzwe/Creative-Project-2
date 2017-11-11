using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{
    void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int i = 0;
        while (i < vertices.Length)
        {
            vertices[i] += Vector3.up * Time.deltaTime;
            i++;
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }
}