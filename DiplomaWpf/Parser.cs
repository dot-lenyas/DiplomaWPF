using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using PascalABCCompiler.SyntaxTree;
using SimplePascalParser;
using PascalABCCompiler.Parsers;
using GPPGParserScanner;

namespace PascalABCCompiler.SimplePascalParser
{
    public class SimplePascalLanguageParser
    {
        public static string LastWord(string s)
        {
            if (s == null)
                return "!!Oops! NoType!!";
            var ss = s.Split('.');
            if (ss[ss.Length - 1] == "")
                return "!!Oops! NoType!!";
            else return ss[ss.Length - 1];

        }

        public static void PrintNode(syntax_tree_node n, TreeNode<string> tree)
        {
            if (n.GetType() == typeof(program_module))
            {
                program_module pm = (program_module)n;
                Console.WriteLine("PROGRAM {0};", pm.program_name.prog_name.name);
                PrintNode(pm.program_block, tree);
                Console.WriteLine(".");
            }
            else if (n.GetType() == typeof(block))
            {
                block b = (block)n;
                PrintNode(b.defs, tree);
                Console.WriteLine("BEGIN");
                PrintNode(b.program_code, tree);
                Console.Write("END");
            }
            else if (n.GetType() == typeof(declarations))
            {
                var ds = (declarations)n;
                foreach (var d in ds.defs)
                    PrintNode(d, tree);
            }
            else if (n.GetType() == typeof(variable_definitions))
            {
                var vd = (variable_definitions)n;
                foreach (var d in vd.var_definitions)
                    PrintNode(d, tree);
            }
            else if (n.GetType() == typeof(var_def_statement))
            {
                var vds = (var_def_statement)n;
                foreach (var d in vds.vars.idents)
                    Console.Write(LastWord(d.ToString()) + " ");
                Console.WriteLine(": " + vds.vars_type);
            }
            else if (n.GetType() == typeof(statement_list))
            {
                var sl = (statement_list)n;
                int i = -1;
                foreach (var s in sl.subnodes)
                {
                    i++;
                    if (s.GetType() == typeof(assign))
                    {
                        var assignVar = (assign)s;
                        var idnt = (ident)assignVar.to;
                        if (assignVar.from.GetType() == typeof(int32_const))
                        {
                            var incost = (int32_const)assignVar.from;
                            Console.WriteLine($"{idnt.name} := {incost.val}");
                            tree = tree.AddChild($"{idnt.name} = {incost.val}");
                        }
                        else if (assignVar.from.GetType() == typeof(ident))
                        {
                            var incost = (ident)assignVar.from;
                            Console.WriteLine($"{idnt.name} := {incost.name}");
                            tree = tree.AddChild($"{idnt.name} = {incost.name}");
                        }
                        else if (assignVar.from.GetType() == typeof(bool_const))
                        {
                            var incost = (bool_const)assignVar.from;
                            Console.WriteLine($"{idnt.name} := {incost.val}");
                            tree = tree.AddChild($"{idnt.name} = {incost.val}");
                        }

                    }
                    else if (s.GetType() == typeof(if_node))
                    {
                        bool checkNext = false;
                        var ifSt = (if_node)s;
                        var cond = (bin_expr)ifSt.condition;
                        String temp = ParseCondition(cond);
                        Console.WriteLine(temp);
                        TreeNode<string> ifNode = tree.AddChild("if");
                        ifNode.cond = temp;
                        var d = (statement_list)ifSt.then_body;
                        TreeNode<string> thenNode = ifNode.AddChild("then");
                        tree = thenNode;
                        checkNext = true;
                        PrintNode(d, tree);
                        /*foreach (var trees in tree.Children)
                        {
                            tree = trees;
                        }
                        PrintNextNode(sl.subnodes[i + 1], tree, i);*/
                        Console.WriteLine("end;");
                        /*TreeNode<string> nextElem = tree.Children.ElementAt(0);*/
                        if (ifSt.else_body != null)
                        {

                            if (ifSt.else_body.GetType() == typeof(statement_list))
                            {
                                TreeNode<string> elseNode = ifNode.AddChild("else");
                                tree = elseNode;
                                Console.WriteLine("else begin");
                                d = (statement_list)ifSt.else_body;
                                PrintNode(d, tree);
                                /*foreach (var trees in tree)
                                {
                                    tree = trees;
                                }*/

                                Console.WriteLine("end;");
                                /*tree.Children.Add(nextElem);
                                nextElem.Parent = ifNode;
                                ifNode.Children.Add(nextElem);
                                tree = nextElem;*/
                                tree = tree.Parent;

                            }
                        }
                        else
                        {
                            tree = tree.Parent;
                        }
                        /*else if (ifSt.else_body.GetType() == typeof(assign))
                        {
                            //TODO
                        }*/
                    }

                    else if (s.GetType() == typeof(while_node))
                    {
                        var whNode = (while_node)s;
                        var whileExpression = (bin_expr)whNode.expr;
                        var op = whileExpression.operation_type;
                        var leftEx = (ident)whileExpression.left;
                        if (whileExpression.right.GetType() == typeof(int32_const))
                        {
                            var rightEx = (int32_const)whileExpression.right;
                            tree.AddChild($"while {leftEx.name} {op} {rightEx.val}");
                        }
                        else if (whileExpression.right.GetType() == typeof(bool_const))
                        {
                            var rightEx = (bool_const)whileExpression.right;
                            tree.AddChild($"while {leftEx.name} {op} {rightEx.val}");
                        }
                        else if (whileExpression.right.GetType() == typeof(ident))
                        {
                            var rightEx = (ident)whileExpression.right;
                            tree.AddChild($"while {leftEx.name} {op} {rightEx.name}");
                        }


                    }
                }
            }

            else Console.WriteLine("UnDef: " + LastWord(n.ToString()));
        }

        public static String ParseCondition(bin_expr condition)
        {
            String s = "";
            bin_expr tempCondition = condition;
            bool isOr = false;
            String inp = "";
            if (condition.operation_type != Operators.LogicalOR && condition.operation_type != Operators.LogicalAND)
            {
                if (condition.left.GetType() == typeof(ident))
                {
                    var idnt = (ident)condition.left;
                    s += idnt.name;
                }
                else if (condition.left.GetType() == typeof(int32_const))
                {
                    var number = (int32_const)condition.left;
                    s += number.val;
                }

                s += " " + condition.operation_type.ToString() + " ";
                if (condition.right.GetType() == typeof(ident))
                {
                    var idnt = (ident)condition.right;
                    s += idnt.name;
                }
                else if (condition.right.GetType() == typeof(int32_const))
                {
                    var number = (int32_const)condition.right;
                    s += number.val;
                }

                return s;
            }
            if (condition.operation_type == Operators.LogicalOR)
            {
                condition = (bin_expr)condition.left;
                isOr = true;
            }


            while (true)
            {
                if (condition.operation_type == Operators.LogicalAND)
                {
                    var leftCondition = (bin_expr)condition.left;
                    var rightCondition = (bin_expr)condition.right;
                    if (leftCondition.operation_type != Operators.LogicalAND && leftCondition.operation_type != Operators.LogicalOR && rightCondition.operation_type != Operators.LogicalOR && rightCondition.operation_type != Operators.LogicalAND)
                    {
                        if (leftCondition.left.GetType() == typeof(ident))
                        {
                            var idnt = (ident)leftCondition.left;
                            s += idnt.name;
                        }
                        else if (leftCondition.left.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)leftCondition.left;
                            s += number.val;
                        }

                        s += " " + leftCondition.operation_type.ToString() + " ";
                        if (leftCondition.right.GetType() == typeof(ident))
                        {
                            var idnt = (ident)leftCondition.right;
                            s += idnt.name;
                        }
                        else if (leftCondition.right.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)leftCondition.right;
                            s += number.val;
                        }
                        s += "\n";
                        s += "and\n";
                        if (rightCondition.left.GetType() == typeof(ident))
                        {
                            var idnt = (ident)rightCondition.left;
                            s += idnt.name;
                        }
                        else if (rightCondition.left.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)rightCondition.left;
                            s += number.val;
                        }

                        s += " " + rightCondition.operation_type.ToString() + " ";
                        if (rightCondition.right.GetType() == typeof(ident))
                        {
                            var idnt = (ident)rightCondition.right;
                            s += idnt.name;
                        }
                        else if (rightCondition.right.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)rightCondition.right;
                            s += number.val;
                        }
                        break;
                    }
                    else if (leftCondition.operation_type == Operators.LogicalAND && rightCondition.operation_type != Operators.LogicalOR && rightCondition.operation_type != Operators.LogicalAND)
                    {
                        if (rightCondition.left.GetType() == typeof(ident))
                        {
                            var idnt = (ident)rightCondition.left;
                            s += idnt.name;
                        }
                        else if (rightCondition.left.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)rightCondition.left;
                            s += number.val;
                        }

                        s += " " + rightCondition.operation_type.ToString() + " ";
                        if (rightCondition.right.GetType() == typeof(ident))
                        {
                            var idnt = (ident)rightCondition.right;
                            s += idnt.name;
                        }
                        else if (rightCondition.right.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)rightCondition.right;
                            s += number.val;
                        }
                        s += "\n";
                        s += "and\n";
                        condition = leftCondition;
                    }
                    else
                    {
                        s = "error";
                        break;
                    }
                }
                else if (condition.operation_type == Operators.LogicalOR)
                {
                    var leftCondition = (bin_expr)condition.left;
                    var rightCondition = (bin_expr)condition.right;
                    if (leftCondition.operation_type == Operators.LogicalOR && rightCondition.operation_type == Operators.LogicalAND)
                    {
                        String tempStr = ParseCondition(rightCondition);
                        s += tempStr;
                        condition = leftCondition;
                        s += "\nor\n";
                    }
                    else if (leftCondition.operation_type != Operators.LogicalOR && leftCondition.operation_type != Operators.LogicalAND && rightCondition.operation_type == Operators.LogicalAND)
                    {
                        String tempStr = ParseCondition(rightCondition);
                        s += tempStr;
                        condition = leftCondition;
                        s += "\nor\n";
                    }
                    else if (leftCondition.operation_type == Operators.LogicalOR && rightCondition.operation_type != Operators.LogicalOR)
                    {
                        if (rightCondition.left.GetType() == typeof(ident))
                        {
                            var idnt = (ident)rightCondition.left;
                            s += idnt.name;
                        }
                        else if (rightCondition.left.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)rightCondition.left;
                            s += number.val;
                        }

                        s += " " + rightCondition.operation_type.ToString() + " ";
                        if (rightCondition.right.GetType() == typeof(ident))
                        {
                            var idnt = (ident)rightCondition.right;
                            s += idnt.name;
                        }
                        else if (rightCondition.right.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)rightCondition.right;
                            s += number.val;
                        }
                        s += "\nor\n";
                        condition = (bin_expr)condition.left;
                    }
                    else if (leftCondition.operation_type == Operators.LogicalAND && rightCondition.operation_type != Operators.LogicalOR && rightCondition.operation_type != Operators.LogicalAND)
                    {
                        if (rightCondition.left.GetType() == typeof(ident))
                        {
                            var idnt = (ident)rightCondition.left;
                            s += idnt.name;
                        }
                        else if (rightCondition.left.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)rightCondition.left;
                            s += number.val;
                        }

                        s += " " + rightCondition.operation_type.ToString() + " ";
                        if (rightCondition.right.GetType() == typeof(ident))
                        {
                            var idnt = (ident)rightCondition.right;
                            s += idnt.name;
                        }
                        else if (rightCondition.right.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)rightCondition.right;
                            s += number.val;
                        }
                        s += "\nor\n";
                        condition = (bin_expr)condition.left;
                    }
                    else if (leftCondition.operation_type != Operators.LogicalAND &&
                             leftCondition.operation_type != Operators.LogicalOR &&
                             rightCondition.operation_type != Operators.LogicalOR &&
                             rightCondition.operation_type != Operators.LogicalAND)
                    {
                        if (leftCondition.left.GetType() == typeof(ident))
                        {
                            var idnt = (ident)leftCondition.left;
                            s += idnt.name;
                        }
                        else if (leftCondition.left.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)leftCondition.left;
                            s += number.val;
                        }

                        s += " " + leftCondition.operation_type.ToString() + " ";
                        if (leftCondition.right.GetType() == typeof(ident))
                        {
                            var idnt = (ident)leftCondition.right;
                            s += idnt.name;
                        }
                        else if (leftCondition.right.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)leftCondition.right;
                            s += number.val;
                        }
                        s += "\n";
                        s += "or\n";
                        if (rightCondition.left.GetType() == typeof(ident))
                        {
                            var idnt = (ident)rightCondition.left;
                            s += idnt.name;
                        }
                        else if (rightCondition.left.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)rightCondition.left;
                            s += number.val;
                        }

                        s += " " + rightCondition.operation_type.ToString() + " ";
                        if (rightCondition.right.GetType() == typeof(ident))
                        {
                            var idnt = (ident)rightCondition.right;
                            s += idnt.name;
                        }
                        else if (rightCondition.right.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)rightCondition.right;
                            s += number.val;
                        }
                        break;
                    }
                }
                else if (condition.operation_type != Operators.LogicalAND &&
                         condition.operation_type != Operators.LogicalOR)
                {
                    if (condition.left.GetType() == typeof(ident))
                    {
                        var idnt = (ident)condition.left;
                        s += idnt.name;
                    }
                    else if (condition.left.GetType() == typeof(int32_const))
                    {
                        var number = (int32_const)condition.left;
                        s += number.val;
                    }

                    s += " " + condition.operation_type.ToString() + " ";
                    if (condition.right.GetType() == typeof(ident))
                    {
                        var idnt = (ident)condition.right;
                        s += idnt.name;
                    }
                    else if (condition.right.GetType() == typeof(int32_const))
                    {
                        var number = (int32_const)condition.right;
                        s += number.val;
                    }
                    break;
                }

            }

            condition = tempCondition;
            condition = (bin_expr)condition.right;
            if (isOr)
            {
                s += "\nor\n";
                while (true)
                {
                    if (condition.operation_type == Operators.LogicalAND)
                    {
                        var leftCondition = (bin_expr)condition.left;
                        var rightCondition = (bin_expr)condition.right;
                        if (leftCondition.operation_type != Operators.LogicalAND && leftCondition.operation_type != Operators.LogicalOR && rightCondition.operation_type != Operators.LogicalOR && rightCondition.operation_type != Operators.LogicalAND)
                        {
                            if (leftCondition.left.GetType() == typeof(ident))
                            {
                                var idnt = (ident)leftCondition.left;
                                s += idnt.name;
                            }
                            else if (leftCondition.left.GetType() == typeof(int32_const))
                            {
                                var number = (int32_const)leftCondition.left;
                                s += number.val;
                            }

                            s += " " + leftCondition.operation_type.ToString() + " ";
                            if (leftCondition.right.GetType() == typeof(ident))
                            {
                                var idnt = (ident)leftCondition.right;
                                s += idnt.name;
                            }
                            else if (leftCondition.right.GetType() == typeof(int32_const))
                            {
                                var number = (int32_const)leftCondition.right;
                                s += number.val;
                            }
                            s += "\n";
                            s += "and\n";
                            if (rightCondition.left.GetType() == typeof(ident))
                            {
                                var idnt = (ident)rightCondition.left;
                                s += idnt.name;
                            }
                            else if (rightCondition.left.GetType() == typeof(int32_const))
                            {
                                var number = (int32_const)rightCondition.left;
                                s += number.val;
                            }

                            s += " " + rightCondition.operation_type.ToString() + " ";
                            if (rightCondition.right.GetType() == typeof(ident))
                            {
                                var idnt = (ident)rightCondition.right;
                                s += idnt.name;
                            }
                            else if (rightCondition.right.GetType() == typeof(int32_const))
                            {
                                var number = (int32_const)rightCondition.right;
                                s += number.val;
                            }
                            break;
                        }
                        else if (leftCondition.operation_type == Operators.LogicalAND && rightCondition.operation_type != Operators.LogicalOR && rightCondition.operation_type != Operators.LogicalAND)
                        {
                            if (rightCondition.left.GetType() == typeof(ident))
                            {
                                var idnt = (ident)rightCondition.left;
                                s += idnt.name;
                            }
                            else if (rightCondition.left.GetType() == typeof(int32_const))
                            {
                                var number = (int32_const)rightCondition.left;
                                s += number.val;
                            }

                            s += " " + rightCondition.operation_type.ToString() + " ";
                            if (rightCondition.right.GetType() == typeof(ident))
                            {
                                var idnt = (ident)rightCondition.right;
                                s += idnt.name;
                            }
                            else if (rightCondition.right.GetType() == typeof(int32_const))
                            {
                                var number = (int32_const)rightCondition.right;
                                s += number.val;
                            }
                            s += "\n";
                            s += "and\n";
                            condition = leftCondition;
                        }
                        else
                        {
                            s = "error";
                            break;
                        }
                    }
                    else if (condition.operation_type == Operators.LogicalOR)
                    {
                        var leftCondition = (bin_expr)condition.left;
                        var rightCondition = (bin_expr)condition.right;
                        if (leftCondition.operation_type == Operators.LogicalOR && rightCondition.operation_type == Operators.LogicalAND)
                        {
                            String tempStr = ParseCondition(rightCondition);
                            s += tempStr;
                            condition = leftCondition;
                            s += "\nor\n";
                        }
                        else if (leftCondition.operation_type != Operators.LogicalOR && leftCondition.operation_type != Operators.LogicalAND && rightCondition.operation_type == Operators.LogicalAND)
                        {
                            String tempStr = ParseCondition(rightCondition);
                            s += tempStr;
                            condition = leftCondition;
                            s += "\nor\n";
                        }
                        else if (leftCondition.operation_type == Operators.LogicalOR && rightCondition.operation_type != Operators.LogicalOR)
                        {
                            if (rightCondition.left.GetType() == typeof(ident))
                            {
                                var idnt = (ident)rightCondition.left;
                                s += idnt.name;
                            }
                            else if (rightCondition.left.GetType() == typeof(int32_const))
                            {
                                var number = (int32_const)rightCondition.left;
                                s += number.val;
                            }

                            s += " " + rightCondition.operation_type.ToString() + " ";
                            if (rightCondition.right.GetType() == typeof(ident))
                            {
                                var idnt = (ident)rightCondition.right;
                                s += idnt.name;
                            }
                            else if (rightCondition.right.GetType() == typeof(int32_const))
                            {
                                var number = (int32_const)rightCondition.right;
                                s += number.val;
                            }
                            s += "\nor\n";
                            condition = (bin_expr)condition.left;
                        }
                        else if (leftCondition.operation_type == Operators.LogicalAND && rightCondition.operation_type != Operators.LogicalOR && rightCondition.operation_type != Operators.LogicalAND)
                        {
                            if (rightCondition.left.GetType() == typeof(ident))
                            {
                                var idnt = (ident)rightCondition.left;
                                s += idnt.name;
                            }
                            else if (rightCondition.left.GetType() == typeof(int32_const))
                            {
                                var number = (int32_const)rightCondition.left;
                                s += number.val;
                            }

                            s += " " + rightCondition.operation_type.ToString() + " ";
                            if (rightCondition.right.GetType() == typeof(ident))
                            {
                                var idnt = (ident)rightCondition.right;
                                s += idnt.name;
                            }
                            else if (rightCondition.right.GetType() == typeof(int32_const))
                            {
                                var number = (int32_const)rightCondition.right;
                                s += number.val;
                            }
                            s += "\nor\n";
                            condition = (bin_expr)condition.left;
                        }
                        else if (leftCondition.operation_type != Operators.LogicalAND &&
                                 leftCondition.operation_type != Operators.LogicalOR &&
                                 rightCondition.operation_type != Operators.LogicalOR &&
                                 rightCondition.operation_type != Operators.LogicalAND)
                        {
                            if (leftCondition.left.GetType() == typeof(ident))
                            {
                                var idnt = (ident)leftCondition.left;
                                s += idnt.name;
                            }
                            else if (leftCondition.left.GetType() == typeof(int32_const))
                            {
                                var number = (int32_const)leftCondition.left;
                                s += number.val;
                            }

                            s += " " + leftCondition.operation_type.ToString() + " ";
                            if (leftCondition.right.GetType() == typeof(ident))
                            {
                                var idnt = (ident)leftCondition.right;
                                s += idnt.name;
                            }
                            else if (leftCondition.right.GetType() == typeof(int32_const))
                            {
                                var number = (int32_const)leftCondition.right;
                                s += number.val;
                            }
                            s += "\n";
                            s += "or\n";
                            if (rightCondition.left.GetType() == typeof(ident))
                            {
                                var idnt = (ident)rightCondition.left;
                                s += idnt.name;
                            }
                            else if (rightCondition.left.GetType() == typeof(int32_const))
                            {
                                var number = (int32_const)rightCondition.left;
                                s += number.val;
                            }

                            s += " " + rightCondition.operation_type.ToString() + " ";
                            if (rightCondition.right.GetType() == typeof(ident))
                            {
                                var idnt = (ident)rightCondition.right;
                                s += idnt.name;
                            }
                            else if (rightCondition.right.GetType() == typeof(int32_const))
                            {
                                var number = (int32_const)rightCondition.right;
                                s += number.val;
                            }
                            break;
                        }
                    }
                    else if (condition.operation_type != Operators.LogicalAND &&
                             condition.operation_type != Operators.LogicalOR)
                    {
                        if (condition.left.GetType() == typeof(ident))
                        {
                            var idnt = (ident)condition.left;
                            s += idnt.name;
                        }
                        else if (condition.left.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)condition.left;
                            s += number.val;
                        }

                        s += " " + condition.operation_type.ToString() + " ";
                        if (condition.right.GetType() == typeof(ident))
                        {
                            var idnt = (ident)condition.right;
                            s += idnt.name;
                        }
                        else if (condition.right.GetType() == typeof(int32_const))
                        {
                            var number = (int32_const)condition.right;
                            s += number.val;
                        }
                        s += "\n";
                        s += "\n";
                        break;
                    }
                }
            }


            return s;


        }

        public static void PrintNextNode(statement n, TreeNode<string> tree, int i)
        {
            statement_list sl = new statement_list();
            sl.Add(n);
            PrintNode(sl, tree);
        }



    }
}
