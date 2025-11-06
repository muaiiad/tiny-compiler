using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


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
            ReservedWords.Add("int", Token_Class.T_Int);
            ReservedWords.Add("float", Token_Class.T_Float);
            ReservedWords.Add("string", Token_Class.T_String);
            ReservedWords.Add("read", Token_Class.T_Read);
            ReservedWords.Add("write", Token_Class.T_Write);
            ReservedWords.Add("repeat", Token_Class.T_Repeat);  
            ReservedWords.Add("until", Token_Class.T_Until);
            ReservedWords.Add("if", Token_Class.T_If);
            ReservedWords.Add("elseif", Token_Class.T_ElseIf);
            ReservedWords.Add("else", Token_Class.T_Else);
            ReservedWords.Add("then", Token_Class.T_Then);
            ReservedWords.Add("return", Token_Class.T_Return);
            ReservedWords.Add("endl", Token_Class.T_Endl);
            ReservedWords.Add("end", Token_Class.T_End);
            ReservedWords.Add("main", Token_Class.T_Main);


            Operators.Add("+", Token_Class.T_PlusOp);
            Operators.Add("-", Token_Class.T_MinusOp);
            Operators.Add("*", Token_Class.T_MultiplyOp);
            Operators.Add("/", Token_Class.T_DivideOp);
            Operators.Add("<", Token_Class.T_LessThan);
            Operators.Add(">", Token_Class.T_GreaterThan);
            Operators.Add("=", Token_Class.T_Equal);
            Operators.Add("!", Token_Class.T_NotEqual);
            Operators.Add("&&", Token_Class.T_And);
            Operators.Add("||", Token_Class.T_Or);
            Operators.Add("(", Token_Class.T_LeftParenthesis);
            Operators.Add(")", Token_Class.T_RightParentesis);
            Operators.Add("{", Token_Class.T_LeftCurlyBracket);
            Operators.Add("}", Token_Class.T_RightCurlyBracket);
            Operators.Add(",", Token_Class.T_Comma);
            Operators.Add(";", Token_Class.T_Semicolon);
            Operators.Add(":=", Token_Class.T_Assign);
            Operators.Add("==", Token_Class.T_Equal);
            Operators.Add("!=", Token_Class.T_NotEqual);
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
                    j++;
                    // Identifier or Reserved Word
                    while (j < SourceCode.Length && 
                           ((SourceCode[j] >= 'A' && SourceCode[j] <= 'Z') ||
                            (SourceCode[j] >= 'a' && SourceCode[j] <= 'z') ||
                            (SourceCode[j] >= '0' && SourceCode[j] <= '9')))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }

                    i = j - 1;
                    FindTokenClass(CurrentLexeme);


                }

                else if (CurrentChar >= '0' && CurrentChar <= '9')
                {
                    j++;
                    bool deci= false;
                    while (j < SourceCode.Length){
                        char next = SourceCode[j];
                        if (next >= '0' && next <= '9') {
                            CurrentLexeme += next;
                            j++;
                        }
                        else if ((!deci) && next == '.') {
                            deci = true;
                            CurrentLexeme += next;
                            j++;
                        }
                        else break;
                    }
                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                }

                //brackets
                else if (CurrentChar == '(' || CurrentChar == ')' || CurrentChar == '{' || CurrentChar == '}')
                {
                    CurrentLexeme = CurrentChar.ToString();
                    FindTokenClass(CurrentLexeme);
                }
                else if (CurrentChar == '/')
                {
                    j++;
                    if (j<SourceCode.Length && SourceCode[j] == '*')
                    {
                        j++;
                        while (j < SourceCode.Length - 1 && !(SourceCode[j] == '*' && SourceCode[j + 1] == '/'))
                        {
                            j++;
                        }
                        j += 2;
                        i = j - 1;
                    }
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
            if (ReservedWords.ContainsKey(Lex))
            {
                TC = ReservedWords[Lex];
                Tok.token_type = TC;
                Tokens.Add(Tok);
                return;
            }
            //Is it an identifier?
            if (isIdentifier(Lex))
            {
                TC = Token_Class.T_Identifier;
                Tok.token_type = TC;
                Tokens.Add(Tok);
                return;
            }
            //Is it a Constant?
            if (isConstant(Lex))
            {
                TC = Token_Class.T_Number;
                Tok.token_type = TC;
                Tokens.Add(Tok);
                return;
            }


            //Is it an operator?
            if (Operators.ContainsKey(Lex))
            {
                TC = Operators[Lex];
                Tok.token_type = TC;
                Tokens.Add(Tok);
                return;
            }

            //Is it an undefined?
        }

    

        bool isIdentifier(string lex)
        {

            // TODO: Check if the lex is an identifier or not.
            if (Regex.IsMatch(lex, @"\b[ a-zA-Z ]([ a-zA-Z0-9])*\b"))
            {
                return true;
            }
            
            return false;
        }
        bool isConstant(string lex)
        {
            // TODO: Check if the lex is a constant (Number) or not.
            string num=@"^([1-9][0-9]*(\.[0-9]+)?)|(0(\.[0-9]+)?)$";
            return (Regex.IsMatch(lex, num));
        }
    }
}
