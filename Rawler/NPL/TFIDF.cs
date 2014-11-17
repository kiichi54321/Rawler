using Rawler.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RawlerLib.MyExtend;
using System.Runtime.Serialization;

namespace Rawler.NPL
{
 /// <summary>
    /// TFIDFを行う
 /// </summary>
    [DataContract] 
    public class TFIDFAnalyze
    {
        Dictionary<string, DocumentData> docDic = new Dictionary<string, DocumentData>();
        [DataMember]
        public Dictionary<string, DocumentData> DocDic
        {
            get { return docDic; }
            set { docDic = value; }
        }
        public void DicClear()
        {
            docDic.Clear();
            idfDic = null;
            documentId = 0;
        }

        public void AddDocDic(Dictionary<string, DocumentData> dic)
        {
            foreach (var item in dic)
            {
                var doc = docDic.GetValueOrAdd(item.Key, new DocumentData(){ DocName = item.Key});
                foreach (var item2 in item.Value.WordDic)
                {
                    doc.Add(item2.Key,item2.Value.Count);
                }
            }
        }
        
        public void Add(string docName,string word)
        {
            docDic.GetValueOrAdd(docName, new DocumentData() { DocName = docName }).Add(word);
            idfDic = null;
        }

        public void Add(string docName, string word,int count)
        {
            docDic.GetValueOrAdd(docName, new DocumentData() { DocName = docName }).Add(word,count);
            idfDic = null;
        }


        int documentId = 0;
        public void AddDocument(IEnumerable<string> list)
        {
            foreach (var item in list)
            {
                Add(documentId.ToString(), item);
            }
            documentId++;
            idfDic = null;
        }

        Dictionary<string, CountData> idfDic;

        public Dictionary<string, CountData> IDFDic
        {
            get { return idfDic; }
           
        }

        public Dictionary<string, CountData> CreateIDFdic()
        {
            idfDic = docDic.Values.AsParallel().SelectMany(n => n.GetWordList()).GroupBy(n => n).Select(n => new CountData() { Word = n.Key, Count = n.Count(), Value = Math.Log(docDic.Count / (double)(n.Count() + 1)) }).ToDictionary(n => n.Word);
            return idfDic;
        }

        public static Dictionary<string,CountData> CreateIDFdic(CountDic dic,int docNum)
        {
            return dic.Dic.Select(n => new CountData() { Word = n.Key, Count = n.Value, Value = Math.Log(docNum / (double)(n.Value + 1)) }).ToDictionary(n => n.Word);
        }



        public IEnumerable<TfidfResult> GetResult(string docName)
        {
            if (idfDic == null) CreateIDFdic();
            var all = docDic.GetValueOrDefault(docName).WordDic.Values.Sum(n=>n.Count);
            return docDic.GetValueOrDefault(docName).WordDic.Values.AsParallel().Select(n => new TfidfResult() { Word = n.Word, TF = n.Count / (double)all, IDF = idfDic.GetValueOrDefault(n.Word).Value }).OrderByDescending(n => n.TFIDF);
        }

        public IEnumerable<TfidfResult> GetResult(IEnumerable<string> list)
        {
            if (idfDic == null) CreateIDFdic();
            var all = list.Count();
            return list.AsParallel().GroupBy(n => n).Select(n => new TfidfResult() { Word = n.Key, TF = n.Count() / (double)all, IDF = idfDic.GetValueOrDefault(n.Key, new CountData() { Value = Math.Log(docDic.Count) }).Value }).OrderByDescending(n => n.TFIDF);
        }

        public IEnumerable<TfidfResult> GetResult(IEnumerable<KeyValuePair<string,int>> list)
        {
            if (idfDic == null) CreateIDFdic();
            var all = list.Sum(n => n.Value);
            return list.AsParallel().Select(n => new TfidfResult() { Word = n.Key, TF = n.Value / (double)all, IDF = idfDic.GetValueOrDefault(n.Key, new CountData() { Value = Math.Log(docDic.Count) }).Value }).OrderByDescending(n => n.TFIDF);

        }

        public IEnumerable<TfidfResult> GetResult()
        {
            if (idfDic == null) CreateIDFdic();
            var all = docDic.SelectMany(n=>n.Value.WordDic.Values).Sum(n=>n.Count);
            return docDic.AsParallel().SelectMany(n => n.Value.WordDic.Values).GroupBy(n => n.Word).Select(n => new TfidfResult() { Word = n.Key, TF = n.Sum(m => m.Count) / (double)all, IDF = idfDic.GetValueOrDefault(n.Key).Value }).OrderByDescending(n => n.TFIDF);               
        }

        public static TFIDFAnalyze Marge(IEnumerable<TFIDFAnalyze> list)
        {
            TFIDFAnalyze analyze = new TFIDFAnalyze();
            foreach (var item in list)
            {
                analyze.AddDocDic(item.DocDic);
            }
            return analyze;
        }

        public static TFIDFAnalyze Marge(IEnumerable<string> fileNames)
        {
            TFIDFAnalyze analyze = new TFIDFAnalyze();
            foreach (var fileName in fileNames)
            {
                foreach (var item in System.IO.File.ReadLines(fileName))
                {
                    try
                    {
                        var d = item.Split('\t');
                        int c = int.Parse(d.Last());
                        analyze.Add(d[0], d[1], c);
                    }
                    catch { }
                }
            }

            return analyze;
        }

        public void SaveIDF(string fileName)
        {
            var idf = CreateIDFdic();
            using (var file = System.IO.File.Create(fileName))
            {
                DataContractSerializer dc = new DataContractSerializer(idf.GetType());
                dc.WriteObject(file, idf);
            }
        }

        public void LoadIDF(string fileName)
        {
            using (var file = System.IO.File.OpenRead(fileName))
            {
                DataContractSerializer dc = new DataContractSerializer(idfDic.GetType());
                idfDic = (Dictionary<string,CountData>)dc.ReadObject(file);
            }
        }

        public void SaveText(string fileName)
        {
            using (var file = System.IO.File.CreateText(fileName))
            {
                foreach (var item in docDic)
                {
                    foreach (var item2 in item.Value.WordDic)
                    {
                        var line = item.Key + "\t" + item2.Key + "\t" + item2.Value.Count;
                        try
                        {
                            file.Encoding.GetBytes(line);
                            file.WriteLine(line);
                        }
                        catch { }
                    }
                }
            }
        }

        public static TFIDFAnalyze LoadText(string fileName)
        {
            TFIDFAnalyze tfidf = new TFIDFAnalyze();
            foreach (var item in System.IO.File.ReadLines(fileName))
            {
                try
                {
                    var d = item.Split('\t');
                    int c = int.Parse(d.Last());
                    tfidf.Add(d[0], d[1], c);
                }
                catch { }
            }
            return tfidf;
        }

        public void Save(string fileName)
        {
            using (var file = System.IO.File.Create(fileName))
            {
                DataContractSerializer dc = new DataContractSerializer(this.GetType());
                dc.WriteObject(file, this);                
            }
        }

        public static TFIDFAnalyze Load(string fileName)
        {
            using(var file = System.IO.File.OpenRead(fileName))
            {
                DataContractSerializer dc = new DataContractSerializer(typeof(TFIDFAnalyze));
                return  (TFIDFAnalyze)dc.ReadObject(file);
            }
        }


        public class TfidfResult
        {
            public string Word { get; set; }
            public double TF { get; set; }
            public double IDF { get; set; }
            public double TFIDF { get{return TF*IDF;}}
        }
        [DataContract]
        public class DocumentData
        {
            Dictionary<string, CountData> wordDic = new Dictionary<string, CountData>();
            [DataMember]
            public string DocName { get; set; }
            [DataMember]
            public Dictionary<string, CountData> WordDic
            {
                get { return wordDic; }
                set { wordDic = value; }
            }

            public void Add(string word)
            {
                wordDic.GetValueOrAdd(word, new CountData() { Word = word }).Add();
            }

            public void Add(string word,int count)
            {
                wordDic.GetValueOrAdd(word, new CountData() { Word = word }).Add(count);
            }




            public IEnumerable<string> GetWordList()
            {
                return wordDic.Keys;
            }


            public void Cut(IEnumerable<string> list)
            {
                foreach (var item in list)
                {
                    if(wordDic.ContainsKey(item))
                    {
                        wordDic.Remove(item);
                    }
                }
            }
        }
        [DataContract]
        public class CountData
        {
            int count = 0;
            [DataMember]
            public string Word { get; set; }
            [DataMember]
            public double Value{get;set;}
            [DataMember]
            public int Count
            {
                get { return count; }
                set { count = value; }
            }
            public void Add()
            {
                count++;
            }
            public void Add(int c)
            {
                count += c;
            }

            public override string ToString()
            {
                return Word + "\t" + Count + "\t" + Value;
            }
        }



        public void Cut(int p)
        {
            CountDic cdic = new CountDic();
            foreach (var item in docDic.Values.SelectMany(n=>n.WordDic))
            {
                cdic.Add(item.Key, item.Value.Count);
            }
            var list = cdic.Dic.Where(n => n.Value < p).Select(n => n.Key).ToArray();
            foreach (var item in docDic)
            {
                item.Value.Cut(list);
            }
        }
    }
}
