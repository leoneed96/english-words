using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.WPF
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
    public class ErrorHandler : IErrorHandler
    {
        public void HandleError(Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }
}
