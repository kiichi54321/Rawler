using Rawler.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RawlerLib.MyExtend;

namespace Rawler.NPL
{
 /// <summary>
    /// TFIDFを行う
 /// </summary>
    public class TFIDFAnalyze
    {
        Dictionary<string, DocumentData> docDic = new Dictionary<string, DocumentData>();

        public Dictionary<string, DocumentData> DocDic
        {
            get { return docDic; }
        }
        public void DicClear()
        {
            docDic.Clear();
            idfDic = null;
            documentId = 0;
        }
        
        public void Add(string docName,string word)
        {
            docDic.GetValueOrAdd(docName, new DocumentData() { DocName = docName }).Add(word);
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

        public Dictionary<string, CountData> CreateIDFdic()
        {
            idfDic = docDic.Values.AsParallel().SelectMany(n => n.GetWordList()).GroupBy(n => n).Select(n => new CountData() { Word = n.Key, Count = n.Count(), Value = Math.Log(docDic.Count / (double)(n.Count() + 1)) }).ToDictionary(n => n.Word);
            return idfDic;
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

        public IEnumerable<TfidfResult> GetResult()
        {
            if (idfDic == null) CreateIDFdic();
            var all = docDic.SelectMany(n=>n.Value.WordDic.Values).Sum(n=>n.Count);
            return docDic.AsParallel().SelectMany(n => n.Value.WordDic.Values).GroupBy(n => n.Word).Select(n => new TfidfResult() { Word = n.Key, TF = n.Sum(m => m.Count) / (double)all, IDF = idfDic.GetValueOrDefault(n.Key).Value }).OrderByDescending(n => n.TFIDF);               
        }



        public class TfidfResult
        {
            public string Word { get; set; }
            public double TF { get; set; }
            public double IDF { get; set; }
            public double TFIDF { get{return TF*IDF;}}
        }
        public class DocumentData
        {
            Dictionary<string, CountData> wordDic = new Dictionary<string, CountData>();
            public string DocName { get; set; }

            public Dictionary<string, CountData> WordDic
            {
                get { return wordDic; }
               // set { wordDic = value; }
            }

            public void Add(string word)
            {
                wordDic.GetValueOrAdd(word, new CountData() { Word = word }).Add();
            }

            public IEnumerable<string> GetWordList()
            {
                return wordDic.Keys;
            }
            
        }
        public class CountData
        {
            int count = 0;

            public string Word { get; set; }
            public double Value{get;set;}

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
        }

       
    }
}
