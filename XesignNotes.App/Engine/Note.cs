using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XesignNotes.App.Engine
{
    public class Note
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public NoteColor Color { get; set; }
    }

    public enum NoteColor
    {
        Monochrome,
        Blue,
        Orange,
        Green,
        Purple,
        Grey
    }
}
