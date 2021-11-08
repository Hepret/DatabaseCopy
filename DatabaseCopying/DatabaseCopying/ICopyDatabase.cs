using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopying
{
    public interface ICopyDatabase
    {
        void CopySchema();
        void CopyData();
        void Copy();
    }
}
