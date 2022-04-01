using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Begin;

namespace TestBegin
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Code: ");
            string input = Console.ReadLine();
            while (input != "q")
            {
                Begin.Begin b = new Begin.Begin(input);
                Console.WriteLine(b.Evaluate());

                Console.Write("Code: ");
                input = Console.ReadLine();
            }
        }
    }
}
