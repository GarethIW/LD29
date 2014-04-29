using System;
using System.Windows.Forms;

namespace LD29
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                if (!System.Diagnostics.Debugger.IsAttached) currentDomain.UnhandledException += currentDomain_UnhandledException;
                game.Run();
            }

            
        }

        static void currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(((Exception)e.ExceptionObject).ToString());
            Environment.Exit(-1);
        }
    }
#endif
}

