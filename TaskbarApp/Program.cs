using System;
using System.Windows.Forms;

namespace TaskbarApp
{
    internal static class Program
    {
        [STAThread]
        static void Main() =>
            Application.Run(new TaskbarApp());
    }
}
