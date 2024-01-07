using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.MAUI.Classes
{
    internal class CommentEventArgs : EventArgs
    {
        public int TipoLista { get; }
        public int Indice { get; }

        public CommentEventArgs(int tipo, int index)
        {
            TipoLista = tipo;
            Indice = index;
        }
    }
}
