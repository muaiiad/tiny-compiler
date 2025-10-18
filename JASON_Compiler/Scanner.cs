using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Token_Class
{
    T_Number, T_StringLiteral, T_Identifier,

    //operators
    T_PlusOp, T_MinusOp, T_MultiplyOp, T_DivideOp,

    //comparison
    T_LessThan, T_GreaterThan, T_Equal, T_NotEqual, T_And, T_Or,

    //brackets
    T_LeftParenthesis, T_RightParentesis, T_LeftCurlyBracket, T_RightCurlyBracket,

    //idk what to categorize these
    T_Comma, T_Semicolon, T_Assign,

    //keywords
    T_Int, T_Float, T_String, T_Read,
    T_Write, T_Repeat, T_Until, T_If,
    T_ElseIf, T_Else, T_Then, T_Return,
    T_Endl, T_End, T_Main

}
namespace Tiny_Compiler
{
    public class Token
    {
       public string lex;
       public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            // TODO: Initialize the ReservedWords and Operators dictionaries

            // JASON CODE
            //ReservedWords.Add("IF", Token_Class.If);
            //ReservedWords.Add("BEGIN", Token_Class.Begin);
            //ReservedWords.Add("CALL", Token_Class.Call);
            //ReservedWords.Add("DECLARE", Token_Class.Declare);
            //ReservedWords.Add("END", Token_Class.End);
            //ReservedWords.Add("DO", Token_Class.Do);
            //ReservedWords.Add("ELSE", Token_Class.Else);
            //ReservedWords.Add("ENDIF", Token_Class.EndIf);
            //ReservedWords.Add("ENDUNTIL", Token_Class.EndUntil);
            //ReservedWords.Add("ENDWHILE", Token_Class.EndWhile);
            //ReservedWords.Add("INTEGER", Token_Class.Integer);
            //ReservedWords.Add("PARAMETERS", Token_Class.Parameters);
            //ReservedWords.Add("PROCEDURE", Token_Class.Procedure);
            //ReservedWords.Add("PROGRAM", Token_Class.Program);
            //ReservedWords.Add("READ", Token_Class.Read);
            //ReservedWords.Add("REAL", Token_Class.Real);
            //ReservedWords.Add("SET", Token_Class.Set);
            //ReservedWords.Add("THEN", Token_Class.Then);
            //ReservedWords.Add("UNTIL", Token_Class.Until);
            //ReservedWords.Add("WHILE", Token_Class.While);
            //ReservedWords.Add("WRITE", Token_Class.Write);

            //Operators.Add(".", Token_Class.Dot);
            //Operators.Add(";", Token_Class.Semicolon);
            //Operators.Add(",", Token_Class.Comma);
            //Operators.Add("(", Token_Class.LParanthesis);
            //Operators.Add(")", Token_Class.RParanthesis);
            //Operators.Add("=", Token_Class.EqualOp);
            //Operators.Add("<", Token_Class.LessThanOp);
            //Operators.Add(">", Token_Class.GreaterThanOp);
            //Operators.Add("!", Token_Class.NotEqualOp);
            //Operators.Add("+", Token_Class.PlusOp);
            //Operators.Add("-", Token_Class.MinusOp);
            //Operators.Add("*", Token_Class.MultiplyOp);
            //Operators.Add("/", Token_Class.DivideOp);



        }

        public void StartScanning(string SourceCode)
        {
            for(int i=0; i<SourceCode.Length;i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                // TODO : Complete the scanning logic here

                if (CurrentChar >= 'A' && CurrentChar <= 'z') 
                {
                   
                }

                else if(CurrentChar >= '0' && CurrentChar <= '9')
                {

                }
                else
                {
                   
                }
            }
            
            tiny_compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            // TODO: Determine the token class of the lexeme
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            

            //Is it an identifier?
            

            //Is it a Constant?

            //Is it an operator?

            //Is it an undefined?
        }

    

        bool isIdentifier(string lex)
        {

            bool isValid=true;
            // TODO: Check if the lex is an identifier or not.
            
            return isValid;
        }
        bool isConstant(string lex)
        {
            bool isValid = true;
            // TODO: Check if the lex is a constant (Number) or not.

            return isValid;
        }
    }
}
