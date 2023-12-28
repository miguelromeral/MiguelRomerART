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

        internal void ShowMessagePrevious(bool isNew, string previous)
        {
            if (!isNew)
            {
                System.Console.WriteLine("  [Previous: " + previous + "][Leave empty to use it]");
            }
        }

        internal void ShowMessage(bool isNew, string previous, string field)
        {
            System.Console.WriteLine("------------------------------------------------------");
            System.Console.WriteLine(field + ":");
            ShowMessagePrevious(isNew, previous);
        }
         
        internal string ReadValue(bool isNew, string previous)
        {
            var input = System.Console.ReadLine();
            if (!isNew && String.IsNullOrEmpty(input))
            {
                input = previous;
            }
            System.Console.WriteLine(" * Value set: "+input);
            return input;
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
                ShowMessagePrevious(isNew, dictionary[previous].ToString());
            }

            var input = System.Console.ReadLine();
            if (!isNew && String.IsNullOrEmpty(input))
            {
                System.Console.WriteLine(" * Value set: " + dictionary[previous]);
                return previous;
            }
            else
            {
                int.TryParse(input, out int numeroEntero);
                System.Console.WriteLine(" * Value set: " + dictionary[numeroEntero]);
                return numeroEntero;
            }
        }

        internal int FillFreeIntValue(bool isNew, int previous, string field)
        {
            ShowMessage(isNew, previous.ToString(), field);

            var input = System.Console.ReadLine();
            if (!isNew && String.IsNullOrEmpty(input))
            {
                System.Console.WriteLine(" * Value set: " + previous);
                return previous;
            }
            else
            {
                int.TryParse(input, out int numeroEntero);
                System.Console.WriteLine(" * Value set: " + numeroEntero);
                return numeroEntero;
            }
        }


        internal bool FillBoolValue(bool isNew, bool previous, string field)
        {
            ShowMessage(isNew, previous.ToString(), field);
            System.Console.WriteLine($"  [Type 'Y' or 'y' to set the value to 'True']");

            var input = System.Console.ReadLine();

            if (!isNew && String.IsNullOrEmpty(input))
            {
                System.Console.WriteLine(" * Value set: " + previous.ToString());
                return previous;
            }
            else
            {
                bool value = false;
                if (input.ToLower().Equals("y"))
                {
                    value = true;
                }
                System.Console.WriteLine(" * Value set: " + value.ToString());
                return value;
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
