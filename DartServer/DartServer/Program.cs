using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var server = new Server();
            }
            catch(Exception e)
            {
                Console.WriteLine("Server crashed. " + e.Message);
            }
        }
    }
}
