using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tiny_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            //root = new Node("Program");
            //root.Children.Add(Program());
            return root;
        }

        Node Function_Call() {
            Node func = new Node("Function_Call");
            func.Children.Add(match(Token_Class.T_Identifier));
            func.Children.Add(match(Token_Class.T_LeftParenthesis));
            func.Children.Add(Arguments());
            func.Children.Add(match(Token_Class.T_RightParenthesis));
            return func;
        }

        Node Arguments ()
        {
            if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
            {
                Node args = new Node("Arguments");
                args.Children.Add(match(Token_Class.T_Identifier));
                args.Children.Add(MoreArgs());
                return args;
            } else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + Token_Class.T_Identifier.ToString() + "\r\n");
                return null;
            }
        }
        Node MoreArgs()
        {
            if (TokenStream[InputPointer].token_type == Token_Class.T_Comma)
            {
                Node moreargs = new Node("MoreArgs");
                moreargs.Children.Add(match(Token_Class.T_Comma));
                moreargs.Children.Add(match(Token_Class.T_Identifier));
                moreargs.Children.Add(MoreArgs());
                return moreargs;
            } else
            {
                return null;
            }

        }

        Node Term ()
        {
            Node term = new Node("Term");
            if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier )
            {
                if (InputPointer + 1 < TokenStream.Count &&
                    TokenStream[InputPointer + 1].token_type == Token_Class.T_LeftParenthesis)
                {
                    term.Children.Add(Function_Call());
                } else
                {
                    term.Children.Add(match(Token_Class.T_Identifier));
                }
            } else if (TokenStream[InputPointer].token_type == Token_Class.T_Number)
            {
                term.Children.Add( match(Token_Class.T_Number));
            } else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                + Token_Class.T_Identifier.ToString() + "\r\n");
                return null;
            }
            return term;
        }

        Node Operand()
        {
            Node operand = new Node("Operand");
            if (TokenStream[InputPointer].token_type == Token_Class.T_LeftParenthesis) 
            {
                match(Token_Class.T_LeftParenthesis);
                operand.Children.Add(Equation());
                match(Token_Class.T_RightParenthesis);
            } else
            {
                operand.Children.Add(Term());
            }
            return operand;
        }

        Node Equation()
        {
            Node equation = new Node("Equation");
            equation.Children.Add(Operand());
            equation.Children.Add(EquationPrime());
            return equation;
        }
        Node Operation()
        {
            Node operation = new Node("Operation");
            if (TokenStream[InputPointer].token_type == Token_Class.T_PlusOp)
            {
                operation.Children.Add(match(Token_Class.T_PlusOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_MinusOp)
            {
                operation.Children.Add(match(Token_Class.T_MinusOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_MultiplyOp)
            {
                operation.Children.Add(match(Token_Class.T_MultiplyOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.T_DivideOp)
            {
                operation.Children.Add(match(Token_Class.T_DivideOp));
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected operation\n");
                return null;
            }
            return operation;
        }
        Node EquationPrime() {
            Node equationprime = new Node("EquationPrime");
            if (TokenStream[InputPointer].token_type == Token_Class.T_PlusOp ||
            TokenStream[InputPointer].token_type == Token_Class.T_MinusOp ||
            TokenStream[InputPointer].token_type == Token_Class.T_MultiplyOp ||
            TokenStream[InputPointer].token_type == Token_Class.T_DivideOp)
            {
                equationprime.Children.Add(Operation());
                equationprime.Children.Add(Operand());
                equationprime.Children.Add(EquationPrime());
                return equationprime;
            }

            // ε-production (do nothing)
            return null;
        }

        Node Expression()
        {
            Node expression = new Node("Expression");
            if (TokenStream[InputPointer].token_type == Token_Class.T_StringLiteral)
            {
                expression.Children.Add(match(Token_Class.T_StringLiteral));
                return expression;
            }
            expression.Children.Add(Equation());
            return expression;
        }

        //Node Program()
        //{
        //    Node program = new Node("Program");
        //    program.Children.Add(Header());
        //    program.Children.Add(DeclSec());
        //    program.Children.Add(Block());
        //    program.Children.Add(match(Token_Class.T_Comma));
        //    MessageBox.Show("Success");
        //    return program;
        //}

        //Node Header()
        //{
        //    Node header = new Node("Header");
        //    // write your code here to check the header sructure
        //    return header;
        //}
        //Node DeclSec()
        //{
        //    Node declsec = new Node("DeclSec");
        //    // write your code here to check atleast the declare sturcure 
        //    // without adding procedures
        //    return declsec;
        //}
        //Node Block()
        //{
        //    Node block = new Node("block");
        //    // write your code here to match statements
        //    return block;
        //}

        // Implement your logic here
//Noha
Node Condition()
{
    Node cond = new Node("Condition");
    cond.Children.Add(match(Token_Class.T_Identifier));
    cond.Children.Add(Condition_Operator());
    cond.Children.Add(Term());
    return cond;
}
Node Condition_Operator()
{
    Node op = new Node("Condition_Operator");
    Token_Class tt = TokenStream[InputPointer].token_type;
    if (tt == Token_Class.T_Equal)
        op.Children.Add(match(Token_Class.T_Equal));
    else if (tt == Token_Class.T_NotEqual)
        op.Children.Add(match(Token_Class.T_NotEqual));
    else if (tt == Token_Class.T_LessThan)
        op.Children.Add(match(Token_Class.T_LessThan));
    else if (tt == Token_Class.T_GreaterThan)
        op.Children.Add(match(Token_Class.T_GreaterThan));
    else
    {
        Errors.Error_List.Add("Parsing Error: Invalid condition operator.\n");
        return null;
    }
    return op;
}
Node Else_If_Statement()
{
    Node elif = new Node("Else_If_Statement");
    elif.Children.Add(match(Token_Class.T_ElseIf));
    elif.Children.Add(Condition_Statement());
    elif.Children.Add(match(Token_Class.T_Then));
    elif.Children.Add(Statements());
    elif.Children.Add(If_Ending());
    return elif;
}
Node Else_Statement()
{
    Node els = new Node("Else_Statement");
    els.Children.Add(match(Token_Class.T_Else));
    els.Children.Add(Statements());
    els.Children.Add(match(Token_Class.T_End));

    return els;
}
Node Parameter()
{
    Node param = new Node("Parameter");
    Token_Class tt = TokenStream[InputPointer].token_type;
    if (tt == Token_Class.T_Int ||
        tt == Token_Class.T_Float ||
        tt == Token_Class.T_String)
    {
        param.Children.Add(match(tt));
    }
    else
    {
        Errors.Error_List.Add("Parsing Error: Expected datatype in parameter\n");
        return null;
    }
    param.Children.Add(match(Token_Class.T_Identifier));
    return param;
}
//---------------------------------------------
        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
