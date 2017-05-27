using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Cloud_Oauth
{
    class Program
    {
        const string ClosePageResponse =
@"<html>
  <head><title>OAuth 2.0 Authentication Token Received</title></head>
  <body>
    Received verification code.
    <script type='text/javascript'>
      // This doesn't work on every browser.
      window.setTimeout(function() {
          window.open('', '_self', ''); 
          window.close(); 
        }, 1000);
      if (window.opener) { window.opener.checkToken(); }
    </script>
  </body>
</html>";
        static HttpListener listener;
        static Wait form;
        static int timeout = 5 * 60 * 1000;
        static Process parrent_process;
        [STAThread]
        static void Main(string[] args)
        {
            parrent_process = ParentProcessUtilities.GetParentProcess(Process.GetCurrentProcess().Handle);
            if (parrent_process != null && args != null && args.Length >= 1 && args[0].Length > 0)
            {
                try
                {
                    listener = new HttpListener();
                    listener.Prefixes.Add(args[0]);
                    listener.Start();
                    listener.BeginGetContext(new AsyncCallback(RecieveCode), null);
                    if (args.Length >= 2) int.TryParse(args[1], out timeout);
                    parrent_process.EnableRaisingEvents = true;
                    parrent_process.Exited += Parrent_process_Exited;
                    form = new Wait(timeout);
                    form.ShowDialog();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message + " - " + ex.StackTrace);
                }
            }
            else Console.WriteLine("Arg error");
        }

        static void Parrent_process_Exited(object sender, EventArgs e)
        {
            form.CloseForm();
        }

        static void RecieveCode(IAsyncResult rs)
        {
            HttpListenerContext ls = listener.EndGetContext(rs);
            using (var writer = new StreamWriter(ls.Response.OutputStream))
            {
                writer.WriteLine(ClosePageResponse);
                writer.Flush();
            }
            ls.Response.OutputStream.Close();
            form.CloseForm();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ParentProcessUtilities
    {
        // These members must match PROCESS_BASIC_INFORMATION
        internal IntPtr Reserved1;
        internal IntPtr PebBaseAddress;
        internal IntPtr Reserved2_0;
        internal IntPtr Reserved2_1;
        internal IntPtr UniqueProcessId;
        internal IntPtr InheritedFromUniqueProcessId;

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);

        public static Process GetParentProcess(IntPtr handle)
        {
            ParentProcessUtilities pbi = new ParentProcessUtilities();
            int returnLength;
            int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
            if (status != 0)
                return null;

            try
            {
                return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
            }
            catch (ArgumentException)
            {
                // not found
                return null;
            }
        }
    }

    public class Wait : Form
    {
        public System.Threading.Timer timer;
        public Wait(int timeout = 60000)
        {
            this.ShowInTaskbar = false;
            this.Opacity = 0;
            timer = new System.Threading.Timer(new TimerCallback(TimerCallBack), null, timeout, timeout);
        }

        void TimerCallBack(object obj)
        {
            Console.WriteLine("timeout");
            CloseForm();
        }

        public void CloseForm()
        {
            if (InvokeRequired) Invoke(new Action(() => this.Close()));
            else this.Close();
        }

        /// <summary>
        /// Hide form in alt tab
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var Params = base.CreateParams;
                Params.ExStyle |= 0x80;
                return Params;
            }
        }
    }
}
