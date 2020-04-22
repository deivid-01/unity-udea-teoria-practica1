using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Clase que crea  y contiene el arbol sintatico  donde se encuentra la expresión regular</summary>
public class SyntaticTree 
{
    /// <summary>Estructura para controlar un objecto con varios valores ( más practico que realizar una clase)</summary>
    struct SuperString
    {
        public string data;
        public string side;
    }
    /// <summary>  Expresion regular</summary>
    string regex;

    /// <summary>  Pila principal para constuir el arbol sintatico</summary>
    public Stack stack = new Stack();
    /// <summary>  Pila para guardar ultimo node y asi facilitar la construccion del arbol sintactico</summary>
    public Stack fatherStack = new Stack();
    /// <summary>Pila de basura, donde voy a sacar datos cada vez que la pila principal stack, se quede sin elementos</summary>
    public Stack garbage = new Stack();
    /// <summary>  Pila auxiliar para pasar elementos de una pila a otra</summary>
    public Stack cache = new Stack();
    /// <summary>
    ///   <para>
    ///  El nodo ultimo, es el nodo más externo del arbol</para>
    ///   <para></para>
    /// </summary>
    [Header("Aux Variables")]
    public Node ultimo;
    /// <summary>  Arreglo para almecer las siguientesPosiciones de  un estado a otro</summary>
    public List<List<int>> followPos;
    /// <summary>  Las hojas del arbol</summary>
    public List<string> leaves = new List<string>();
    /// <summary>  El nodo raiz</summary>
    public Node root;
    /// <summary>  Contador del numero de hojas</summary>
    public int cont=1;

    /// <summary>  Varfiable auxiliar</summary>
    public string aux = "";
    /// <summary>Initializes a new instance of the <see cref="SyntaticTree"/> class.</summary>
    /// <param name="regex">The regular expression.</param>
    public SyntaticTree(string regex)
    {
        this.regex = regex;
        this.root = null;
        CreateSyntaticTree();
        Package();
        //At this moment Syntatic tree
        //has been created 
        //--------------
        //Nodes leaf
        inordenSetValuesLeaf(root);
        NullableLeaf(root); 
        FirstPosLef(root);
        LasPosLef(root);
        //----------------
        //Nodes "|","*",".","+"
        NullableSymbol(root); 
        FirstPosSymbol(root);
        LasPosSymbol(root);
        //Built--FollowPost
        
        followPos = new List<List<int>>(cont-1);
        
       
        for (int i = 0;i < cont - 1; i++)
        {
            List<int> a = new List<int>();
            a.Add(i);
            followPos.Add(a);
        }
        FollowPos(root);
        for (int i = 0; i < followPos.Count; i++)
        {
            followPos[i].RemoveAt(0);
        }
        
        //Debug--Delete me later
        //inorden(root);
    }

    /// <summary>  Recorrido inorden del arbol sintatico</summary>
    /// <param name="r">  Nodo inicial</param>
    public void inorden(Node r)
    {
        if (r is null == false)
        {
            inorden(r.retornali());
            inorden(r.retornald());
            /*
          Debug.Log("Node: " + r.retornaDato() +
                 " | Nullable: " + r.retornaNullable()+
                 " | FirstPos: " + r.ShowFirstPost() +
                 " | LastPos: " + r.ShowLastPost() );


            */
           // Debug.Log(r.retornaDato());


        }
    }

    #region Function to Create SyntaticTree
    /// <summary>Creates the syntatic tree.</summary>
    void CreateSyntaticTree()
    {
        
        ultimo = new Node(".");
        ultimo.asignald(new Node("#"));
        ultimo.asignali(new Node("root_son"));
        root = ultimo;
        ultimo = root.retornali();
    }
    /// <summary>  Empaqueta las expresiones ingresadas para facilida de procesamiento de los datos al crear el arbol sintatico</summary>
    void Package()
    {
        SearchBrackeysAsterik(regex.Length - 1);
        SendToGarbage();
        SearchUnionConjuntion('|');
        SearchUnionConjuntion('.');
        BuiltSyntaticTree();
        if (fatherStack.Count == 0 && stack.Count == 0 && garbage.Count == 0)
        {
            return;
        }
        Unpackage();
        if (regex.Equals("?"))
        {
            return;
        }
        Package();
    }
    /// <summary>  Desempaqueta lo que se haya mandado a la pila, para procesar cada expresión por aparte.</summary>
    void Unpackage()
    {
        #region 1. Stack no empty 
        if (stack.Count == 0)
        {
            if (garbage.Count > 0)
            {
                regex = (string)garbage.Pop();
            }
            else
            {
                regex = "?";
                return;
            }
        }
        else
        {
            SuperString aux = (SuperString)stack.Peek();
            regex = aux.data;
        }
        #endregion
        #region 2. Upload ultimo node
        if (!(ultimo.retornald() is null))
        {
            if (ultimo.retornald().retornaDato().Equals("X") || ultimo.retornald().retornaDato().Equals("Y"))
            {
                SearchSide("right");
            }
            else
                SearchSide("left");
        }
        if (stack.Count > 0)
            stack.Pop();
        #endregion
        #region 3. Check last index "*"or "+"
        BuiltSyntaticTreePlusAsteric("*");
        BuiltSyntaticTreePlusAsteric("+");
        #endregion
        #region Delete brackeys(if exists)
        DeleteBrackeysAndOtherShit();
        #endregion
    }
    /// <summary>Replaces the first string founded</summary>
    /// <param name="text">The text to replace</param>
    /// <param name="search">The string to search</param>
    /// <param name="replace">  Replace text with this variable</param>
    /// <returns>Return the new regex</returns>
    string ReplaceFirst(string text, string search, string replace)
    {
        int pos = text.IndexOf(search);
        if (pos < 0)
            return text;
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }
    /// <summary>Searches the brackeys and asterik in regex.</summary>
    /// <param name="i"> Index to get each character of the array</param>
    void SearchBrackeysAsterik(int i)
    {
        bool upload = false;
        if (i > 0)
        {
            if (regex[i].Equals('*') || regex[i].Equals('+'))
            {
                string X;
                if (regex[i - 1].Equals(')'))
                { //(##)* //Case for brackeys
                    int leftP = 0;
                    int rightP = 0;
                    int endIndex = -1;
                    int startIndex = -1;
                    #region Get startIndex and endIndex
                    for (int x = i - 1; x >= 0; x--)
                    {
                        if (regex[x].Equals(')'))
                        {
                            ++rightP;
                        }
                        else if (regex[x].Equals('('))
                        {
                            ++leftP;
                            if (leftP == rightP)
                            {
                                startIndex = x;
                                endIndex = i - 1;
                                break;
                            }
                        }
                    }
                    #endregion
                    #region Package
                    if (!startIndex.Equals(-1) && !endIndex.Equals(-1))
                    {
                        //Sub
                        X = regex.Substring(startIndex, (endIndex - startIndex) + 2);
                        //Send to garbage    
                        cache.Push(X);
                        //Upload
                        regex = ReplaceFirst(regex, X, "X");
                        upload = true;
                    }
                    else if (startIndex.Equals(-1) || endIndex.Equals(-1))
                    {
                        return;
                    }
                    #endregion
                }
                else
                { //##*
                    int j = 0;
                    for (int w = i - 1; w >= 0; w--)
                    {
                        if (regex[w].Equals('.') || regex[w].Equals('|'))
                        {
                            j = w + 1;
                            break;
                        }
                    }
                    #region Package
                    X = regex.Substring(j, i - j + 1);
                    // Send to Stack
                    cache.Push(X);
                    //Update 
                    regex = ReplaceFirst(regex, X, "X");
                    upload = true;
                    #endregion
                }
            }
            if (!upload)
            {
                SearchBrackeysAsterik(i - 1);
            }
            else
            {
                SearchBrackeysAsterik(regex.IndexOf('X') - 1);
            }
        }
        else
        {
            return;
        }
    }
    /// <summary>Searches the side if is right or left sequence</summary>
    /// <param name="str">The string.</param>
    void SearchSide(string str)
    {
        object[] array;
        if (stack.Count > 0)
        {
            array = stack.ToArray();
            for (int i = array.Length - 1; i >= 0; --i)
            {
                SuperString a = (SuperString)array[i];
                if (a.side.Equals(str))
                {
                    if (fatherStack.Count == 0)
                        return;
                    ultimo = (Node)fatherStack.Pop();
                    if (str.Equals("right"))
                    {
                        ultimo = ultimo.retornald();
                        return;
                    }
                    if (str.Equals("left"))
                    {
                        ultimo = ultimo.retornali();
                        return;
                    }
                }
            }
        }
        else if (garbage.Count > 0 || fatherStack.Count > 0)
        {
            ultimo = (Node)fatherStack.Pop();
            if (str.Equals("right"))
            {
                ultimo = ultimo.retornald();
                return;
            }
            if (str.Equals("left"))
            {
                ultimo = ultimo.retornali();
                return;
            }
        }
    }
    /// <summary>Builts the syntatic tree.</summary>
    void BuiltSyntaticTree()
    {
        //bool sendUltimotoStack=false;
        if (regex.Contains("|"))
        {
            ultimo.asignaDato("|");
        }
        else if (regex.Contains("."))
        {
            ultimo.asignaDato(".");
        }
        else
            return;
        Node newNode = new Node(regex.Substring(regex.IndexOf(ultimo.retornaDato()[0]) + 1, regex.Length - regex.IndexOf(ultimo.retornaDato()[0]) - 1));
        ultimo.asignald(newNode);
        //  Debug.Log("Right " + ultimo.retornald().retornaDato());
        newNode = new Node(regex.Substring(0, regex.IndexOf(ultimo.retornaDato()[0])));
        ultimo.asignali(newNode);
        //Debug.Log("Left " + ultimo.retornali().retornaDato());
        return;
    }
    /// <summary>Deletes the brackeys and other shit.</summary>
    void DeleteBrackeysAndOtherShit()
    {
        //Check + or *
        if (!regex[regex.Length - 1].Equals('+') && !regex[regex.Length - 1].Equals('*'))
        {
            return;
        }
        regex = regex.Substring(0, regex.Length - 1); // Delete + or *
        //Check if "(" exist
        if (regex.IndexOf('(') == -1)
        {
            return;
        }
        regex = regex.Substring(1, regex.Length - 2);
    }
    /// <summary>Builts the syntatic tree with plus asteric.</summary>
    /// <param name="str">The string.</param>
    void BuiltSyntaticTreePlusAsteric(string str)
    {
        if (regex[regex.Length - 1].Equals(str[0]))
        {
            #region Si la primera frase tiene * o #
            if (ultimo.retornaDato().Equals("root_son"))
            {
                ultimo.asignaDato(str);
                Node aux4 = new Node("asa");
                ultimo.asignali(aux4);
                ultimo = ultimo.retornali();
                if (!regex[0].Equals('('))
                {
                    ultimo.asignaDato(regex.Substring(0, regex.Length - 1));
                    if (fatherStack.Count > 0)
                    {
                        ultimo = (Node)fatherStack.Pop();
                        fatherStack.Push(ultimo);
                    }
                    Unpackage();
                }
                return;
            }
            #endregion
            //Crear nodo
            Node aux = new Node(str);
            Node aux2 = new Node("aux");
            //Ultimo lo conecto al nuevo nodo
            ultimo.asignaDato(str);
            ultimo.asignali(aux2);
            ultimo = ultimo.retornali();
            //Me muevo un paso mas profundo
            #region In case of ####*
            if (!regex[0].Equals('('))
            {
                ultimo.asignaDato(regex.Substring(0, regex.Length - 1));
                if (fatherStack.Count > 0)
                {
                    ultimo = (Node)fatherStack.Pop();
                    fatherStack.Push(ultimo);
                }
                Unpackage();
            }
            #endregion
        }
        return;
    }
    /// <summary>Searches the union and  conjuntion symbol</summary>
    /// <param name="kindOf">If it is "|" or "."</param>
    void SearchUnionConjuntion(char kindOf)
    {
        Stack buffer = new Stack();
        bool alreadyX = false;
        bool alreadyY = false;
        #region Inicialization
        int i = 0;
        SuperString X;
        SuperString Y;
        #endregion
        #region Verification
        if (regex.IndexOf(kindOf) == -1)
        { return; }
        #endregion
        #region Get X y Y (Substring)
        if (kindOf.Equals('|'))
        {
            X.data = regex.Substring(0, regex.IndexOf(kindOf));
            Y.data = regex.Substring(regex.IndexOf(kindOf) + 1, regex.Length - regex.IndexOf(kindOf) - 1);
        }
        else
        {
            X.data = regex.Substring(0, regex.LastIndexOf(kindOf));
            Y.data = regex.Substring(regex.LastIndexOf(kindOf) + 1, regex.Length - regex.LastIndexOf(kindOf) - 1);
        }
        X.side = "left";
        Y.side = "right"; //oliwis
        #endregion
        #region Send to stack and upload
        if (!(IsClean(X.data)))
        {
            stack.Push(X);
            fatherStack.Push(ultimo);
            regex = ReplaceFirst(regex, X.data, "X");
        }
        if (!(IsClean(Y.data)))
        {
            alreadyY = true;
            stack.Push(Y);
            fatherStack.Push(ultimo);
            regex = ReplaceFirst(regex, Y.data, "Y");
        }
        /*
        if (X.data.Equals("X") || X.data.Equals("Y"))
        {
            fatherStack.Push(ultimo);
            if (garbage.Count > 0)
            {
                SuperString aux;
                aux.data = (string)garbage.Pop();
                if (Y.data.Equals("X") || Y.data.Equals("Y"))
                    aux.side = Y.side;
                else
                    aux.side = X.side; 
                buffer.Push(aux);
            }
        }
        */
        if (Y.data.Equals("X") || Y.data.Equals("Y"))
        {
            fatherStack.Push(ultimo);
            if (garbage.Count > 0)
            {
                SuperString aux;
                aux.data = (string)garbage.Pop();
                aux.side = Y.side;
                buffer.Push(aux);
                if (X.data.Equals("X") || X.data.Equals("Y"))
                {
                    fatherStack.Push(ultimo);
                    if (garbage.Count > 0)
                    {
                        SuperString aux2;
                        aux2.data = (string)garbage.Pop();
                        aux2.side = X.side;
                        buffer.Push(aux2);
                    }
                    alreadyX = true;
                }
            }
        }
        if (alreadyX == false && (X.data.Equals("X") || X.data.Equals("Y")))
        {
            fatherStack.Push(ultimo);
            if (garbage.Count > 0 && alreadyY == false)
            {
                SuperString aux2;
                aux2.data = (string)garbage.Pop();
                aux2.side = X.side;
                buffer.Push(aux2);
            }
        }
        while (buffer.Count > 0)
        {
            stack.Push((SuperString)buffer.Pop());
        }
        #endregion
    }
    /// <summary>Determines whether the specified string is clean</summary>
    /// <param name="str">The string.</param>
    /// <returns>
    ///   <c>true</c> if the specified string is clean; otherwise, <c>false</c>.</returns>
    public bool IsClean(string str)
    {
        if (str.Contains("|") || str.Contains("."))
        { //It is kind of dirty
            return false;
        }
        return true;
    }
    /// <summary>Sends to garbage.</summary>
    public void SendToGarbage()
    {
        while (cache.Count > 0)
        {
            garbage.Push((string)cache.Pop());
        }
    }
    #endregion
    #region Function Nullable,Firstpos,lastpos...
    /// <summary>   set values leaf inorden</summary>
    /// <param name="r">  Root node</param>
    public void inordenSetValuesLeaf(Node r)
    {
        if (r is null == false)
        {
            inordenSetValuesLeaf(r.retornali());

            

            if (!r.retornaDato().Equals("|")
               && !r.retornaDato().Equals(".")
               && !r.retornaDato().Equals("*"))
            {

                r.asignaLabel(cont);
                leaves.Add(r.retornaDato());
                Debug.Log("Leaves "+r.retornaDato());
                cont++;

            }
            inordenSetValuesLeaf(r.retornald());
        }




    }
    /// <summary>Nullables the leaf.</summary>
    /// <param name="r">The r.</param>
    public void NullableLeaf(Node r)
    {
        if (r is null == false)
        {
            NullableLeaf(r.retornali());

            if (!r.retornaDato().Equals("|")
            && !r.retornaDato().Equals(".")
            && !r.retornaDato().Equals("*"))
            {
                r.asignaNullable(false);
            }

            NullableLeaf(r.retornald());
        }
    }
    /// <summary>Firsts the position lef.</summary>
    /// <param name="r">  The root node</param>
    public void FirstPosLef(Node r)
    {
        if (r is null == false)
        {
            FirstPosLef(r.retornali());

            if (!r.retornaDato().Equals("|")
            && !r.retornaDato().Equals(".")
            && !r.retornaDato().Equals("*"))
            {
                r.firstPos.Add(r.retornaLabel());
            }

            FirstPosLef(r.retornald());
        }
    }
    /// <summary>  last  position lef.</summary>
    /// <param name="r">  The root node</param>
    public void LasPosLef(Node r)
    {
        if (r is null == false)
        {
            LasPosLef(r.retornali());

            if (!r.retornaDato().Equals("|")
            && !r.retornaDato().Equals(".")
            && !r.retornaDato().Equals("*"))
            {
                r.lastPos.Add(r.retornaLabel());
            }

            LasPosLef(r.retornald());
        }
    }
    /// <summary>
    ///   <para>
    ///  Search nullables for symbols</para>
    ///   <para></para>
    /// </summary>
    /// <param name="r">  The root node</param>
    public void NullableSymbol(Node r)
    {
        if (r is null == false)
        {
            NullableSymbol(r.retornali());
            NullableSymbol(r.retornald());

            if (r.retornaDato().Equals("|"))
            {
 
              r.asignaNullable(r.retornali().retornaNullable() ||
                   r.retornald().retornaNullable());
                
            }
            else if (r.retornaDato().Equals("."))
            {
                r.asignaNullable(r.retornali().retornaNullable() &&
                  r.retornald().retornaNullable());
            }
            else if (r.retornaDato().Equals("*"))
            {
                r.asignaNullable(true);
            }
            else if (r.retornaDato().Equals("+"))
            {
                r.asignaNullable(r.retornali().retornaNullable());
            }
        }

    }
    /// <summary>Firsts the position symbol.</summary>
    /// <param name="r">The root node</param>
    public void FirstPosSymbol(Node r)
    {
        if (r is null == false)
        {
            FirstPosSymbol(r.retornali());
            FirstPosSymbol(r.retornald());

            if (r.retornaDato().Equals("|"))
            {

                r.firstPos.AddRange((r.retornali().firstPos));
                r.firstPos.AddRange((r.retornald().firstPos));

            }
            else if (r.retornaDato().Equals("."))
            {
                if (r.retornali().retornaNullable())
                {
                    r.firstPos.AddRange((r.retornali().firstPos));
                    r.firstPos.AddRange((r.retornald().firstPos));
                }
                else
                    r.firstPos.AddRange((r.retornali().firstPos));
            }
            else if (r.retornaDato().Equals("*")|| r.retornaDato().Equals("+"))
            {
                r.firstPos.AddRange((r.retornali().firstPos));
            }
 

        }
    }
    /// <summary>Set the lastPost of each node</summary>
    /// <param name="r">  The root node</param>
    public void LasPosSymbol(Node r)
    {
        if (r is null == false)
        {
            LasPosSymbol(r.retornali());
            LasPosSymbol(r.retornald());

            if (r.retornaDato().Equals("|"))
            {

                r.lastPos.AddRange((r.retornali().lastPos));
                r.lastPos.AddRange((r.retornald().lastPos));

            }
            else if (r.retornaDato().Equals("."))
            {
                if (r.retornald().retornaNullable())
                {
                    r.lastPos.AddRange((r.retornali().lastPos));
                    r.lastPos.AddRange((r.retornald().lastPos));
                }
                else
                    r.lastPos.AddRange((r.retornald().lastPos));
            }
            else if (r.retornaDato().Equals("*")|| r.retornaDato().Equals("+"))
            {
                r.lastPos.AddRange((r.retornali().lastPos));
            }

        }
    }
    /// <summary>  Set the followPos list of each node</summary>
    /// <param name="r">The r.</param>
    public void FollowPos(Node r)
    {
        if (r is null == false)
        {
            

            if (r.retornaDato().Equals("."))
            {
                List<int> lastPos_c1 = new List<int>();
                List<int> firstPos_c2 = new List<int>();
                lastPos_c1.AddRange(r.retornali().lastPos);
                firstPos_c2.AddRange(r.retornald().firstPos);
                for (int i = 0; i < lastPos_c1.Count; i++)
                {
                   
                   followPos[lastPos_c1[i] - 1].AddRange(firstPos_c2);
                }


            }
            else if (r.retornaDato().Equals("*"))
            {
                List<int> lastPos_n = new List<int>();
                List<int> firstPos_n = new List<int>();
                lastPos_n.AddRange(r.lastPos);
                firstPos_n.AddRange(r.firstPos);
                for (int i = 0; i < lastPos_n.Count; i++)
                {
                    
                    followPos[lastPos_n[i] - 1].AddRange(firstPos_n);
                }
            }

            FollowPos(r.retornali());
            FollowPos(r.retornald());


        }
        
    }


    #endregion

}



