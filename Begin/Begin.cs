using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Begin
{
    public class Begin
    {
        private string expression;

        public Begin(string expression)
        {
            this.expression = expression;
        }

        public object Evaluate(int sCounter = 0)
        {
            IParseTree parsedTree = Parse(expression);
            BeginVisitor visitor = new BeginVisitor(sCounter);
            //visitor.EvaluationResolver = IdentityResolver;
            //visitor.Parameters = parameters;
            return visitor.Visit(parsedTree);
        }

        protected IParseTree Parse(string expression)
        {
            BeginLexer lexer = new BeginLexer(new AntlrInputStream(expression));
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            BeginParser parser = new BeginParser(tokens);
            try
            {
                IParseTree tree = parser.code(); //.expression();
                return tree;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
