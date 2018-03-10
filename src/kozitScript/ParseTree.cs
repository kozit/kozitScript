using System;
using System.Collections.Generic;
using System.Text;

namespace kozitScript
{
    public class ParseTree : Data
    {
        public List<Function> Functions;
    }

    // A function is bunch of Instructions
    public class Function : Data
    {
        public string Name;
        public List<string> Arguments;
        public List<Instruction> Instructions;
    }

    // An Instruction is a function, an operator and a variable.

    public class Instruction : Data
    {
        public int CodeSize;
        public Variable Var;
        public Operator Op;
        public Execute Func;
    }
    // A variable has a name and value
    public class Variable : Data
    {
        public VariableType VariableType = VariableType.Global;
        public bool ignore = false;
        public bool isNum = false;
        public bool isString = false;
        public bool isList = false;
        public string Name;
        public object value;
    }
    // An operator has a type
    public class Operator : Data
    {
        public string Type;
        public List<string> Arguments;
    }
    // A Execute (function call) has a namespace ID, function id and arguments.
    public class Execute : Data
    {
        public string Function;
        public List<string> Arguments;
    }

    public class Data
    {
    }

    public enum VariableType
    {

        Global,
        Globalconstant,
        Functions

    }

}
