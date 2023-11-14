using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillController : MonoBehaviour
{
    public GameObject log; 
    public float drillSpeed = 0.5f; // Speed at which the log is drilled
    public GameObject sawdustParticles;

    public float particleDuration = 3.0f; //Duration for which the particle effect prevails on every click

    private Mesh logMesh;
    private Vector3[] originalVertices; // Store the original vertices of the log mesh

    void Start()
    {
        logMesh = log.GetComponent<MeshFilter>().mesh;
        originalVertices = logMesh.vertices;
        sawdustParticles.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for a left mouse click (checks if ray hits the log)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == log)
            {
                Vector3 drillPoint = hit.point; // Fetch the point where the drill will happen
                StartCoroutine(DrillCoroutine(drillPoint));
            }
        }
    }

    IEnumerator DrillCoroutine(Vector3 drillPoint)
    {
        // Resetting the mesh vertices to their original state
        logMesh.vertices = originalVertices;
        logMesh.RecalculateNormals();
        logMesh.RecalculateBounds();

        sawdustParticles.SetActive(true);

        // Timer to turn off the particle system after particleDuration seconds.
        StartCoroutine(StopParticleEffect());

        while (Vector3.Distance(log.transform.position, drillPoint) > 0.01f)
        {
            Drill(log.transform.InverseTransformPoint(drillPoint));
            yield return null;
        }

        sawdustParticles.SetActive(false);
    }

    IEnumerator StopParticleEffect()
    {
        yield return new WaitForSeconds(particleDuration);
        sawdustParticles.SetActive(false);
    }

    void Drill(Vector3 drillPoint)
    {
        Vector3[] vertices = logMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            float distance = Vector3.Distance(vertices[i], drillPoint);
            if (distance < 0.3f) // This threshold controls drilling radius.
            {
                vertices[i] -= vertices[i].normalized * drillSpeed * Time.deltaTime;
            }
        }

        logMesh.vertices = vertices;
        logMesh.RecalculateNormals();
        logMesh.RecalculateBounds();
    }
}
