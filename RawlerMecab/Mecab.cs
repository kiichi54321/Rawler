using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawler.Tool;

namespace RawlerMecab
{
    public class Mecab:RawlerMultiBase
    {
        /// <summary>
        /// ObjectのName。表示用
        /// </summary>
        public override string ObjectName
        {
            get { return this.GetType().Name; }
        }

        MeCab.Tagger tagger = null;
        public override void Dispose()
        {

            destroy();
            base.Dispose();
        }

        public string Args { get; set; }

        public Mecab()
        {

        }




        string a = "表層形\t品詞,品詞細分類1,品詞細分類2,品詞細分類3,活用形,活用型,原形,読み,発音";

        public string UseHinshi { get; set; }

        public IEnumerable<string> GetResult(string text)
        {
            string result = string.Empty;
            try
            {
                result = AnalyzeMultiLine(text);
            }
            catch (Exception e)
            {
                ReportManage.ErrReport(this, "形態素解析に失敗しました。" + e.Message + "\t" + GetText());
                tagger.Dispose();
                tagger = null;
            }
            System.IO.StringReader sr = new System.IO.StringReader(result);
            List<string> list = new List<string>();
            string[] hinshiArray = null;
            if (UseHinshi != null)
            {
                hinshiArray = UseHinshi.Split(',');
            }
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();

                bool flag = false;
                if (line.Length > 0 && line != "EOS")
                {
                    if (UseHinshi != null)
                    {
                        string hinsi = MecabAnalyzeLine(line, MecabViewType.品詞);
                        if (hinsi != null)
                        {
                            foreach (var item in hinshiArray)
                            {
                                if (hinsi.Contains(item))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        string t = MecabAnalyzeLine(line, ViewType);
                        if (t != null)
                        {
                            list.Add(t);
                        }
                        else
                        {
                            ReportManage.ErrReport(this, "MecabLineの解析に失敗しました" + line);
                        }
                    }

                }
            }
            return list;
        }


        public override void Run(bool runChildren)
        {

            string result = string.Empty;
            try
            {
                result = AnalyzeMultiLine(GetText());
            }
            catch(Exception e)
            {
                ReportManage.ErrReport(this, "形態素解析に失敗しました。" + e.Message + "\t" + GetText());
                tagger.Dispose();
                tagger = null;
            }
            System.IO.StringReader sr = new System.IO.StringReader(result);
            List<string> list = new List<string>();
            string[] hinshiArray = null;
            if (UseHinshi != null)
            {
                 hinshiArray = UseHinshi.Split(',');
            }
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
 
                bool flag = false;
                if (line.Length > 0 && line != "EOS")
                {
                    if (UseHinshi != null)
                    {
                        string hinsi = MecabAnalyzeLine(line, MecabViewType.品詞);
                        if (hinsi != null)
                        {
                            foreach (var item in hinshiArray)
                            {
                                if (hinsi.IndexOf(item) == 0)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        string t = MecabAnalyzeLine(line, ViewType);
                        if (t != null)
                        {
                            list.Add(t);
                        }
                        else
                        {
                            ReportManage.ErrReport(this,"MecabLineの解析に失敗しました"+line);
                        }
                    }

                }
            }

            base.RunChildrenForArray(runChildren,list);
        }


        public string _Analyze(string txt)
        {
            string r = string.Empty;
            try
            {
                while (tagger == null)
                {
                    _Init();
                    if (tagger == null)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }
                r =  tagger.parse(txt);
            }
            catch (Exception e)
            {
                ReportManage.ErrReport(this, "形態素解析に失敗しました:" + txt);
            }
            finally
            {
                tagger.Dispose();
                tagger = null;
            }
            return r;
        }

        private void _Init()
        {
            if (tagger != null)
            {
                tagger.Dispose();
                tagger = null;
            }

            try
            {
                if (Args != null)
                {
                    tagger = new MeCab.Tagger(Args);
                }
                else
                {
                    tagger = new MeCab.Tagger();
                }

                if (tagger == null)
                {
                    ReportManage.ErrReport(this, "Mecabの起動に失敗しました。インストールされているか確かめてください。");
                    throw new Exception("Mecabの起動に失敗しました。インストールされているか確かめてください。");
                }
            }
            catch
            {
                ReportManage.ErrReport(this, "Mecabの起動に失敗しました。インストールされているか確かめてください。");
                throw new Exception("Mecabの起動に失敗しました。インストールされているか確かめてください。");
            }
        }


        private void Init()
        {
            if (mecabDotNet != null)
            {
                mecabDotNet.Dispose();
            }

            try
            {
                if (Args != null)
                {
                    mecabDotNet = new MecabDotNet.Mecab(Args);
                }
                else
                {
                    mecabDotNet = new MecabDotNet.Mecab("");
                }
            }
            catch
            {
                ReportManage.ErrReport(this, "Mecabの起動に失敗しました。インストールされているか確かめてください。");
                throw new Exception("Mecabの起動に失敗しました。インストールされているか確かめてください。");
            }
        }
        

        MecabDotNet.Mecab mecabDotNet = null;
        public string Analyze(string txt)
        {
            string r = null;
            try
            {
                while (r == null)
                {
                    if (mecabDotNet == null)
                    {
                        Init();
                    }
                    r = mecabDotNet.mecab_sparse_tostr(txt);
                    if (r == null)
                    {
                        System.Threading.Thread.Sleep(100);
                        Init(); 
                    }
                }
               
            }
            catch (Exception e)
            {
                ReportManage.ErrReport(this, "形態素解析に失敗しました:"+e.Message +"\ttext:"+ txt);
            }

            return r;
        }

        public string AnalyzeMultiLine(string txt)
        {
            StringBuilder strbuilder = new StringBuilder();

            foreach (var item in txt.Split(" 。.!?！？".ToCharArray(),StringSplitOptions.RemoveEmptyEntries))
            {
                if (item.Length > 0)
                {
                    strbuilder.Append(Analyze(item));
                }
            }

            return strbuilder.ToString();
        }

        private MecabViewType viewType = MecabViewType.表層形;

        public MecabViewType ViewType
        {
            get { return viewType; }
            set { viewType = value; }
        }


        /// <summary>
        /// Dispose()と同じ。
        /// </summary>
        public void destroy()
        {
            if (mecabDotNet != null)
            {
                mecabDotNet.Dispose();
            }
            if (tagger != null)
            {
                tagger.Dispose();
            }
        }

        public static string MecabAnalyzeLine(string line, MecabViewType type)
        {
            if (type == MecabViewType.すべて)
            {
                return line;
            }

            var d = line.Split('\t');
            if (d.Length > 1)
            {
                if (type == MecabViewType.表層形)
                {
                    return d[0];
                }
                var d2 = d[1].Split(',');
                if (d2.Length > 4)
                {
                    if (type == MecabViewType.品詞)
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < 4; i++)
                        {
                            if (d2[i] != "*")
                            {
                                sb.Append(d2[i]);
                                sb.Append("-");
                            }
                        }
                        sb.Length = sb.Length - 1;
                        return sb.ToString();
                    }
                }
                try
                {
                    if (type == MecabViewType.活用形)
                    {
                        return d2[4];
                    }
                    if (type == MecabViewType.活用型)
                    {
                        return d2[5];
                    }
                    if (type == MecabViewType.原形)
                    {
                        return d2[6];
                    }
                    if (type == MecabViewType.読み)
                    {
                        return d2[7];
                    }
                    if (type == MecabViewType.発音)
                    {
                        return d2[8];
                    }
                }
                catch
                {
                    ReportManage.ErrReport(new Mecab(),"MecabLineの解析に失敗");
                }
                    return null;
                
            }

            return null;


        }
    }

    

    public enum MecabViewType
    {
        すべて,表層形,品詞,活用形,活用型,原形,読み,発音
    }

    public class MecabAnalyeLine:RawlerBase
    {
        private MecabViewType viewType = MecabViewType.表層形;

        public MecabViewType ViewType
        {
            get { return viewType; }
            set { viewType = value; }
        }

        public override void Run(bool runChildren)
        {
            SetText(Mecab.MecabAnalyzeLine(GetText(), viewType));

            base.Run(runChildren);
        }
    }
}
