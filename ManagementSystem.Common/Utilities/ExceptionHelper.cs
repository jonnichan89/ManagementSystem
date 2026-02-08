namespace ManagementSystem.Common.Utilities
{
    public static class ExceptionHelper
    {
        public static string GetErrorMessage(Exception ex, bool retrieveStackTrace)
        {
            string stackTrace = ex.StackTrace;

            string message = "";
            while (ex != null)
            {
                message += ex.Message + Environment.NewLine + Environment.NewLine;
                ex = ex.InnerException;
            }

            if (retrieveStackTrace)
                return message + Environment.NewLine + Environment.NewLine +
                    stackTrace;
            else
                return message;
        }
    }
}
