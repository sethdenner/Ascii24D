using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    class BufferPixel
    {
        public BufferPixel(
            ConsoleColor backgroundColor,
            ConsoleColor foregroundColor,
            char charachterCode
        )
        {
            this._backgroundColor = backgroundColor;
            this._foregroundColor = foregroundColor;
            this._charachterCode = charachterCode;
        }

        private ConsoleColor _backgroundColor;
        private ConsoleColor _foregroundColor;
        private char _charachterCode;

        public ConsoleColor BackgroundColor
        {
            get { return _backgroundColor; } 
            set { _backgroundColor = value; }
        }

        public ConsoleColor ForegroundColor
        { 
            get { return _foregroundColor; } 
            set { _foregroundColor = value; }
        }
        public char CharachterCode
        {
            get { return _charachterCode; }
            set { _charachterCode = value; }
        }

        public override string ToString()
        {
            return this._charachterCode.ToString();
        }
    }

}
