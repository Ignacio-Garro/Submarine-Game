using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class submarineCollisionController : NetworkBehaviour
{
    

    [SerializeField] SubmarineController controller;
    [SerializeField] Transform submarineCentre;
    List<Collision> currentCollisions = new List<Collision>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;
        if (HasSharedParent(transform, collision.transform)) return;
        currentCollisions.Add(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (HasSharedParent(transform, collision.transform)) return;
        currentCollisions.Remove(collision);
    }

    public void MakeDownMovementWithCollisions(float distance)
    {
        controller.transform.position += Vector3.up * distance;
        
        /*Vector3 pointA = Vector3.zero;
        Vector3 pointB = Vector3.zero;
        float minDistance = float.PositiveInfinity;
        foreach (Collision collision in currentCollisions)
        {
            foreach(ContactPoint point1 in collision.contacts)
            {
                if (point1.normal.z < 0) continue;
                if (pointA == Vector3.zero) pointA = point1.point;
                foreach (ContactPoint point2 in collision.contacts)
                {
                    if (point2.normal.z < 0 || point1.point == point2.point) continue;
                    float dist = CalculateDistanceToLine(point1.point, point2.point, submarineCentre.position);
                    if (dist < minDistance)
                    {
                        Vector3 averagetemp = (point1.point + point2.point) / 2;
                        Vector3 dirABtemp = point1.point - point2.point;
                        dirABtemp.Normalize();
                        Vector3 dirABPerpendiculartemp = new Vector3(dirABtemp.y, -dirABtemp.x, 0);
                        Vector3 dirtemp = submarineCentre.position - averagetemp;
                        bool yes = true;
                        if (Vector3.Dot(dirABPerpendiculartemp, dirtemp) < 0)
                        {
                            dirABPerpendiculartemp *= -1;
                        }
                        foreach (Collision collisiontemp in currentCollisions)
                        {
                            
                            foreach (ContactPoint point in collisiontemp.contacts)
                            {
                                if (point.normal.z < 0 || point.point == point1.point || point.point == point2.point) continue;
                                if (Vector3.Dot(dirABPerpendiculartemp, point.point - averagetemp) > 0) yes = false;
                            }
                        }
                        if (yes)
                        {
                            minDistance = dist;
                            pointA = point1.point;
                            pointB = point2.point;
                        }
                        
                    }
                }
            }
        }
        
        if(pointA == Vector3.zero && pointB == Vector3.zero)
        {
            controller.transform.GetComponent<Rigidbody>().MovePosition(controller.transform.position + Vector3.up * distance);
            return;
        }
        else if(pointB == Vector3.zero)
        {
            Vector3 dirA = submarineCentre.position - pointA;
            Vector3 perpendicularA = new Vector3(dirA.x, -dirA.y, 0);
            perpendicularA.Normalize();
            pointB = dirA + perpendicularA;
        }

        Vector3 average = (pointA + pointB) / 2;
        Vector3 dir = submarineCentre.position - average;
        dir.z = 0;
        dir.Normalize();

        
        Vector3 dirAB = pointA - pointB;
        dirAB.Normalize();
        Vector3 dirABPerpendicular = new Vector3(dirAB.y, -dirAB.x, 0);
        float angle = distance * 360 / (controller.SubmarineLength * 0.5f * 2 * Mathf.PI);
        if(Vector3.Dot(dirABPerpendicular, dir) > 0)
        {
            dirABPerpendicular *= -1;
            angle = -angle;
        }

        
        RotateObjectAroundLine(controller.transform, pointA, pointB, angle);
        */
    }

    bool HasSharedParent(Transform a, Transform b)
    {
        // Crear una lista de todos los padres del primer transform
        var parentsA = new System.Collections.Generic.HashSet<Transform>();
        while (a != null)
        {
            parentsA.Add(a);
            a = a.parent;
        }

        // Recorrer los padres del segundo transform y comprobar si alguno está en la lista
        while (b != null)
        {
            if (parentsA.Contains(b))
            {
                return true; // Encontramos un padre compartido
            }
            b = b.parent;
        }

        // Si llegamos aquí, no hay padres compartidos
        return false;
    }

    void RotateObjectAroundLine(Transform obj, Vector3 A, Vector3 B, float angle)
    {
        // Calcula el vector AB
        Vector3 AB = B - A;
        // Normaliza el vector para obtener el eje de rotación unitario
        Vector3 axis = AB.normalized;

        // Crea la rotación usando Quaternion.AngleAxis
        Quaternion rotation = Quaternion.AngleAxis(angle, axis);

        // Aplica la rotación al objeto
        obj.RotateAround(A, axis, angle);
    }


    float CalculateDistanceToLine(Vector3 A, Vector3 B, Vector3 P)
    {
        Vector3 AB = B - A;
        Vector3 AP = P - A;
        float AB_squared = Vector3.Dot(AB, AB);
        float AP_dot_AB = Vector3.Dot(AP, AB);
        Vector3 projection = A + (AP_dot_AB / AB_squared) * AB;
        return Vector3.Distance(P, projection);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
