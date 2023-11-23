using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numc.lexer
{
    struct Token
    {
        public string content;
        public TokenType type;
        public int line;
        public Token(string content, TokenType type, int line)
        {
            this.content = content.Replace(" ", "").Replace("\t", "");
            this.type = type;
            this.line = line;
        }

        internal void print()
        {
            Console.WriteLine("Type:    " + type.ToString());
            Console.WriteLine("Line:    " + line);
            Console.WriteLine("Content: " + content);
        }
        internal void setLine(int line)
        {
            this.line = line;
        }

        
    }
    enum TokenType
    {
        CURLY_BRACE_LEFT,
        CURLY_BRACE_RIGHT,
        BRACE_RIGHT,
        BRACE_LEFT,

        OPERATOR_PLUS,
        OPERATOR_MINUS,
        OPERATOR_DIVIDE,
        OPERATOR_MULTIPLY,
        OPERATOR_NOT,
        OPERATOR_XOR,
        OPERATOR_MODULO,

        OPERATOR_SINGLE_EQUALS,
        OPERATOR_SINGLE_OR,
        OPERATOR_SINGLE_AND,

        OPERATOR_DOUBLE_EQUALS,
        OPERATOR_DOUBLE_OR,
        OPERATOR_DOUBLE_AND,

        SEMICOLON,
        COMMA,

        NAME,
        KEYWORD,
        NUMBER
    }
}
