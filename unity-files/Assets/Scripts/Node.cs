using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Clase nodo donde se encuentra la configuracion del nodo doble para la creación del arbol sintatico</summary>
public class Node
{
    // Start is called before the first frame update
    /// <summary>  Liga izquierda</summary>
    Node li;
    /// <summary>  Liga derecha</summary>
    Node ld;
    /// <summary>The value
    /// of the node</summary>
    string value;

    /// <summary>  The enumation of the node</summary>
    int label;
    /// <summary>Si es nulable o no</summary>
    bool nullable;
    /// <summary>  Arreglo de firstPos</summary>
    public List<int> firstPos = new List<int>();
    /// <summary>  Arreglo LastPos</summary>
    public List<int> lastPos = new List<int>();


    /// <summary>Initializes a new instance of the <see cref="Node"/> class.</summary>
    /// <param name="d">The ddata</param>
    public Node(string d) //Constructor
    {
        li = null;
        ld = null;
        this.value = d;
        this.label = 0;
        this.nullable = false;
    }




    /// <summary>Asignas el dato.</summary>
    /// <param name="d">The d.</param>
    public void asignaDato(string d)
    {
        value = d;
    }
    /// <summary>Retornas el dato.</summary>
    /// <returns></returns>
    public string retornaDato() //obtener values
    {
        return value;
    }



    /// <summary>Asignalis li specified x.</summary>
    /// <param name="x">The x.</param>
    public void asignali(Node x) //asignar liga izquierda
    {
        li = x;
    }
    /// <summary>Asigna liga derecha en specified x.</summary>
    /// <param name="x">  Nodo a asignar</param>
    public void asignald(Node x) //asignar liga derecha
    {
        ld = x;
    }

    /// <summary>Retorna liga izquierda</summary>
    /// <returns>Liga izquierda</returns>
    public Node retornali() //retorna liga izquierda
    {
        return li;
    }

    /// <summary>  retorna liga derecha</summary>
    /// <returns></returns>
    public Node retornald() //retorna liga derecha
    {
        return ld;
    }

    /// <summary>Asignas the label.</summary>
    /// <param name="label">The label.</param>
    public void asignaLabel(int label)
    {
        this.label = label;
    }

    /// <summary>Retornas the label.</summary>
    /// <returns></returns>
    public int retornaLabel()
    {
        return this.label;
    }
    /// <summary>Asignas the nullable.</summary>
    /// <param name="nullable">if set to <c>true</c> [nullable].</param>
    public void asignaNullable(bool nullable)
    {
        this.nullable = nullable;
    }

    /// <summary>Retornas the nullable.</summary>
    /// <returns>Node to return</returns>
    public bool retornaNullable()
    {
        return this.nullable;
    }

    /// <summary>Shows the array of  first post.</summary>
    /// <returns>firstPost array</returns>
    public string ShowFirstPost()
    {
        string aux = "";
        foreach (var item in firstPos)
        {
            aux += item.ToString();
        }

        return aux;
    }

    /// <summary>Shows the last post array</summary>
    /// <returns>last post array</returns>
    public string ShowLastPost()
    {
        string aux = "";
        foreach (var item in lastPos)
        {
            aux += item.ToString();
        }

        return aux;
    }


}
