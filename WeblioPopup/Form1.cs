using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WeblioPopup
{
    public partial class Form1 : Form
    {
        private HotKey hotKeyA;
        private const String DefaultURL = "http://ejje.weblio.jp/";
        private const String SearchURL = "http://ejje.weblio.jp/content/";
        private bool IsProgress = true;


        //コンストラクタ
        public Form1()
        {
            InitializeComponent();
           
            word_input.Focus();
            //ctrl + F12 をホットキーに登録
            this.hotKeyA = new HotKey(this.Handle, MOD.MOD_CONTROL, Keys.F12);
            if (this.hotKeyA.Register() == 0) {
                MessageBox.Show("このホットキーはすでに登録されています。\nWeblioPopupはすでに起動済みの可能性があります。");
                notifyIcon1.Visible = false;
                System.Environment.Exit(0);
            }
        }

        //ホットキーが押された時の動作
        protected override void WndProc(ref System.Windows.Forms.Message m) {            
            if (m.Msg == HotKey.WM_HOTKEY) {
                if (m.LParam == this.hotKeyA.GetLParam()) {
                    if (Form.ActiveForm != this) {
                        this.Show();
                        this.Activate();
                        word_input.Focus();
                        word_input.SelectAll();
                    } else {
                        this.Hide();
                    }
                }
            }

            base.WndProc(ref m); //なんか絶対必要なものらしい。
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //textboxに入力されるたびに更新
        private void word_input_TextChanged(object sender, EventArgs e)
        {
            webBrowser1.Stop();
            progressBar1.Value = 0;
            progressBar1.Visible = true;
            IsProgress = true;
            webBrowser1.Navigate(SearchURL + word_input.Text);
            word_input.Focus();

        }

        //web Browserのロードが終了したらtextboxにフォーカスを移す
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            word_input.Focus();
            progressBar1.Visible = false;
            webBrowser1.Document.Window.ScrollTo(0, 300);
        }

        //
        private void webBrowser1_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            if (!IsProgress)
            {
                return;
            }
            if (e.CurrentProgress == -1 || e.MaximumProgress == 0)
            {
                IsProgress = false;
                progressBar1.Value = 100;
            }
            else
            {
                progressBar1.Value = Math.Min((int)(100.0 * e.CurrentProgress / e.MaximumProgress), 100);
            }

        }

        //閉じるボタンが押されたとき
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        //最小化ボタンが押されたとき
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {               
                this.Hide();
            }
        }

        //右クリックでできるメニュー
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            System.Environment.Exit(0);
        }
    }
}
