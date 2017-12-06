using System;
using System.Collections.Generic;
using System.Text;

namespace kozitScript
{
    public class ParseTree : Data
    {
        public Function Main;
        public List<Function> Functions;
    }

    // A function is bunch of Instructions
    public class Function : Data
    {
        public string Name;
        public Args Arguments;
        public List<Instruction> Instructions;
    }

    // An Instruction is a function, an operator and a variable.

    public class Instruction : Data
    {
        public Variable Var;
        public Operator Op;
        public Execute Func;
    }
    // A variable has a name and value
    public class Variable : Data
    {
        public bool ignore = false;
        public bool isNum = false;
        public bool isConst = false;
        public bool isReg = false;
        public bool isList = false;
        public string Name;
        public List<string> List;
    }
    // An operator has a type
    public class Operator : Data
    {
        public string Type;
    }
    // A Execute (function call) has a namespace ID, function id and arguments.
    public class Execute : Data
    {
        public string Function;
        public Args Arguments;
    }
    // The arguments have variables.
    public class Args : Data
    {
        public List<Variable> Arguments;
    }

    public class Data
    {
    }
}
