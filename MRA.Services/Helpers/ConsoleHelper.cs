using FirebaseAdmin.Messaging;
using MRA.DTO.Firebase.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.Helpers
{
    public class ConsoleHelper
    {

        private int PAD_RIGHT = 10;

        public void ShowMessageInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("INFO: ".PadRight(PAD_RIGHT) + message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ShowMessageWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("WARNING: ".PadRight(PAD_RIGHT) + message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void ShowMessageError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("DANGER: ".PadRight(PAD_RIGHT) + message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ShowMessagePrevious(bool isNew, string previous)
        {
            if (!isNew)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("".PadRight(PAD_RIGHT));
                Console.Write("[Default: '");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(previous);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("'][Leave empty to use it]");

                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public void ShowMessage(bool isNew, string previous, string field)
        {
            Console.ForegroundColor = ConsoleColor.White;
            ShowMessageInfo("------------------------------------------------------");
            System.Console.WriteLine("INPUT:".PadRight(PAD_RIGHT) + field.ToUpper());
            ShowMessagePrevious(isNew, previous);
        }

        public string ReadValueFromConsole()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("> ");
            return Console.ReadLine();
        }

        public string ReadValue(bool isNew, string previous)
        {

            var input = ReadValueFromConsole();
            if (!isNew && String.IsNullOrEmpty(input))
            {
                input = previous;
            }
            ShowValueSet(input.ToString());
            return input;
        }

        public int FillIntValue(bool isNew, int previous, string field, Dictionary<int, string> dictionary)
        {
            ShowMessageInfo("------------------------------------------------------");
            ShowMessageInfo(field.ToUpper() + ":");

            foreach (var type in dictionary)
            {
                ShowMessageInfo(type.Key.ToString().PadRight(5) + "= " + type.Value);
            }

            if (!isNew)
            {
                System.Console.WriteLine("  ");
                ShowMessagePrevious(isNew, dictionary[previous].ToString());
            }

            var input = ReadValueFromConsole();
            if (!isNew && String.IsNullOrEmpty(input))
            {
                ShowValueSet(dictionary[previous]);
                return previous;
            }
            else
            {
                int.TryParse(input, out int numeroEntero);
                ShowValueSet(dictionary[numeroEntero]);
                return numeroEntero;
            }
        }

        public int FillFreeIntValue(bool isNew, int previous, string field)
        {
            ShowMessage(isNew, previous.ToString(), field);

            var input = ReadValueFromConsole();
            if (!isNew && String.IsNullOrEmpty(input))
            {
                ShowValueSet(previous.ToString());
                return previous;
            }
            else
            {
                int.TryParse(input, out int numeroEntero);
                ShowValueSet(numeroEntero.ToString());
                return numeroEntero;
            }
        }


        public bool FillBoolValue(bool isNew, bool previous, string field)
        {
            ShowMessage(isNew, previous.ToString(), field);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            System.Console.WriteLine("".PadRight(PAD_RIGHT) + "[Y|y: 'True']");
            Console.ForegroundColor = ConsoleColor.White;

            var input = ReadValueFromConsole();

            if (!isNew && String.IsNullOrEmpty(input))
            {
                ShowValueSet(previous.ToString());
                return previous;
            }
            else
            {
                bool value = false;
                if (input.ToLower().Equals("y"))
                {
                    value = true;
                }

                ShowValueSet(value.ToString());
                return value;
            }
        }

        public void ShowValueSet(string value)
        {
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("VALUE ".PadRight(PAD_RIGHT));
            Console.WriteLine((String.IsNullOrEmpty(value) ? "''" : value));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("");
        }

        public string FillStringValue(bool isNew, string previous, string field)
        {
            ShowMessage(isNew, previous, field);
            return ReadValue(isNew, previous);
        }

        public void PrintPropreties(object obj)
        {
            // Obtener todas las propiedades
            PropertyDescriptorCollection propiedades = TypeDescriptor.GetProperties(obj);

            // Ordenar las propiedades por nombre
            var propiedadesOrdenadas = propiedades.Sort()
                                                  .Cast<PropertyDescriptor>()
                                                  .ToList();

            var propiedadesOrdenadas2 = new PropertyDescriptorCollection(propiedadesOrdenadas.ToArray());

            foreach (PropertyDescriptor descriptor in propiedadesOrdenadas2)
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(obj) ?? "";
                ShowMessageInfo(name.PadRight(20) + " = " + value);
            }
        }
    }
}
