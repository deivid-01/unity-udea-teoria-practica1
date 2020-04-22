using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
/// <summary>Clase del automata finito, donde se construye y crea la correspondiente tabla donde se muestra el automata como tal</summary>
public class DFA
{

    /// <summary>Struc de estados, donde se definen cada una de las variables que le corresponden a un estado</summary>
    public struct State{
        public List<int >value;
        public bool visited;
        public char name;
        public bool approval;
    }

    /// <summary>Struct de transiciones</summary>
    struct Transition {
        public string input;
        public string from;
        public string towards;
    }

    /// <summary>The game controller mian class</summary>
    GameController gameController;
    /// <summary>The syntatic tree</summary>
    SyntaticTree syntaticTree;
    /// <summary>The table
    /// of states and transitions</summary>
    public string[,] table;


     State actualState;

    /// <summary>The lisf of states</summary>
    public List<State> states = new List<State>();
    /// <summary>The lists of  transitions</summary>
    List<Transition> transitions = new List<Transition>();
    /// <summary>The list of input symbols</summary>
    public List<string> inputSymbols = new List<string>();

    public bool errorStateOn = false;
    public State errorState;

    /// <summary>Initializes a new instance of the <see cref="DFA"/> class.</summary>
    /// <param name="root">The root.</param>
    public DFA(Node root)
    {
        gameController = new GameController();
        syntaticTree = gameController.GetSyntaticTree();

        GenerateInputSymbols();

        states.Add(NewState(root.firstPos.ToArray(), states.Count));

        GenerateStates();
        CreateTable();
        actualState = states[0];//First state is actual state
    }

    /// <summary>Gets the table.</summary>
    /// <returns>The table</returns>
    public string[,] GetTable()
    {
        return table;
    }

    /// <summary>Generates the states.</summary>
    void GenerateStates()
    {
        
        int index= SearchStateNoVisited();
        if (index==-1)
            return;
        foreach (var symbol in inputSymbols)
        {
            List<int> set=CreateSubSet(states[index],symbol);
            if (set == null)
            {
                if (!errorStateOn)
                {
                     errorState = ErrorState(states.Count);
                    transitions.Add(NewTransition(symbol, states[index].name.ToString(), errorState.name.ToString()));
                    states.Add(errorState);
                    errorStateOn = true;
                }
                else
                {
                    transitions.Add(NewTransition(symbol, states[index].name.ToString(), errorState.name.ToString()));
                }


            }

            else if (!SetExist(set.ToArray()))
            {
                State newState = NewState(set.ToArray(), states.Count);
                states.Add(newState);
                transitions.Add(NewTransition(symbol, states[index].name.ToString(), newState.name.ToString()));


            }
            else
            {
                transitions.Add(NewTransition(symbol, states[index].name.ToString(), WhereSetExist(set.ToArray()).name.ToString()));
        
            }

        }

        states[index] = SetVisitedState(index);
        GenerateStates();
    }

    /// <summary>Searches the state no visited.</summary>
    /// <returns></returns>
    int SearchStateNoVisited()
    {
        for (int i = 0; i < states.Count; i++)
        {
            if (!states[i].visited)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>Generates the input symbols.</summary>
    void GenerateInputSymbols()
    {
        foreach (var simbol in syntaticTree.leaves )
        {
            if (!inputSymbols.Contains(simbol) && !simbol.Equals("#"))
            {
                inputSymbols.Add(simbol);
            }
        }
    }

    /// <summary>Creates the sub set.</summary>
    /// <param name="state">The state.</param>
    /// <param name="symbol">The symbol.</param>
    /// <returns></returns>
    List<int> CreateSubSet(State state, string symbol)
    {
        List<int> subset = new List<int>();
        foreach (var node in state.value)
        {
            if (syntaticTree.leaves[node - 1].Equals(symbol))
            {
                List<int> followPosNode = gameController.GetSyntaticTree().followPos[node - 1];
                if (followPosNode.Count > 0)
                {
                    subset.AddRange(followPosNode);
                }
            }

   



        }
        if (subset.Count == 0)
        {
            return null;
        }
        return subset;
    }

    /// <summary>  Search if set exist</summary>
    /// <param name="set">The set.</param>
    /// <returns></returns>
    bool SetExist(int[ ] set) {

        //recorer statedD
        foreach (var state in states)
        {
            state.value.Sort();
            Array.Sort(set);
            if (state.value.SequenceEqual(set))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>  En que parte de la lista se encuentra</summary>
    /// <param name="set">The set.</param>
    /// <returns>La ubicacion donde se encuentra</returns>
    State WhereSetExist(int[] set)
    {

        //recorer statedD
        foreach (var state in states)
        {
            state.value.Sort();
            Array.Sort(set);

            if (state.value.SequenceEqual(set))
            {
                return state;
            }
        }
        State nullState = NewState(null, -60);

        return nullState;
  
    }

    /// <summary>Creates new state.</summary>
    /// <param name="v">The v.</param>
    /// <param name="n">The n.</param>
    /// <returns></returns>
    State NewState(int[] v, int n)
    {
        
        State newState;
        newState.value = new List<int>();
        newState.value.AddRange(v.ToList());
        newState.visited = false;
        newState.name =(char)(65 + n);

        if (newState.value.Contains(syntaticTree.leaves.Count))
        {
            newState.approval = true;
        }
        else
            newState.approval = false; 



        return newState;
    }

    State ErrorState(int n)
    {
        State newState;
        newState.value= new List<int>();
        newState.visited = false;
        newState.name = (char)(65 + n);
        if (newState.value.Contains(syntaticTree.leaves.Count))
        {
            newState.approval = true;
        }
        else
            newState.approval = false;



        return newState;
   
    }

    /// <summary>Creates new transition.</summary>
    /// <param name="input">The input.</param>
    /// <param name="from">From.</param>
    /// <param name="towards">The towards.</param>
    /// <returns></returns>
    Transition NewTransition(string input,string from, string towards)
    {
        Transition newTransition;
        newTransition.input =input;

        newTransition.from = from;
        newTransition.towards = towards;

        return newTransition;
    }

    /// <summary>Sets the state of the visited.</summary>
    /// <param name="index">The index.</param>
    /// <returns></returns>
    State SetVisitedState(int index) {

        State newState;
        newState.value = states[index].value;
        newState.name = states[index].name;
        newState.approval = states[index].approval; 
        newState.visited = true;
        return newState;

    }
    /// <summary>Shows the set.</summary>
    /// <param name="v">The v.</param>
    void ShowSet(int[ ]v)
    {
        Debug.Log("Inicio");
        foreach (var item in v)
        {
            Debug.Log(item);
        }
        Debug.Log("Fin");
    }

    /// <summary>Reads the input.</summary>
    /// <param name="inputArray">The input array.</param>
    /// <returns></returns>
    public string ReadInput(string inputArray)
    {
        string input = "";
        for (int i = 0; i < inputArray.Length; i++)
        {
            bool correct = false;
            input += inputArray[i];
            foreach (var symbol in inputSymbols)
            {
                if (symbol.Equals(input))
                {
                    //Actualizar Estado
                    UpdateState(symbol);
                    input = "";
                    correct = true;
                    break;
                }
                else 
                {
                    //Romper y seguir sumando
                    if (symbol.Length != input.Length)
                    {
                        break;
                    }
                    else if (symbol.Substring(0, input.Length).Equals(input))
                    {
                        correct = true;
                        break;
                        }
                    
                }               
            }

            if (!correct)
            {

                return "Rejected sequence";
            }
        }

        return CheckFinalStatus();
    }

    /// <summary>Updates the state.</summary>
    /// <param name="inputSymbol">The input symbol.</param>
    public void UpdateState(string inputSymbol)
    {

        int indexInputSymbol = inputSymbols.IndexOf(inputSymbol);
        int indexState = states.IndexOf(actualState);

        foreach (var state in states)
        {
            if (state.name.ToString().Equals(table[indexState, indexInputSymbol]))
            {
                actualState = state;
                return;
            }
        }

    }

    /// <summary>Creates the table.</summary>
    public void CreateTable()
    {
        table = new string[states.Count,inputSymbols.Count];

        for (int i = 0; i < states.Count; i++)
        {
            for (int j = 0; j < inputSymbols.Count; j++)
            {
                table[i, j] = GetMeetingPoint(i,j);//Between state and symbol
            }
        }
    }

    /// <summary>Gets the meeting point.</summary>
    /// <param name="indexState">State of the index.</param>
    /// <param name="indexSymbol">The index symbol.</param>
    /// <returns>La ubicacion en la tabla de transicion</returns>
    string GetMeetingPoint(int indexState, int indexSymbol)
    {
        //Search in states 

        string stateName = states[indexState].name.ToString();

        //Search in inPutSymbols

        string input = inputSymbols[indexSymbol]; 
            //Search in transition with states and input Symbol

        return SearchOnTransition(stateName,input);
    }

    /// <summary>Searches the state on transition.</summary>
    /// <param name="from">  Initial state</param>
    /// <param name="input">  Input Symbol</param>
    /// <returns>Transition state</returns>
    string SearchOnTransition(string from, string input)
    {
        foreach (var transition in transitions)
        {
            if (transition.from.Equals(from) && transition.input.Equals(input))
            {
                return transition.towards;
            }
        }

        return "-";
    }

    /// <summary>Checks the final status.</summary>
    /// <returns>retorna si la secuencia fue aceptada o no</returns>
    string CheckFinalStatus()
    {
        if (actualState.approval)
        {
            return "Accepted Sequence";
        }
        else
        {
            return "Rejected sequence";
        }
    }
}
