using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.Runtime.InteropServices;
//using System.IO;
using Microsoft.Win32;
using System.Xml;
using Gma.System.MouseKeyHook;
using System.Windows.Interop;

namespace presser
{
    public partial class MainWindow : Window
    {
        KeyboardListener KListener = new KeyboardListener();
        bool isRunning = false, mouseTowasd = false;
        btnState button1State = btnState.off, button2State = btnState.off, 
            button3State= btnState.off, button4State = btnState.off,
            button5State = btnState.off, lClkState= btnState.off;
        int timer1,timer2,timer3,timer4, timer5, lClkTimer, xVal1,yVal1, xVal2, yVal2,
            xVal3, yVal3, xVal4, yVal4, xVal5, yVal5;
        Key b1Key,b2Key,b3Key,b4Key,b5Key,startKey,stopKey,pixelKey;
        Color color1, color2, color3, color4, color5;
        List<Task> tlist = new List<Task>();
        CancellationTokenSource bTokenSource; // = new CancellationTokenSource();
        string pressFocus;
        private IMouseEvents mEvents;
        public POINT prevPoint;
        public struct POINT
        {
            public float X;
            public float Y;
        }
        enum btnState { off,press,hold,pixelChange}
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr window);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern uint GetPixel(IntPtr dc, int x, int y);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ReleaseDC(IntPtr window, IntPtr dc);
        public int GetColorIntAt(int x, int y)
        {
            IntPtr desktopWindow = MainWindow.GetDesktopWindow();
            IntPtr windowDc = MainWindow.GetWindowDC(desktopWindow);
            int pixel = (int)MainWindow.GetPixel(windowDc, x, y);
            MainWindow.ReleaseDC(desktopWindow, windowDc);
            return pixel;
        }
        public Color GetColorAt(int x, int y)
        {
            IntPtr desktopWindow = MainWindow.GetDesktopWindow();
            IntPtr windowDc = MainWindow.GetWindowDC(desktopWindow);
            int pixel = (int)MainWindow.GetPixel(windowDc, x, y);
            MainWindow.ReleaseDC(desktopWindow, windowDc);
            return Color.FromArgb(byte.MaxValue, Convert.ToByte(pixel & (int)byte.MaxValue), Convert.ToByte(pixel >> 8 & (int)byte.MaxValue), Convert.ToByte(pixel >> 16 & (int)byte.MaxValue));
        }
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT currPoint);
        private void mouseSub()
        {
            writeToTbIn("sub");
            mEvents = Hook.GlobalEvents();
            mEvents.MouseMoveExt += mouseMv;
        }
        private void mouseMv(Object sender,MouseEventExtArgs e)
        {
            //todo timer
            if (mouseTowasd==true)
            {
                bTokenSource = new CancellationTokenSource();
                CancellationToken bToken = bTokenSource.Token;
                string temp = "";
                //currPoint= new Point();
                POINT p = new POINT();

                if (GetCursorPos(out p))
                {
                    //prevPoint = PointToScreen(currPoint);
                    writeToTbIn(p.Y.ToString());
                    if (p.X < prevPoint.X)
                    { 
                        temp = "L"; 
                    }
                    if (p.X > prevPoint.X)
                    { 
                        temp += "R"; 
                    }
                    if (p.Y < prevPoint.Y)
                    {
                        temp += "U";
                        holdKey(Key.W, 100, bToken);
                    }
                    if (p.Y > prevPoint.Y)
                    { temp += "D"; }
 
                        writeToTbOut(temp);
                }
                prevPoint = p;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);
            pressFocus = "*test.txt - Notepad";
            //pressFocus = "Time Clickers";
            //pressFocus = "Diablo III";
            b1Key = Key.D1;
            b2Key = Key.D2;
            b3Key = Key.D3;
            b4Key = Key.D4;
            b5Key = Key.D5;
            focusTrgt.Text = pressFocus;

            startKey = Key.C;
            stopKey = Key.V;
            pixelKey = Key.B;
            mouseSub();

        }
        void btnStartClk()
        {
            writeToTbIn("started ");
            if(!isRunning)
            {
                bTokenSource = new CancellationTokenSource();
                CancellationToken bToken = bTokenSource.Token;
                isRunning = true;
                isRunningBtn.Content = "on";
                string err = string.Empty;
                try
                {
                    //MessageBox.Show("s" + button1State.ToString());
                    switch (button1State)
                    {
                        case btnState.off:
                            break;
                        case btnState.press:                        
                                tlist.Add( Task.Run(() => pressKey(b1Key, timer1,bToken), bToken));                        
                            break;
                        case btnState.hold:
                                Task.Run(() => holdKey(b1Key, timer1, bToken), bToken);
                            break;
                        case btnState.pixelChange:
                                Task.Run(() => pixelPress(xVal1, yVal1, color1, b1Key, timer1, bToken), bToken);
                            break;
                    }
                    switch (button2State)
                    {
                        case btnState.off:
                            break;
                        case btnState.press:
                                Task.Run(() => pressKey(b2Key, timer2, bToken), bToken);
                            break;
                        case btnState.hold:
                                Task.Run(() => holdKey(b2Key, timer2, bToken), bToken);
                            break;
                        case btnState.pixelChange:
                                Task.Run(() => pixelPress(xVal2, yVal2, color2, b2Key, timer2, bToken), bToken);
                            break;
                    }
                    switch (button3State)
                    {
                        case btnState.off:
                            break;
                        case btnState.press:
                                Task.Run(() => pressKey(b3Key, timer3, bToken), bToken);
                            break;
                        case btnState.hold:
                                Task.Run(() => holdKey(b3Key, timer3, bToken), bToken);
                            break;
                        case btnState.pixelChange:
                                Task.Run(() => pixelPress(xVal3, yVal3, color3, b3Key, timer3, bToken), bToken);
                            break;
                    }
                    switch (button4State)
                    {
                        case btnState.off:
                            break;
                        case btnState.press:
                                Task.Run(() => pressKey(b4Key, timer4, bToken), bToken);
                            break;
                        case btnState.hold:
                                Task.Run(() => holdKey(b4Key, timer4, bToken), bToken);
                            break;
                        case btnState.pixelChange:
                                Task.Run(() => pixelPress(xVal4, yVal4, color4, b4Key, timer4, bToken), bToken);
                            break;
                    }
                    switch (button5State)
                    {
                        case btnState.off:
                            break;
                        case btnState.press:
                                Task.Run(() => pressKey(b5Key, timer5, bToken), bToken);
                            break;
                        case btnState.hold:
                                Task.Run(() => holdKey(b5Key, timer5, bToken), bToken);
                            break;
                        case btnState.pixelChange:
                                Task.Run(() => pixelPress(xVal5, yVal5, color5, b5Key, timer5, bToken), bToken);
                            break;
                    }
                    switch (lClkState)
                    {
                        case btnState.off:
                            break;
                        case btnState.press:
                                Task.Run(() => leftClk(timer1,bToken), bToken);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    err = ex.ToString();
                }
                finally
                {
                    if (err != string.Empty)
                    {
                        MessageBox.Show(err);
                    }
                }
                }
        }
        bool chkFocus()
        {
            if (windowFocus() == pressFocus)
            {
                return true;
            } else
            {
                btnStopClk();
                return false;
            }
        }
        void btnStopClk()
        {            
            isRunning = false;
            bTokenSource.Cancel();
            writeToTbIn("stopped");
            Dispatcher.Invoke(() => { isRunningBtn.Content = "off"; });
            
            //bTokenSource.Dispose();

            DirectInput.ReleaseKey(keyToShort(b1Key));
            DirectInput.ReleaseKey(keyToShort(b2Key));
            DirectInput.ReleaseKey(keyToShort(b3Key));
            DirectInput.ReleaseKey(keyToShort(b4Key));
            DirectInput.ReleaseKey(keyToShort(b5Key));
            DirectInput.LeftMouseClickRelease();
        }
        public void writeToTbIn(string inStr)
        {
            Dispatcher.Invoke(() => { tbIn.Text += inStr; });                
        }
        public void writeToTbOut(string inStr)
        {
            Dispatcher.Invoke(() => { tbOut.Text = inStr; });
            
        }
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(int hWnd, StringBuilder text, int count);
        private string windowFocus()
        {
            StringBuilder s = new StringBuilder(256);
            if(MainWindow.GetWindowText(MainWindow.GetForegroundWindow(),s,256)>0)
            {
                    return s.ToString();
            }
            return "";
        }
        void KListener_KeyDown(object sender, RawKeyEventArgs args)
        {
            //writeToTbIn(args.Key.ToString());
            if (args.Key==startKey)
            {
                btnStartClk();
            }
            if (args.Key == stopKey)
            {
                btnStopClk();
            }
            if (args.Key == pixelKey)
            {
                setPixel();
            }
            //Console.WriteLine(args.Key.ToString());
            //Console.WriteLine(args.ToString()); // Prints the text of pressed button, takes in account big and small letters. E.g. "Shift+a" => "A"
        }
        void setPixel()
        {
            color1 = GetColorAt(xVal1, yVal1);            
            color1Txt.Text = color1.ToString();
            color2 = GetColorAt(xVal2, yVal2);
            color2Txt.Text = color2.ToString();
            color3 = GetColorAt(xVal3, yVal3);
            color3Txt.Text = color3.ToString();
            color4 = GetColorAt(xVal4, yVal4);
            color4Txt.Text = color4.ToString();
            color5 = GetColorAt(xVal5, yVal5);
            color5Txt.Text = color5.ToString();
        }
        void leftClk(int inTimer, CancellationToken t)
        {
            while(isRunning && chkFocus())
            {
                t.ThrowIfCancellationRequested();
                DirectInput.LeftMouseClick();
                Thread.Sleep(inTimer);
            }
        }
        void pressKey(Key inKey,int inTimer,CancellationToken t)
        {
            //writeToTbOut(isRunning.ToString());

            while(isRunning && chkFocus())
            {                
                t.ThrowIfCancellationRequested();
                DirectInput.PressKey(keyToShort(inKey));
                Thread.Sleep(inTimer);
            }
        }
        void holdKey(Key inKey, int inTimer,CancellationToken t)
        {
            while(isRunning && chkFocus())
            {
                
                t.ThrowIfCancellationRequested();
                DirectInput.PressAndHoldKey(keyToShort(inKey));
                Thread.Sleep(inTimer);
                DirectInput.ReleaseKey(keyToShort(inKey));
            }
        }
        void pixelPress(int inX,int inY,Color inColor,Key inKey, int inTimer, CancellationToken t)
        {
            //MessageBox.Show("aaa");
            while(isRunning && chkFocus())
            {                
                t.ThrowIfCancellationRequested();
                if (GetColorAt(inX,inY) == inColor)
                {
                    DirectInput.PressKey(keyToShort(inKey));
                    Thread.Sleep(inTimer);
                }
                Thread.Sleep(50);
            }
        }
        Key stringToKey(string inStr)
        {
            switch(inStr)
            {
                case "1":
                    return Key.D1;
                case "2":
                    return Key.D2;
                case "3":
                    return Key.D3;
                case "4":
                    return Key.D4;
                case "5":
                    return Key.D5;
                case "-":
                    return Key.OemMinus;
                case "a":
                    return Key.A;
                case "s":
                    return Key.S;
                case "d":
                    return Key.D;
                case "f":
                    return Key.F;
                case "g":
                    return Key.G;
                case "q":
                    return Key.Q;
                case "w":
                    return Key.W;
                case "e":
                    return Key.E;
                case "r":
                    return Key.R;
                case "t":
                    return Key.T;
                case "z":
                    return Key.Z;
                case "x":
                    return Key.X;
                case "c":
                    return Key.C;
                case "v":
                    return Key.V;
                case "b":
                    return Key.B;
            }
            return Key.D1;
        }
        short keyToShort(Key inKey)
        {
            //MessageBox.Show("zzz" + inKey.ToString());
            switch(inKey)
            {
                case Key.Escape:
                    return 01;
                case Key.D1:
                    return 02;
                case Key.D2:
                    return 03;
                case Key.D3:
                    return 04;
                case Key.D4:
                    return 05;
                case Key.D5:
                    return 06;
                case Key.OemMinus:
                    return 12;
                case Key.Q:
                    return 16;
                case Key.W:
                    return 17;
                case Key.E:
                    return 18;
                case Key.R:
                    return 19;
                case Key.T:
                    return 20;
                case Key.A:
                    return 30;
                case Key.S:
                    return 31;
                case Key.D:
                    return 32;
                case Key.F:
                    return 33;
                case Key.G:
                    return 34;
                case Key.Z:
                    return 44;
                case Key.X:
                    return 45;
                case Key.C:
                    return 46;
                case Key.V:
                    return 47;
                case Key.B:
                    return 48;
            }
            return 0;
        }
        private void startKeyTxtChanged(object sender, TextChangedEventArgs e)
        {
            startKey = stringToKey(startKeyTxt.Text);
        }
        private void stopKeyTxtChanged(object sender, TextChangedEventArgs e)
        {
            stopKey = stringToKey(stopKeyTxt.Text);
        }
        private void pixelKeyTxtChanged(object sender, TextChangedEventArgs e)
        {
            pixelKey = stringToKey(pixelKeyTxt.Text);
        }

        private void mouseToWasdChk_Checked(object sender, RoutedEventArgs e)
        {
            if( mouseToWasdChk.IsChecked==true)
            {
                mouseTowasd = true;
            }
            else
            {
                mouseTowasd = false;
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            KListener.Dispose();
        }
        private void focusTrgt_TextChanged(object sender, TextChangedEventArgs e)
        {
            pressFocus = focusTrgt.Text;
        }
        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "presser File|*.xml|xml files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.FileName = pressFocus;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.Title = "presser Save Configuration to File";
            saveFileDialog.ValidateNames = true;
            if (!saveFileDialog.ShowDialog().Value)
                return;
            string errStr = string.Empty;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            try
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(saveFileDialog.FileName, settings))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteComment(pressFocus);
                    xmlWriter.WriteStartElement("presserSave");
                    xmlWriter.WriteElementString("leftClkType", lClkState.ToString());
                    xmlWriter.WriteElementString("leftClkTimer", lClkTimer.ToString());

                    xmlWriter.WriteElementString("b1State", button1State.ToString());
                    xmlWriter.WriteElementString("b1Key", key1Txt.Text); 
                    xmlWriter.WriteElementString("b1Timer", timer1.ToString());
                    xmlWriter.WriteElementString("x1", xVal1.ToString());
                    xmlWriter.WriteElementString("y1", yVal1.ToString());
                    xmlWriter.WriteElementString("color1", color1.ToString());

                    xmlWriter.WriteElementString("b2State", button2State.ToString());
                    xmlWriter.WriteElementString("b2Key", key2Txt.Text);
                    xmlWriter.WriteElementString("b2Timer", timer2.ToString());
                    xmlWriter.WriteElementString("x2", xVal2.ToString());
                    xmlWriter.WriteElementString("y2", yVal2.ToString());
                    xmlWriter.WriteElementString("color2", color2.ToString());

                    xmlWriter.WriteElementString("b3State", button3State.ToString());
                    xmlWriter.WriteElementString("b3Key", key3Txt.Text);
                    xmlWriter.WriteElementString("b3Timer", timer3.ToString());
                    xmlWriter.WriteElementString("x3", xVal3.ToString());
                    xmlWriter.WriteElementString("y3", yVal3.ToString());
                    xmlWriter.WriteElementString("color3", color3.ToString());

                    xmlWriter.WriteElementString("b4State", button4State.ToString());
                    xmlWriter.WriteElementString("b4Key", key4Txt.Text);
                    xmlWriter.WriteElementString("b4Timer", timer4.ToString());
                    xmlWriter.WriteElementString("x4", xVal4.ToString());
                    xmlWriter.WriteElementString("y4", yVal4.ToString());
                    xmlWriter.WriteElementString("color4", color4.ToString());

                    xmlWriter.WriteElementString("b5State", button5State.ToString());
                    xmlWriter.WriteElementString("b5Key", key5Txt.Text);
                    xmlWriter.WriteElementString("b5Timer", timer5.ToString());
                    xmlWriter.WriteElementString("x5", xVal5.ToString());
                    xmlWriter.WriteElementString("y5", yVal5.ToString());
                    xmlWriter.WriteElementString("color5", color5.ToString());

                    xmlWriter.WriteElementString("startKey", startKey.ToString().ToLower());
                    xmlWriter.WriteElementString("stopKey", stopKey.ToString().ToLower());
                    xmlWriter.WriteElementString("pixelKey", pixelKey.ToString().ToLower());
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                }

            }
            catch (Exception ex)
            {
                errStr = ex.ToString();
            }
            finally
            {
                if (errStr==string.Empty)
                {

                } else
                {
                    MessageBox.Show(errStr);
                }

            }
        }
        private void btnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "presser File|*.xml|xml files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Title = "presser Load Configuration from File";
            openFileDialog.RestoreDirectory = true;           
            string errStr = string.Empty;
            string currElement = "";
            try
            {
                if (!openFileDialog.ShowDialog().Value)
                    return;
                using (XmlReader reader = XmlReader.Create(openFileDialog.FileName))
                {
                    while(reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Comment:
                                {
                                    focusTrgt.Text = reader.Value;
                                    pressFocus = reader.Value;
                                }
                                break;
                            case XmlNodeType.Element:                               
                                currElement = reader.Name;
                                break;
                            case XmlNodeType.Text:
                                //MessageBox.Show("v" + reader.Value);
                                switch (currElement)
                                {
                                    case "leftClkType":
                                        {
                                            if (reader.Value == "off")
                                            {
                                                lClkState = btnState.off;
                                                lClk.SelectedIndex = 0;
                                            }
 
                                            else
                                            {
                                                lClkState = btnState.press;
                                                lClk.SelectedIndex = 1;
                                            }
                                                
                                        }
                                        break;
                                    case "leftClkTimer":
                                        {
                                            Int32.TryParse( reader.Value, out lClkTimer);
                                            lClktimerTxt.Text = reader.Value;
                                        }
                                        break;
                                    case "b1State":
                                        {
                                            switch(reader.Value)
                                            {
                                                case "off":
                                                    {
                                                        button1State = btnState.off;
                                                        btn1.SelectedIndex = 0;
                                                    }
                                                    break;
                                                case "press":
                                                    {
                                                        button1State = btnState.press;
                                                        btn1.SelectedIndex = 1;
                                                    }
                                                    break;
                                                case "hold":
                                                    {
                                                        button1State = btnState.hold;
                                                        btn1.SelectedIndex = 2;
                                                    }
                                                    break;
                                                case "pixelChange":
                                                    {
                                                        button1State = btnState.pixelChange;
                                                        btn1.SelectedIndex = 3;
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    case "b1Key":
                                        {
                                            key1Txt.Text = reader.Value;
                                            b1Key = stringToKey(reader.Value);
                                        }
                                        break;
                                    case "b1Timer":
                                        {
                                            timer1Txt.Text = reader.Value;
                                            Int32.TryParse(reader.Value, out timer1);
                                        }
                                        break;                                        
                                    case "x1":
                                        {
                                            xval1Txt.Text = reader.Value;
                                            Int32.TryParse(reader.Value, out xVal1);
                                        }
                                        break;
                                    case "y1":
                                        {
                                            yval1Txt.Text = reader.Value;
                                            Int32.TryParse(reader.Value, out yVal1);
                                        }
                                        break;
                                    case "color1":
                                        {
                                            color1Txt.Text = reader.Value;
                                            color1 = (Color)ColorConverter.ConvertFromString(reader.Value);                                            
                                        }
                                        break;                                        
                                    case "b2State":
                                        {
                                            switch (reader.Value)
                                            {
                                                case "off":
                                                    {
                                                        button2State = btnState.off;
                                                        btn2.SelectedIndex = 0;
                                                    }
                                                    break;
                                                case "press":
                                                    {
                                                        button2State = btnState.press;
                                                        btn2.SelectedIndex = 1;
                                                    }
                                                    break;
                                                case "hold":
                                                    {
                                                        button2State = btnState.hold;
                                                        btn2.SelectedIndex = 2;
                                                    }
                                                    break;
                                                case "pixelChange":
                                                    {
                                                        button2State = btnState.pixelChange;
                                                        btn2.SelectedIndex = 3;
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    case "b2Key":
                                        {
                                            key2Txt.Text = reader.Value;
                                            b2Key = stringToKey(reader.Value);
                                        }
                                        break;
                                    case "b2Timer":
                                        {
                                            timer2Txt.Text = reader.Value;
                                            Int32.TryParse(reader.Value, out timer2);
                                        }
                                        break;
                                    case "x2":
                                        {
                                            xval2Txt.Text = reader.Value;
                                            Int32.TryParse(reader.Value, out xVal2);
                                        }
                                        break;
                                    case "y2":
                                        {
                                            yval2Txt.Text = reader.Value;
                                            Int32.TryParse(reader.Value, out yVal2);
                                        }
                                        break;
                                    case "color2":
                                        {
                                            color2Txt.Text = reader.Value;
                                            color2 = (Color)ColorConverter.ConvertFromString(reader.Value);
                                        }
                                        break;
                                    case "b3State":
                                        {
                                            switch (reader.Value)
                                            {
                                                case "off":
                                                    {
                                                        button3State = btnState.off;
                                                        btn3.SelectedIndex = 0;
                                                    }
                                                    break;
                                                case "press":
                                                    {
                                                        button3State = btnState.press;
                                                        btn3.SelectedIndex = 1;
                                                    }
                                                    break;
                                                case "hold":
                                                    {
                                                        button3State = btnState.hold;
                                                        btn3.SelectedIndex = 2;
                                                    }
                                                    break;
                                                case "pixelChange":
                                                    {
                                                        button3State = btnState.pixelChange;
                                                        btn3.SelectedIndex = 3;
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    case "b3Key":
                                        {
                                            key3Txt.Text = reader.Value;
                                            b3Key = stringToKey(reader.Value);
                                        }
                                        break;
                                    case "b3Timer":
                                        {
                                            timer3Txt.Text = reader.Value;
                                            Int32.TryParse(reader.Value, out timer3);
                                        }
                                        break;
                                    case "x3":
                                        {
                                            xval3Txt.Text = reader.Value;
                                            Int32.TryParse(reader.Value, out xVal3);
                                        }
                                        break;
                                    case "y3":
                                        {
                                            yval3Txt.Text = reader.Value;
                                            Int32.TryParse(reader.Value, out yVal3);
                                        }
                                        break;
                                    case "color3":
                                        {
                                            color3Txt.Text = reader.Value;
                                            color3 = (Color)ColorConverter.ConvertFromString(reader.Value);
                                        }
                                        break;
                                    case "b4State":
                                        {
                                            switch (reader.Value)
                                            {
                                                case "off":
                                                    {
                                                        button4State = btnState.off;
                                                        btn4.SelectedIndex = 0;
                                                    }
                                                    break;
                                                case "press":
                                                    {
                                                        button4State = btnState.press;
                                                        btn4.SelectedIndex = 1;
                                                    }
                                                    break;
                                                case "hold":
                                                    {
                                                        button4State = btnState.hold;
                                                        btn4.SelectedIndex = 2;
                                                    }
                                                    break;
                                                case "pixelChange":
                                                    {
                                                        button4State = btnState.pixelChange;
                                                        btn4.SelectedIndex = 3;
                                                    }
                                                    break;
                                            }
                                        }                                       
                                        break;
                                    case "b4Key":
                                        {
                                            key4Txt.Text = reader.Value;
                                            b4Key = stringToKey(reader.Value);
                                        }
                                        break;
                                    case "b4Timer":
                                        {
                                            timer4Txt.Text = reader.Value;
                                            Int32.TryParse(reader.Value, out timer4);
                                        }
                                        break;
                                    case "x4":
                                        {
                                            xval4Txt.Text = reader.Value;
                                            Int32.TryParse(reader.Value, out xVal4);
                                        }
                                        break;
                                    case "y4":
                                        {
                                            yval4Txt.Text = reader.Value;
                                            Int32.TryParse(reader.Value, out yVal4);
                                        }
                                        break;
                                    case "color4":
                                        {
                                            color4Txt.Text = reader.Value;
                                            color4 = (Color)ColorConverter.ConvertFromString(reader.Value);
                                        }
                                        break;
                                    case "b5State":
                                        {
                                            switch (reader.Value)
                                            {
                                                case "off":
                                                    {
                                                        button5State = btnState.off;
                                                        btn5.SelectedIndex = 0;
                                                    }
                                                    break;
                                                case "press":
                                                    {
                                                        button5State = btnState.press;
                                                        btn5.SelectedIndex = 1;
                                                    }
                                                    break;
                                                case "hold":
                                                    {
                                                        button5State = btnState.hold;
                                                        btn5.SelectedIndex = 2;
                                                    }
                                                    break;
                                                case "pixelChange":
                                                    {
                                                        button5State = btnState.pixelChange;
                                                        btn5.SelectedIndex = 3;
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    case "b5Key":
                                        {
                                            key5Txt.Text = reader.Value;
                                            b5Key = stringToKey(reader.Value);
                                        }
                                        break;
                                    case "b5Timer":
                                        {
                                            timer5Txt.Text = reader.Value;
                                            Int32.TryParse(reader.Value, out timer5);
                                        }
                                        break;
                                    case "x5":
                                        {
                                            xval5Txt.Text = reader.Value;
                                            Int32.TryParse(reader.Value, out xVal5);
                                        }
                                        break;
                                    case "y5":
                                        {
                                            yval5Txt.Text = reader.Value;
                                            Int32.TryParse(reader.Value, out yVal5);
                                        }
                                        break;
                                    case "color5":
                                        {
                                            color5Txt.Text = reader.Value;
                                            color5 = (Color)ColorConverter.ConvertFromString(reader.Value);
                                        }
                                        break;
                                    case "startKey":
                                        {
                                            startKeyTxt.Text = reader.Value;
                                            startKey = stringToKey(reader.Value);
                                        }
                                        break;
                                    case "stopKey":
                                        {
                                            stopKeyTxt.Text = reader.Value;
                                            stopKey = stringToKey(reader.Value);
                                        }
                                        break;
                                    case "pixelKey":
                                        {
                                            pixelKeyTxt.Text = reader.Value;
                                            pixelKey = stringToKey(reader.Value);
                                        }
                                        break;
                                }
                                break;

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                errStr = ex.ToString();
            }
            finally
            {
                if (errStr == string.Empty)
                {

                }
                else
                {
                    MessageBox.Show(errStr);
                }

            }

        }
        private void lClktimerChanged(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(lClktimerTxt.Text, out lClkTimer);
        }
        private void lClkChanged(object sender, SelectionChangedEventArgs e)
        {            
            switch (lClk.SelectedIndex)
            {
                case 0:
                    lClkState = btnState.off;
                    break;
                case 1:
                    lClkState = btnState.press;
                    break;
                case 2:
                    lClkState = btnState.hold;
                    break;
            }
        }
        private void xVal11Changed(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(xval1Txt.Text, out xVal1);
        }
        private void yVal11Changed(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(yval1Txt.Text, out yVal1);
        }
        private void cmb1Changed(object sender, SelectionChangedEventArgs e)
        {
            switch(btn1.SelectedIndex)
            {
                case 0:
                    button1State = btnState.off;
                    break;
                case 1:
                    button1State = btnState.press;
                    break;
                case 2:
                    button1State = btnState.hold;
                    break;
                case 3:
                    button1State = btnState.pixelChange;
                    break;
            }
        }
        private void timer1Changed(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(timer1Txt.Text, out timer1);
        }
        private void key1Changed(object sender, TextChangedEventArgs e)
        {
            b1Key = stringToKey(key1Txt.Text);
        }
        private void xVal2Changed(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(xval2Txt.Text, out xVal2);
        }
        private void yVal2Changed(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(yval2Txt.Text, out yVal2);
        }
        private void cmb2Changed(object sender, SelectionChangedEventArgs e)
        {
            switch (btn2.SelectedIndex)
            {
                case 0:
                    button2State = btnState.off;
                    break;
                case 1:
                    button2State = btnState.press;
                    break;
                case 2:
                    button2State = btnState.hold;
                    break;
                case 3:
                    button2State = btnState.pixelChange;
                    break;
            }
        }
        private void timer2Changed(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(timer2Txt.Text, out timer2);
        }
        private void key2Changed(object sender, TextChangedEventArgs e)
        {
            b2Key = stringToKey(key2Txt.Text);
        }
        private void xVal3Changed(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(xval3Txt.Text, out xVal3);
        }
        private void yVal3Changed(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(yval3Txt.Text, out yVal3);
        }
        private void cmb3Changed(object sender, SelectionChangedEventArgs e)
        {
            switch (btn3.SelectedIndex)
            {
                case 0:
                    button3State = btnState.off;
                    break;
                case 1:
                    button3State = btnState.press;
                    break;
                case 2:
                    button3State = btnState.hold;
                    break;
                case 3:
                    button3State = btnState.pixelChange;
                    break;
            }
        }
        private void timer3Changed(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(timer3Txt.Text, out timer3);
        }
        private void key3Changed(object sender, TextChangedEventArgs e)
        {
            b3Key = stringToKey(key3Txt.Text);
        }
        private void xVal4Changed(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(xval4Txt.Text, out xVal4);
        }
        private void yVal4Changed(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(yval4Txt.Text, out yVal4);
        }
        private void cmb4Changed(object sender, SelectionChangedEventArgs e)
        {
            switch (btn4.SelectedIndex)
            {
                case 0:
                    button4State = btnState.off;
                    break;
                case 1:
                    button4State = btnState.press;
                    break;
                case 2:
                    button4State = btnState.hold;
                    break;
                case 3:
                    button4State = btnState.pixelChange;
                    break;
            }
        }
        private void timer4Changed(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(timer4Txt.Text, out timer4);
        }
        private void key4Changed(object sender, TextChangedEventArgs e)
        {
            b4Key = stringToKey(key4Txt.Text);
        }
        private void xVal5Changed(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(xval5Txt.Text, out xVal5);
        }
        private void yVal5Changed(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(yval5Txt.Text, out yVal5);
        }
        private void cmb5Changed(object sender, SelectionChangedEventArgs e)
        {
            switch (btn5.SelectedIndex)
            {
                case 0:
                    button5State = btnState.off;
                    break;
                case 1:
                    button5State = btnState.press;
                    break;
                case 2:
                    button5State = btnState.hold;
                    break;
            }
        }
        private void key5Changed(object sender, TextChangedEventArgs e)
        {
            b5Key = stringToKey(key5Txt.Text);
        }
        private void timer5Changed(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(timer5Txt.Text, out timer5);
        }
    }
}
