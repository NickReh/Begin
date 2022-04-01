using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Begin
{
    class BeginVisitor : BeginBaseVisitor<object>
    {
        private int stackCounter;
        private string totalReturn = "";
        private List<Function> functions = new List<Function>();
        private List<Dictionary<string, object>> functionStack = new List<Dictionary<string, object>>();

        public BeginVisitor(int sCounter)
        {
            stackCounter = sCounter;
        }

        public override object VisitCode(BeginParser.CodeContext context)
        {
            for(int i = 0; i < context.children.Count(); i++){
                if (context.block(i) != null)
                {
                    totalReturn += VisitBlock(context.block(i)) + "\n";
                }
            }
            return totalReturn;
        }

        public override object VisitStmnt(BeginParser.StmntContext context)
        {
            return VisitStatement(context.statement());
        }

        public override object VisitAssgVar(BeginParser.AssgVarContext context)
        {
            return "assgmnt: " + context.CONTEXTVALUE().GetText() + " = " + VisitLogicalExpression(context.logicalExpression());
        }

        //public override object VisitExpression(BeginParser.ExpressionContext context)
        //{
        //    return VisitLogicalExpression(context.logicalExpression());
        //}

        public override object VisitLogOr(BeginParser.LogOrContext context)
        {
            bool left = Convert.ToBoolean(Visit(context.logicalOr(0)));
            bool right = Convert.ToBoolean(Visit(context.logicalOr(1)));
            return left || right;
        }

        public override object VisitLogAnd(BeginParser.LogAndContext context)
        {
            bool left = Convert.ToBoolean(Visit(context.logicalAnd(0)));
            bool right = Convert.ToBoolean(Visit(context.logicalAnd(1)));
            return left && right;
        }

        public override object VisitEqNoteq(BeginParser.EqNoteqContext context)
        {
            object left = Visit(context.equalityExpression(0));
            object right = Visit(context.equalityExpression(1));
            if (context.op.Type == BeginParser.EQUALS)
            {
                return Comparer.Default.Compare(left, Convert.ChangeType(right, left.GetType())) == 0;
            }
            else //not equal
            {
                return Comparer.Default.Compare(left, Convert.ChangeType(right, left.GetType())) != 0;
            }
        }

        public override object VisitLTGT(BeginParser.LTGTContext context)
        {
            object left = Visit(context.relationalExpression(0));
            object right = Visit(context.relationalExpression(1));
            if (context.op.Type == BeginParser.LT)
            {
                return Comparer.Default.Compare(left, Convert.ChangeType(right, left.GetType())) < 0;
            }
            else if (context.op.Type == BeginParser.LTEQ)
            {
                return Comparer.Default.Compare(left, Convert.ChangeType(right, left.GetType())) <= 0;
            }
            else if (context.op.Type == BeginParser.GT)
            {
                return Comparer.Default.Compare(left, Convert.ChangeType(right, left.GetType())) > 0;
            }
            else //gteq
            {
                return Comparer.Default.Compare(left, Convert.ChangeType(right, left.GetType())) >= 0;
            }
        }

        public override object VisitMulDivMod(BeginParser.MulDivModContext context)
        {
            decimal left = Convert.ToDecimal(Visit(context.numberExpression(0)));
            decimal right = Convert.ToDecimal(Visit(context.numberExpression(1)));
            if (context.op.Type == BeginParser.MULT)
            {
                return left * right;
            }
            else if (context.op.Type == BeginParser.DIV)
            {
                return left / right;
            }
            else //modulo
            {
                return Convert.ToDecimal(Visit(context.numberExpression(0))) % Convert.ToDecimal(Visit(context.numberExpression(1)));
            }
        }

        public override object VisitPlMin(BeginParser.PlMinContext context)
        {
            if (context.op.Type == BeginParser.PLUS)
            {
                decimal l = 0, r = 0;
                if (decimal.TryParse(Visit(context.numberExpression(0)).ToString(), out l) && decimal.TryParse(Visit(context.numberExpression(1)).ToString(), out r))
                {
                    return l + r;
                }
                else
                {
                    return Visit(context.numberExpression(0)).ToString() + Visit(context.numberExpression(1)).ToString();
                }
            }
            else //Minus
            {
                return Convert.ToDecimal(Visit(context.numberExpression(0))) - Convert.ToDecimal(Visit(context.numberExpression(1)));
            }
        }

        public override object VisitNegEx(BeginParser.NegExContext context)
        {
            if (context.op.Type == BeginParser.NOT)
            {
                bool ex = Convert.ToBoolean(Visit(context.primaryExpression()));
                return !ex;
            }
            else
            {
                decimal dec = Convert.ToDecimal(Visit(context.primaryExpression()));
                return dec * -1;
            }
        }

        public override object VisitIncEx(BeginParser.IncExContext context)
        {
            return int.Parse(context.INTEGER().GetText()) + 1;
        }

        public override object VisitDecEx(BeginParser.DecExContext context)
        {
            return int.Parse(context.INTEGER().GetText()) - 1;
        }

        public override object VisitParens(BeginParser.ParensContext context)
        {
            return Visit(context.logicalExpression());
        }

        public override object VisitInt(BeginParser.IntContext context)
        {
            return int.Parse(context.GetText());
        }

        public override object VisitFloat(BeginParser.FloatContext context)
        {
            return decimal.Parse(context.GetText());
        }

        public override object VisitBool(BeginParser.BoolContext context)
        {
            return bool.Parse(context.GetText());
        }

        public override object VisitStr(BeginParser.StrContext context)
        {
            //string s = context.GetText();
            //return s;//.Substring(1, s.Length - 2);
            string text = context.GetText();
            StringBuilder sb = new StringBuilder(text);
            int startIndex = 1; // Skip initial quote
            int slashIndex = -1;

            while ((slashIndex = sb.ToString().IndexOf('\\', startIndex)) != -1)
            {
                char escapeType = sb[slashIndex + 1];
                switch (escapeType)
                {
                    case 'u':
                        string hcode = String.Concat(sb[slashIndex + 4], sb[slashIndex + 5]);
                        string lcode = String.Concat(sb[slashIndex + 2], sb[slashIndex + 3]);
                        char unicodeChar = Encoding.Unicode.GetChars(new byte[] { System.Convert.ToByte(hcode, 16), System.Convert.ToByte(lcode, 16) })[0];
                        sb.Remove(slashIndex, 6).Insert(slashIndex, unicodeChar);
                        break;
                    case 'n': sb.Remove(slashIndex, 2).Insert(slashIndex, '\n'); break;
                    case 'r': sb.Remove(slashIndex, 2).Insert(slashIndex, '\r'); break;
                    case 't': sb.Remove(slashIndex, 2).Insert(slashIndex, '\t'); break;
                    case '\'': sb.Remove(slashIndex, 2).Insert(slashIndex, '\''); break;
                    case '\\': sb.Remove(slashIndex, 2).Insert(slashIndex, '\\'); break;
                    default: throw new ApplicationException("Unvalid escape sequence: \\" + escapeType);
                }

                startIndex = slashIndex + 1;

            }

            sb.Remove(0, 1);
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        public override object VisitFunctionDecl(BeginParser.FunctionDeclContext context)
        {
            //parameters go <name, type> so key is unique
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            int i = 3;
            while (context.children[i].GetText() != ")")
            {
                parameters.Add(context.children[i + 1].GetText(), context.children[i].GetText());
                i += 2;
            }

            bool newFunctionOk = true;
            foreach(Function func in functions.Where(f => f.name == context.children[1].GetText()))
            {
                if(!newFunctionOk){break;}
                if (func.parameters.Count() == parameters.Count())
                {
                    if (parameters.Count() == 0)
                    {
                        newFunctionOk = false;
                        break;
                    }
                    bool paramsStillMatch = true;
                    for (int p = 0; p < func.parameters.Count(); p++)
                    {
                        if (func.parameters.ElementAt(p).Value != parameters.ElementAt(p).Value)
                        {
                            paramsStillMatch = false;
                            break;
                        }
                    }
                    if (paramsStillMatch)
                    {
                        newFunctionOk = false;
                        break;
                    }
                }
            }

            if (!newFunctionOk)
            {
                throw new Exception("Function Signiture Already Exists!");
            }

            Function newFunction = new Function(context.children[1].GetText(), parameters, context.functionBody());
            functions.Add(newFunction);

            return "func decl";
        }

        public override object VisitFuncCall(BeginParser.FuncCallContext context)
        {
            stackCounter++;
            if(stackCounter > 60)
            {
                throw new Exception("Infinite Loop or Stack Overflow Error. Please reduce the number of nested function calls.");
            }

            List<object> parameters = new List<object>();
            object ret;
            foreach (var child in context.functionCall().children)
            {
                ret = Visit(child);
                if (ret != null)
                {
                    parameters.Add(ret);
                }
            }

            Function func = null;
            Dictionary<string, object> funcParamDic;
            foreach (Function f in functions.Where(x => x.name == context.functionCall().children[0].GetText() && x.parameters.Count() == parameters.Count()))
            {
                funcParamDic = new Dictionary<string, object>();
                bool paramsStillMatch = true;
                for (int p = 0; p < f.parameters.Count(); p++)
                {
                    try
                    {
                        Convert.ChangeType(parameters[p], Type.GetType(f.parameters.ElementAt(p).Value, true));
                    }
                    catch (Exception ex)
                    {
                        paramsStillMatch = false;
                        break;
                    }
                    funcParamDic.Add(f.parameters.ElementAt(p).Key, parameters[p]); //<name, value>
                }
                if (paramsStillMatch)
                {
                    func = f;
                    functionStack.Insert(0, funcParamDic);
                    break;
                }
            }

            if (func == null)
            {
                throw new Exception("Function '" + context.functionCall().children[0].GetText()  + "' not found!");
            }

            

            object result = null; //
            if (result != null)
            {
                stackCounter--;
                return result;
            }

            functionStack.RemoveAt(0);

            return "func call";
        }

        public override object VisitIfFunctionCall(BeginParser.IfFunctionCallContext context)
        {
            return "if call";
        }

        public override object VisitVarDecl(BeginParser.VarDeclContext context)
        {
            return "variable declaration";
        }
    }
}
