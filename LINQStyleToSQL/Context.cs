using System;
using System.Collections.Generic;
using System.Text;

namespace LINQStyleToSQL
{
    public class Context
    {
        public IEnumerable<Student> Students { get; set; }

        public Context()
        {

        }
    }
}
