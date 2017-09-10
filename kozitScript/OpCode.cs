using System;
using System.Collections.Generic;
using System.Text;

namespace kozitScript
{
    public enum OpCode
    {
        Null = 0xFD, // Nothing.
        Add = 0x00, // Add two numbers
        Subtract = 0x01, // Subtract two numbers
        Divide = 0x02, // Divide two numbers
        Mutiply = 0x03, // Multiply two numbers
        Allocate = 0x04, // Allocate space
        Assign = 0x05, // Asign data to variable
        Interrupt = 0x06, // Interrupt
        Compare = 0x08, // Compare
        Move = 0x09, // Move data from variable to variable
        Addr = 0x0A, // Move address from variable to variable
        Goto = 0x0B, // Jump to a label
        Label = 0x0C, // Define a label
        Clear = 0x0D, // Clear a variable
    }
}
