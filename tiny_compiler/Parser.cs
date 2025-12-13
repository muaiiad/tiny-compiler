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
        //basmala
        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(Function_statements());
            program.Children.Add(Main_function());
            return program;
        }
        Node Function_body()
        {
            Node body = new Node("Function_body");
            body.Children.Add(match(Token_Class.T_LeftCurlyBracket));
            body.Children.Add(Statements());
            body.Children.Add(match(Token_Class.T_RightCurlyBracket));
            return body;
        }
        Node Main_function()
        {
            Node main = new Node("Main_function");
            main.Children.Add(Datatype());
            main.Children.Add(match(Token_Class.T_Main));
            main.Children.Add(match(Token_Class.T_LeftParenthesis));
            main.Children.Add(match(Token_Class.T_RightParenthesis));
            main.Children.Add(Function_body());
            return main;
        }
        Node Datatype()
        {
            Node dt = new Node("Datatype");
            Token_Class tt = TokenStream[InputPointer].token_type;
            if (tt == Token_Class.T_Int ||
                tt == Token_Class.T_Float ||
                tt == Token_Class.T_String)
            {
                dt.Children.Add(match(tt));
                return dt;
            }
            Errors.Error_List.Add("Parsing Error: Expected datatype.\n");
            return null;
        }
        //Node Function_Declaration()
        //{
        //    Node decl = new Node("Function_Declaration");
        //    decl.Children.Add(Datatype());
        //    decl.Children.Add(match(Token_Class.T_Identifier));
        //    decl.Children.Add(match(Token_Class.T_LeftParenthesis));
        //    decl.Children.Add(Parameter());
        //    decl.Children.Add(match(Token_Class.T_RightParenthesis));
        //    return decl;
        //}
        Node Function_statement()
        {
            Node func = new Node("Function_statement");
            func.Children.Add(Function_Declaration());
            func.Children.Add(Function_body());
            return func;
        }
        Node Function_statements()
        {
            Node funcs = new Node("Function_statements");
            if (TokenStream[InputPointer].token_type == Token_Class.T_Int || TokenStream[InputPointer].token_type == Token_Class.T_Float ||
                TokenStream[InputPointer].token_type == Token_Class.T_String) {
                funcs.Children.Add(Function_statement());
                funcs.Children.Add(Function_statements());
                return funcs;
            }
            return null; 
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
        
bool isStatementStart()
{
    Token_Class tt = TokenStream[InputPointer].token_type;

    return tt == Token_Class.T_Write
        || tt == Token_Class.T_Read
        || tt == Token_Class.T_Identifier
        || tt == Token_Class.T_Return
        || tt == Token_Class.T_Int
        || tt == Token_Class.T_Float
        || tt == Token_Class.T_String
        || tt == Token_Class.T_If;
}

/*bool isBooleanOperator(Token_Class t)
{
    return t == Token_Class.T_And
        || t == Token_Class.T_Or;
     
}*/


Node Statements()
{
    Node stmts = new Node("Statements");

    Node first = Statement();
    if (first == null) return null;

    stmts.Children.Add(first);

    while (isStatementStart())
    {
        stmts.Children.Add(Statement());
    }

    return stmts;
}

Node Statement()
{
    Token_Class tt = TokenStream[InputPointer].token_type;

    if (tt == Token_Class.T_Write) return Write_Statement();
    if (tt == Token_Class.T_Read) return Read_Statement();
    if (tt == Token_Class.T_Return) return Return_Statement();
    if (tt == Token_Class.T_If) return If_Statement();
    if (tt == Token_Class.T_Repeat) return Repeat_Statement(); 

    if (tt == Token_Class.T_Identifier)
    {
       
        if (InputPointer + 1 < TokenStream.Count &&
            TokenStream[InputPointer + 1].token_type == Token_Class.T_LeftParenthesis)
            return Function_Call();
        else
            return Assignment_Statement();
    }

    if (tt == Token_Class.T_Int || tt == Token_Class.T_Float || tt == Token_Class.T_String)
        return Declaration_Statement();

    Errors.Error_List.Add("Parsing Error: Invalid statement\n");
    return null;
}


Node Read_Statement()
{
    Node readStmt = new Node("Read_Statement");
    readStmt.Children.Add(match(Token_Class.T_Read));
    readStmt.Children.Add(match(Token_Class.T_Identifier));
    readStmt.Children.Add(match(Token_Class.T_Semicolon));
    return readStmt;
}

Node Return_Statement()
{
    Node ret = new Node("Return_Statement");
    ret.Children.Add(match(Token_Class.T_Return));
    ret.Children.Add(Expression());
    ret.Children.Add(match(Token_Class.T_Semicolon));
    return ret;
}


Node If_Statement()
{
    Node ifStmt = new Node("If_Statement");

    ifStmt.Children.Add(match(Token_Class.T_If));
    ifStmt.Children.Add(Condition_Statement());
    ifStmt.Children.Add(match(Token_Class.T_Then));
    ifStmt.Children.Add(Statements());
    ifStmt.Children.Add(If_Ending());

    return ifStmt;
}

Node If_Ending()
{
    Node end = new Node("If_Ending");

    Token_Class tt = TokenStream[InputPointer].token_type;

    if (tt == Token_Class.T_ElseIf)
        end.Children.Add(Else_If_Statement());
    else if (tt == Token_Class.T_Else)
        end.Children.Add(Else_Statement());
    else
        end.Children.Add(match(Token_Class.T_End)); // default END

    return end;
}


Node Condition_Statement()
{
    Node cs = new Node("Condition_Statement");
    cs.Children.Add(Condition());
    cs.Children.Add(Condition_Ending());
    return cs;
}

Node Condition_Ending()
{
    Node ce = new Node("Condition_Ending");

    if (TokenStream[InputPointer].token_type==Token_Class.T_And)
    {
        ce.Children.Add(match(Token_Class.T_And));
        ce.Children.Add(Condition());
        return ce;}
        else if (TokenStream[InputPointer].token_type == Token_Class.T_Or)
            {
        ce.Children.Add(match(Token_Class.T_And));
        ce.Children.Add(Condition());
        return ce; 
            }
    

    return null; 
}
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

Node FunctionName()
{
    //like idnetifer
    return match(Token_Class.T_Identifier);
}

Node Function_Body()
{
    Node body = new Node("Function_Body");

    //{
    body.Children.Add(match(Token_Class.T_LeftCurlyBracket));
    //statements
    Node stmts = null;
    if (isStatementStart()) 
        stmts = Statements();
    if (stmts != null) body.Children.Add(stmts);

    //return
    body.Children.Add(Return_Statement());

    // }
    body.Children.Add(match(Token_Class.T_RightCurlyBracket));

    return body;
}

Node Write_Statement()
{
    Node write = new Node("Write_Statement");
    //write
    write.Children.Add(match(Token_Class.T_Write));

    //expression aw endl
    Token_Class next = TokenStream[InputPointer].token_type;
    if (next == Token_Class.T_StringLiteral ||
        next == Token_Class.T_Number ||
        next == Token_Class.T_Identifier ||
        next == Token_Class.T_LeftParenthesis)
    {
        write.Children.Add(Expression());
    }
    else if (next == Token_Class.T_Endl) 
    {
        write.Children.Add(match(Token_Class.T_Endl));
    }
    else
    {
        Errors.Error_List.Add("Parsing Error: Expected expression or endl after write\n");
        return null;
    }
    //;
    write.Children.Add(match(Token_Class.T_Semicolon));
    return write;
}

Node Repeat_Statement()
{
    Node rep = new Node("Repeat_Statement");
    //repeat
    rep.Children.Add(match(Token_Class.T_Repeat));

    // statements
    Node stmts = null;
    if (isStatementStart())
        stmts = Statements();
    
    if (stmts != null) rep.Children.Add(stmts);

    // until
    rep.Children.Add(match(Token_Class.T_Until));
    //condition
    rep.Children.Add(Condition_Statement());

    return rep;
}
        //---------------------------------------------
        Node Assignment_Statement()
        {
            if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
            {
                Node assignment = new Node("Assignment_Statement");
                assignment.Children.Add(match(Token_Class.T_Identifier));
                assignment.Children.Add(match(Token_Class.T_Assign));
                assignment.Children.Add(Expression());
                assignment.Children.Add(match(Token_Class.T_Semicolon));
                return assignment;
            }

            Errors.Error_List.Add("Parsing Error: Expected " + Token_Class.T_Identifier.ToString() + "\r\n");
            return null;
        }

        Node Initialiser_Statement()
        {
            if (TokenStream[InputPointer].token_type == Token_Class.T_Assign)
            {
                Node initStatement = new Node("Initialiser_Statement");
                initStatement.Children.Add(match(Token_Class.T_Assign));
                initStatement.Children.Add(Expression());
                return initStatement;
            }
            else
            {
                return null;
            }
        }

        Node Initialiser()
        {
            if (TokenStream[InputPointer].token_type == Token_Class.T_Identifier)
            {
                Node init = new Node("Initialiser");
                init.Children.Add(match(Token_Class.T_Identifier));
                init.Children.Add(Initialiser_Statement());
                return init;
            }

            Errors.Error_List.Add("Parsing Error: Expected " + Token_Class.T_Identifier.ToString() + "\r\n");
            return null;
        }

        Node More_Initialisers()
        {
            if(TokenStream[InputPointer].token_type == Token_Class.T_Comma)
            {
                Node moreInits = new Node("More_Initialisers");
                moreInits.Children.Add(match(Token_Class.T_Comma));
                moreInits.Children.Add(Initialiser());
                moreInits.Children.Add(More_Initialisers());
                return moreInits;
            }
            else
            {
                return null;
            }
        }

        Node Initialiser_List()
        {
            Node initTemp = Initialiser();

            if (initTemp != null)
            {
                Node initList = new Node("Initialiser_List");
                initList.Children.Add(initTemp);
                initList.Children.Add(More_Initialisers());
                return initList;
            }

            Errors.Error_List.Add("Parsing Error: Expected an identifier" + "\r\n");
            return null;
        }

        Node Declaration_Statement()
        {
            Node datatype = Datatype();

            if (datatype != null)
            {
                Node decl = new Node("Declaration_statement");
                decl.Children.Add(datatype);

                Node initList = Initialiser_List();
                if (initList != null)
                {
                    decl.Children.Add(initList);
                    decl.Children.Add(match(Token_Class.T_Semicolon));
                    return decl;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected an identifier" + "\r\n");
                    return null;
                }
            }
            Errors.Error_List.Add("Parsing Error: Expected a datatype" + "\r\n");
            return null;
        }

        Node More_Parameters()
        {
            if(TokenStream[InputPointer].token_type == Token_Class.T_Comma)
            {
                Node tempParam = Parameter();

                if (tempParam != null)
                {
                    Node moreParams = new Node("More_Parameters");
                    moreParams.Children.Add(match(Token_Class.T_Comma));
                    moreParams.Children.Add(tempParam);
                    moreParams.Children.Add(More_Parameters());
                    return moreParams;
                }
                Errors.Error_List.Add("Parsing Error: Expected a parameter" + "\r\n");
                return null;
            }
            return null;
        }

        Node Parameters()
        {
            Node tempParam = Parameter();

            if (tempParam != null)
            {
                Node param = new Node("Parameters");
                param.Children.Add(tempParam);
                param.Children.Add(More_Parameters());
                return param;
            }
            Errors.Error_List.Add("Parsing Error: Expected a parameter" + "\r\n");
            return null;
        }

        Node Parameter_List()
        {
            Node tempParam = Parameters();

            if (tempParam != null)
            {
                Node paramList = new Node("Parameter_List");
                paramList.Children.Add(tempParam);
                return paramList;
            }
            return null;
        }

        Node Function_Declaration()
        {
            Node dt = Datatype();

            if (dt != null)
            {
                Node fDecl = new Node("Function_Declaration");
                fDecl.Children.Add(dt);
                fDecl.Children.Add(FunctionName());
                fDecl.Children.Add(match(Token_Class.T_LeftParenthesis));
                fDecl.Children.Add(Parameter_List());
                fDecl.Children.Add(match(Token_Class.T_RightParenthesis));
                fDecl.Children.Add(match(Token_Class.T_Semicolon));
                return fDecl;
            }
            Errors.Error_List.Add("Parsing Error: Expected datatype.\n");
            return null;
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
