using MRA.Services.Firebase.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.Console
{
    internal class ConsoleHelper
    {
        public static string SAME_VALUE = "s";

        internal void ShowMessage(bool isNew, string previous, string field)
        {
            System.Console.WriteLine("------------------------------------------------------");
            System.Console.WriteLine(field+":");

            if (!isNew)
            {
                System.Console.WriteLine("  [Previous: "+previous+"]");
                System.Console.WriteLine("  [Type '" + SAME_VALUE + "' to keep previous]");
            }
        }

        internal string ReadValue(bool isNew, string previous)
        {
            var input = System.Console.ReadLine();
            var value = (isNew ? input : (SAME_VALUE.Equals(input) ? previous : input));
            System.Console.WriteLine(" * Value set: "+value);
            return value;
        }

        internal int FillIntValue(bool isNew, int previous, string field, Dictionary<int, string> dictionary)
        {
            System.Console.WriteLine("------------------------------------------------------");
            System.Console.WriteLine(field + ":");

            foreach(var type in dictionary)
            {
                System.Console.WriteLine($" - {type.Key}: {type.Value}");
            }

            if (!isNew)
            {
                System.Console.WriteLine("  ");
                System.Console.WriteLine("  [Previous: " + dictionary[previous] + "]");
                System.Console.WriteLine("  [Type '" + SAME_VALUE + "' to keep previous]");
            }

            var input = System.Console.ReadLine();
            if (!isNew && input.Equals(ConsoleHelper.SAME_VALUE))
            {
                System.Console.WriteLine(" * Value set: " + dictionary[previous]);
                return previous;
            }
            else
            {
                int.TryParse(input, out int numeroEntero);
                return numeroEntero;
            }
        }

        internal string FillStringValue(bool isNew, string previous, string field)
        {
            ShowMessage(isNew, previous, field);
            return ReadValue(isNew, previous);
        }

        internal void PrintPropreties(object obj)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(obj);
                System.Console.WriteLine("  {0}={1}", name, value);
            }
        }
    }
}
