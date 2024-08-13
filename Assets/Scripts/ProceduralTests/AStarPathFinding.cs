using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    // Clase Nodo
    private class Nodo
    {
        public Vector3Int posicion;
        public Nodo padre;
        public int gCost; // Costo desde el inicio hasta este nodo
        public int hCost; // Costo estimado desde este nodo hasta el destino
        public int FCost => gCost + hCost; // Costo total

        public Nodo(Vector3Int posicion)
        {
            this.posicion = posicion;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Nodo))
                return false;

            return posicion.Equals(((Nodo)obj).posicion);
        }

        public override int GetHashCode()
        {
            return posicion.GetHashCode();
        }
    }

    // Función principal que calcula el camino más corto
    public List<Vector3Int> CalculateShortestPath(Vector3Int posA, Vector3Int posB, Func<Vector3Int, Vector3Int, bool> IsPositionInUse)
    {
        List<Nodo> listaAbierta = new List<Nodo>();
        HashSet<Nodo> listaCerrada = new HashSet<Nodo>();
        Nodo nodoInicio = new Nodo(posA);
        Nodo nodoObjetivo = new Nodo(posB);

        listaAbierta.Add(nodoInicio);

        while (listaAbierta.Count > 0)
        {
            Nodo nodoActual = listaAbierta[0];
            for (int i = 1; i < listaAbierta.Count; i++)
            {
                if (listaAbierta[i].FCost < nodoActual.FCost ||
                    (listaAbierta[i].FCost == nodoActual.FCost && listaAbierta[i].hCost < nodoActual.hCost))
                {
                    nodoActual = listaAbierta[i];
                }
            }

            listaAbierta.Remove(nodoActual);
            listaCerrada.Add(nodoActual);

            if (nodoActual.posicion == nodoObjetivo.posicion)
            {
                return ReconstruirCamino(nodoInicio, nodoActual);
            }

            foreach (Vector3Int vecinoPos in ObtenerVecinos(nodoActual.posicion))
            {
                if (IsPositionInUse(vecinoPos, posB) || listaCerrada.Contains(new Nodo(vecinoPos)))
                {
                    continue;
                }

                Nodo vecino = new Nodo(vecinoPos);
                int nuevoCostoMovimiento = nodoActual.gCost + CalcularHeuristica(nodoActual.posicion, vecino.posicion);
                if (nuevoCostoMovimiento < vecino.gCost || !listaAbierta.Contains(vecino))
                {
                    vecino.gCost = nuevoCostoMovimiento;
                    vecino.hCost = CalcularHeuristica(vecino.posicion, nodoObjetivo.posicion);
                    vecino.padre = nodoActual;

                    if (!listaAbierta.Contains(vecino))
                    {
                        listaAbierta.Add(vecino);
                    }
                }
            }
        }

        return null; // No se encontró camino
    }

    // Función para calcular la heurística (distancia de Manhattan)
    private int CalcularHeuristica(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }

    // Función para obtener los vecinos (posiciones adyacentes)
    private List<Vector3Int> ObtenerVecinos(Vector3Int nodoPosicion)
    {
        List<Vector3Int> vecinos = new List<Vector3Int>();

        vecinos.Add(nodoPosicion + Vector3Int.down);
        vecinos.Add(nodoPosicion + Vector3Int.up);
        vecinos.Add(nodoPosicion + Vector3Int.right);
        vecinos.Add(nodoPosicion + Vector3Int.left);
        vecinos.Add(nodoPosicion + Vector3Int.forward);
        vecinos.Add(nodoPosicion + Vector3Int.back);


        return vecinos;
    }

    // Función para reconstruir el camino desde el nodo final hasta el inicio
    private List<Vector3Int> ReconstruirCamino(Nodo nodoInicio, Nodo nodoFinal)
    {
        List<Vector3Int> camino = new List<Vector3Int>();
        Nodo nodoActual = nodoFinal;

        while (nodoActual != nodoInicio)
        {
            camino.Add(nodoActual.posicion);
            nodoActual = nodoActual.padre;
        }
        camino.Add(nodoInicio.posicion);
        camino.Reverse();
        return camino;
    }

    
    
}