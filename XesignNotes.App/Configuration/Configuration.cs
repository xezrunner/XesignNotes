using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XesignNotes.App.Configuration
{
    public class Configuration
    {
        public string Property { get; set; }

        private object _value;

        public object Value
        {
            get
            {
                if (_value != null)
                    return _value;
                else if (_value == null & DefaultValue != null)
                    return DefaultValue;
                else
                    return null;
            }
            set
            {
                _value = value;
            }
        }
        public object DefaultValue { get; set; }
    }
}
