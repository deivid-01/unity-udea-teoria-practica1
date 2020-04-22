using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// <summary>Clase que controla el ciclo principal de programa</summary>
public class GameController : MonoBehaviour
{


    /// <summary> Arreglo con cada una de las ventanas</summary>
    public GameObject[] pages = new GameObject[4];
    #region Input regex
    /// <summary> Expresion regular ingresada</summary>
    [Header("Input Regular Exp Page")]

    public Text regex;

    #endregion

    #region Loading Page
    /// <summary> Mensaje de cargando</summary>
    [Header("Loading Page")]

    public Text message;
    #endregion

    #region Input Hilera

    /// <summary>
    ///   <para>
    ///  Elemento UI de entrada de datos</para>
    ///   <para></para>
    /// </summary>
    [Header("Input Hilera")]

    public GameObject inputPackage;
    /// <summary>  Elementos de UI de salida de datos</summary>
    public GameObject resultPackage;


    /// <summary>  Tabla con el automata finito</summary>
    public Rows[] table = new Rows[10];
    /// <summary>  Filas del automata</summary>
    public GameObject[] rowsTable;

    /// <summary>
    ///   <para>
    ///  Hilera ingresada</para>
    /// </summary>
    public InputField inputHilera;
    /// <summary>  Resultado si la expresión fue aceptada o no</summary>
    public Text result;





    #endregion
    #region Instructions
    /// <summary>Boton en instrucciones para devolverse de ventana</summary>
    [Header("Instructions")]
 

    public GameObject nextLeft;
    /// <summary>Boton en instrucciones para pasar a la siguiente  ventana</summary>
    public GameObject nextRight;
    // public Button returnPage;

    #endregion

    #region Gargabe Section
    //public Text regex;
    //string regex = "(a|b)*.a.b.b";////"(a|b)*.a.b.b";
    /// <summary>The actual page</summary>
    int actualPage = -1;
    /// <summary>The message loading</summary>
    string messageLoading = "Building...";
    /// <summary>
    ///   <para>
    ///  Variable auxiliar</para>
    ///   <para></para>
    /// </summary>
    int p = 0;
    /// <summary>  Variable auxiliar</summary>
    int auxi = -1;

    #endregion


    /// <summary>  The syntatic tree</summary>
    static SyntaticTree syntaticTree;
    /// <summary>
    ///   <para>
    /// Automata finito</para>
    ///   <para></para>
    /// </summary>
    DFA dfa;
    //-------ERRORES----------
    /*
     "b.a.(a|b)*.a.b";
      "(0|1.0*.1)*.0*"   
         
         
         */

    public GameController()
    {
    }
    /// <summary>Starts this program.</summary>
    void Start()
    {
        EnableTab(0);       
        
    }




    /// <summary>Enables the tab.</summary>
    /// <param name="x">
    ///   <para>
    ///  La x es la ventana que deseo abrir</para>
    ///   <para></para>
    /// </param>
    public void EnableTab(int x)
    {
        foreach (var page in pages)
        {
            page.SetActive(false);
        }

        pages[x].SetActive(true);


        #region Special cases
        if (x == 1 && actualPage!=x)
        {
            nextLeft.SetActive(false);
        }
        else if (x == 3  && actualPage != x)
        {
            nextRight.SetActive(false);
        }




        if (actualPage == 5 && x == 0)
        {
            SceneManager.LoadScene("MainScene");
        }
        else if (x == 5)
        {
            DisableObjects(inputPackage, resultPackage);

            inputHilera.Select();
            inputHilera.text = "";
        }
            #endregion

            actualPage = x;
    }


    /// <summary>  Crear arbol sintatico y autoamta finito con la expresion regular ingresada</summary>
    public void Built()
    {
      
        syntaticTree = new SyntaticTree(regex.text);
        dfa = new DFA(syntaticTree.root);
        ShowTable();

        EnableTab(4);
        
        StartCoroutine(BuildingLoading());

    }

    /// <summary>  Verifica la hilera ingresada luego de crear la expresión regular</summary>
    public void Check()
    {
 
        result.text=(dfa.ReadInput(inputHilera.text));

        DisableObjects(resultPackage, inputPackage);
    }

    /// <summary>
    ///   <para>
    ///  Cierra el programa</para>
    ///   <para></para>
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary> Genera otro hilo de ejecución</summary>
    /// <returns></returns>
    IEnumerator BuildingLoading()
    {
       StartCoroutine(MessageLoading());
           
        
        

        yield return new WaitForSeconds(2);

        EnableTab(5);
        DisableObjects(inputPackage, resultPackage);
    }

    /// <summary>  Genera otro hilo de ejecucion para mostrar mensaje de carga palabra a palabra</summary>
    /// <returns></returns>
    IEnumerator MessageLoading()
    {

        while (true)
        {
            if (p < messageLoading.Length)
                message.text += messageLoading.Substring(p, 1);

            else
            {
                p = -1;
                message.text = "";
            }

            yield return new WaitForSeconds(0.1f);
            p++;
        }
        
        
    }

    /// <summary>Gets the syntatic tree.</summary>
    /// <returns>Syntatic tree</returns>
    public SyntaticTree GetSyntaticTree()
    {
        return syntaticTree;
    }

    /// <summary>Disables elements on Ui.</summary>
    /// <param name="A">a.
    /// Elemento que deseo desactivar</param>
    /// <param name="B">
    ///   <para>
    /// Elemento que deseo activar</para>
    ///   <para></para>
    /// </param>
    public void DisableObjects(GameObject A,GameObject B)
    {
        if (!A.activeInHierarchy)
        {
            A.SetActive(true);
        }

        if (B.activeInHierarchy)
        {
            B.SetActive(false);
        }

      
    }

    /// <summary>  Muestra la tabla del automata finito</summary>
    void ShowTable()
    {
        int rows=dfa.GetTable().GetLength(0);
        int cols = dfa.GetTable().GetLength(1);

        EnableGrid(rows, cols);

    }

    /// <summary>  Activa las casillas necesarias para mostrar la tabla del automata generado</summary>
    /// <param name="rows">The rows.</param>
    /// <param name="cols">The cols.</param>
    void EnableGrid(int rows, int cols)
    {
        Debug.Log("table: "+rows + " | rows: " + cols);
        for (int i = 0; i < rows+1; i++)
        {
            for (int j = 0; j < cols+1; j++)
            {

               
                table[i].rows[j].SetActive(true);
                if (i == 0)
                {
                    //asignar valores de entrada
                    if(j>=1)
                        table[i].rows[j].transform.GetChild(0).gameObject.GetComponent<Text>().text = dfa.inputSymbols[j-1];
                    continue;
                }

                else if (j ==0 )
                {
                    //asignar valores de los estados;
                    if (i >= 1)
                    { table[i].rows[j].transform.GetChild(0).gameObject.GetComponent<Text>().text = dfa.states[i - 1].name.ToString();
                        if (dfa.states[i - 1].approval)
                        {
                            table[i].rows[j].GetComponent<Image>().color = Color.red;
                        }
                    }
                    continue;

                }
                //ASignar valores del dfa
                table[i].rows[j].transform.GetChild(0).gameObject.GetComponent<Text>().text = dfa.GetTable()[i-1, j-1].ToString();
            }
        }

        //disable Rows box
        {
            for (int j = rows+1; j < rowsTable.Length; j++)
            {

                rowsTable[j].SetActive(false);
            }
        }

    }




}
   