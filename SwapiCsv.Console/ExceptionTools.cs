using System;
using System.Collections.Generic;
using System.Text;

namespace SwapiCsv.ConsoleUI
{
    public class ExceptionTools
    {
        public static string BundleExceptionMessages(Exception exception)
        {
            string bundledMessages = exception.Message;
            if (exception.InnerException != null)
            {
                bundledMessages += BundleExceptionMessages(exception.InnerException);
            }

            return bundledMessages;
        }
    }
}
