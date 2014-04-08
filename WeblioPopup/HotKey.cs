using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WeblioPopup
{
    //modifiers 
    [Flags()]
    public enum MOD : uint
    {
        /// <summary>どちらかの Alt キー</summary>
        MOD_ALT = 0x1,
        /// <summary>どちらかの Ctrl キー</summary>
        MOD_CONTROL = 0x2,
        /// <summary>どちらかの Shift キー</summary>
        MOD_SHIFT = 0x4,
        /// <summary>どちらかの Win キー</summary>
        MOD_WIN = 0x8,
        /// <summary>リピート無効</summary>
        MOD_NOREPEAT = 0x4000
    }
}


namespace WeblioPopup
{
    public class HotKey : IDisposable
    {
        public HotKey()
        {
        }

        //コンストラクタ
        public HotKey(IntPtr handle, MOD Modifiers, Keys Key)
        {
            this.Handle = handle;
            this.Modifiers = Modifiers;
            this.Key = Key;
        }

        public const int WM_HOTKEY = 0x312; //hot_keyのwindow message

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int RegisterHotKey(IntPtr hWnd, int id, MOD fsModifiers, uint vk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int UnregisterHotKey(IntPtr hWnd, int id);

        private IntPtr _Handle;
        public IntPtr Handle
        {
            get { return _Handle; }
            set { _Handle = value; }
        }

        private MOD _Modifiers;
        public MOD Modifiers
        {
            get { return _Modifiers; }
            set { _Modifiers = value; }
        }

        private Keys _Key;
        public Keys Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        private bool _IsRegister;
        public bool IsRegister
        {
            get { return _IsRegister; }
        }

        private IntPtr lParam;
        public IntPtr GetLParam()
        {
            return this.lParam;
        }

        private int hotKeyId;

        //ホットキーを登録する
        public int Register()
        {
            if (this.IsRegister == true)
            {
                this.Unregister();
            }

            this.hotKeyId = Convert.ToInt32(((int)(this.Modifiers & (~MOD.MOD_NOREPEAT)) * 0x100) | (int)this.Key);
            this.lParam = new IntPtr((int)(this.Modifiers & (~MOD.MOD_NOREPEAT)) | ((int)this.Key * 0x10000));

            int ret = RegisterHotKey(this.Handle, this.hotKeyId, this.Modifiers, (uint)this.Key);
            if (ret == 0)
            {
                // 登録失敗
                return ret;
            }

            this._IsRegister = true;
            return ret;
        }

        //ホットキーの登録を解除
        public int Unregister()
        {
            int ret = UnregisterHotKey(this.Handle, this.hotKeyId);
            if (ret == 0)
            {
                // 解除失敗
                return ret;
            }

            this._IsRegister = false;
            return ret;
        }


        #region " IDisposable 実装"

        private bool disposedValue = false;
        // 重複する呼び出しを検出するには

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: 他の状態を解放します (マネージ オブジェクト)。
                    try
                    {
                        if (this.IsRegister == true)
                        {
                            this.Unregister();
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                }
            }
            // TODO: ユーザー独自の状態を解放します (アンマネージ オブジェクト)。
            // TODO: 大きなフィールドを null に設定します。
            this.disposedValue = true;
        }

        #region " IDisposable Support "
        // このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #endregion
    }
}