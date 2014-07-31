using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace O3DRender
{
    public class O3DRenderException : ApplicationException
    {
        public O3DRenderException(string message) : base(message) { }
    }
}
