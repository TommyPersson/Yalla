using System;
using System.IO;
using System.Text;

namespace SwankServer
{
    public class ObservableStringWriter : StringWriter
    {
        public ObservableStringWriter(StringBuilder sb) : base(sb)
        {
        }
        
        public delegate void FlushedEventHandler(object sender, EventArgs args);
        
        public event FlushedEventHandler Flushed;
        
        public override void Flush()
        {
            base.Flush();
            
            if (Flushed != null)
            {
                Flushed(this, EventArgs.Empty);
            }
        }
    }
}

