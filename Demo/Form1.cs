using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolKit;


namespace Demo
{

    public partial class frmMain : Form
    {
        private ChromeExtension _chromeExtension;

        public frmMain()
        {
            InitializeComponent();
        }


        private void frmMain_Load(object sender, EventArgs e)
        {
            _chromeExtension = new ChromeExtension(new Listener(lstReceived), typeof(MyEntity));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            MyEntity myEntity = new MyEntity();
            myEntity.text = txtMessage.Text;
            txtMessage.Clear();

            lstReceived.Items.Add("You: " + myEntity.text);

            _chromeExtension.Send(myEntity);
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            _chromeExtension.Stop();
            _chromeExtension = null;
        }
    }

    [DataContract]
    public class MyEntity
    {
        [DataMember(Name = "text")]
        public String text { get; set; }
    }

    class Listener : ChromeExtension.IListener
    {
        private ListBox _lst;

        public Listener(ListBox lst)
        {
            _lst = lst;
        }

        delegate void SetTextCallback(string text);

        private void SetText(string text)
        {
            try
            {
                if (_lst.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetText);
                    _lst.Invoke(d, new object[] { text });
                }
                else
                {
                    _lst.Items.Add(text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
        }

        public void Receive(string message)
        {
            SetText(message);
        }

        public void Receive(Object payload)
        {
            SetText("Sender: " + ((MyEntity)payload).text);
        }

        public void Error(Exception ex)
        {
            Console.WriteLine(ex.Message);
            MessageBox.Show(ex.Message);
        }
    }





}
