using System;
using System.Collections.Generic;
using System.Text;

namespace RawlerLib.Thread
{
    /// <summary>
    /// 解析など用にバックグランド処理をするための基底クラスです。
    /// </summary>
    [Serializable]
    public class BackgroundWorker:IDisposable
    {
        [NonSerialized]
        protected System.ComponentModel.BackgroundWorker backgroundWorker;
        public event System.ComponentModel.ProgressChangedEventHandler ProgressChanged;
        public event System.ComponentModel.RunWorkerCompletedEventHandler RunWorkerCompleted;
        public event System.ComponentModel.DoWorkEventHandler DoWork;

        public BackgroundWorker()
        {
            Init();
            
        }

        /// <summary>
        /// 基本設定
        /// </summary>
        protected void Init()
        {
            backgroundWorker = new System.ComponentModel.BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(backgroundWorker_DoWork);
        }

        void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (DoWork != null)
            {
                DoWork(sender, e);
            }
        }

        public void RunWorkerAsync(object obj)
        {
            if (backgroundWorker.IsBusy == false)
            {
                backgroundWorker.RunWorkerAsync(obj);
            }
        }

        


        public void RunWorkerAsync()
        {
            if (backgroundWorker.IsBusy == false)
            {
                backgroundWorker.RunWorkerAsync();
            }
        }

        public bool IsBusy
        {
            get { return backgroundWorker.IsBusy; }
        }

        protected bool CancellationPending
        {
            get { return backgroundWorker.CancellationPending; }
        }

        public void CancelAsync()
        {
            backgroundWorker.CancelAsync();
        }

        public void ReportProgress(int percent)
        {
            if (backgroundWorker.IsBusy)
            {
                backgroundWorker.ReportProgress(percent);
            }
        }

        public void ReportProgress(int persent, object userState)
        {
            backgroundWorker.ReportProgress(persent, userState);
        }


        int tmpProgressPercentage = -1;
        object tmpUserState = null;

        protected void backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
            {
                if ((e.ProgressPercentage != tmpProgressPercentage) || e.UserState != tmpUserState)
                {
                    ProgressChanged(sender, e);
                    tmpUserState = e.UserState;
                    tmpProgressPercentage = e.ProgressPercentage;
                }
            }
        }

        protected void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (RunWorkerCompleted != null)
            {
                RunWorkerCompleted(sender, e);
            }
        }

        public delegate void Work();
        /// <summary>
        /// サブスレッドで指定のメソッドを実行し、終了メソッドを発動させます。（大失敗使うな）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="endTarget"></param>
        public void SubBackgroundWorkerRun(Work target,Work endTarget,BackgroundWorker bgw)
        {
//            BackgroundWorker bgw = CreateSubBackgroundWorker();
            bgw.DoWork += delegate(object sender,System.ComponentModel.DoWorkEventArgs e)
            {
                target.Invoke();
            };
            bgw.RunWorkerCompleted += delegate(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
            {
                if (endTarget != null)
                {
                    endTarget.Invoke();
                }
            };
            bgw.RunWorkerAsync();
        }


        /// <summary>
        /// ProgressChangedだけを共有したBackgroundWorkerを返します。
        /// </summary>
        /// <returns></returns>
        public BackgroundWorker CreateSubBackgroundWorker()
        {
            
            BackgroundWorker bgw = new BackgroundWorker();
            bgw.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            return bgw;
        }

        #region IDisposable メンバ

        public void Dispose()
        {
            backgroundWorker.Dispose();
        }

        #endregion
    }

    class test : System.ComponentModel.BackgroundWorker
    {
        void t()
        {
            
        }
    }
}
