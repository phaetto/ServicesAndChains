namespace Services.Management.Tracing
{
    using System;
    using System.IO;

    internal sealed class EmergencyLogger
    {
        public void SaveToFile(Exception exception)
        {
            var filePath =
                $"{AppDomain.CurrentDomain.BaseDirectory}EmergencyLogger.{DateTime.UtcNow.ToString("yyyy.MM.dd.hh.mm.ss")}.txt";

            var contents = string.Empty;
            while (exception != null)
            {
                contents += FormatException(exception);
                exception = exception.InnerException;
            }

            File.AppendAllText(filePath, contents);
        }

        private string FormatException(Exception exception)
        {
            return Environment.NewLine + Environment.NewLine 
                + exception.GetType().FullName + Environment.NewLine + Environment.NewLine
                + exception.Message + Environment.NewLine + Environment.NewLine + exception.Source
                + Environment.NewLine + Environment.NewLine + exception.StackTrace + Environment.NewLine
                + Environment.NewLine;
        }
    }
}
