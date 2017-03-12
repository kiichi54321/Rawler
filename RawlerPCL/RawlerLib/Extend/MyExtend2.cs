using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// MyExtend .net 4.52以上版
namespace RawlerLib.MyExtend
{
    public static class Paralell
    {

        /// <summary>
        /// 並列数え上げ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="func"></param>
        /// <param name="readRange">それぞれが一度に読み込む量</param>
        /// <param name="ThreadNum">スレッド数</param>
        /// <param name="progressAction">進行状況</param>
        /// <returns></returns>
        public static Dictionary<T, int> ParalellCount<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> func, int readRange, int ThreadNum, Action<int> progressAction)
        {
            System.Collections.Concurrent.ConcurrentStack<T> stack = new System.Collections.Concurrent.ConcurrentStack<T>(source);
            int count = 0;
            int all = stack.Count;
            List<System.Threading.Tasks.Task<Dictionary<T, int>>> tasks = new List<System.Threading.Tasks.Task<Dictionary<T, int>>>();
            for (int i = 0; i < ThreadNum; i++)
            {
                var task = System.Threading.Tasks.Task.Factory.StartNew<Dictionary<T, int>>((n) =>
                {
                    T[] range = new T[readRange];
                    Dictionary<T, int> cDic = new Dictionary<T, int>();                    
                    while (true)
                    {
                        int c = stack.TryPopRange(range);
                        if (c == 0)
                        {
                            break;
                        }
                        foreach (var item in range.Where(m => m != null))
                        {
                            foreach (var item2 in func(item))
                            {
                                cDic.AddCount(item2);
                            }
                        }
                        if (progressAction != null)
                        {
                            var c1 = System.Threading.Interlocked.Add(ref count, c);
                            progressAction((c1 * 100 / all).MaxMin(100, 0));
                        }
                    }
                    return cDic;
                }, System.Threading.Tasks.TaskCreationOptions.LongRunning);
                if (task != null) tasks.Add(task);
            }
            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
            return tasks.Select(n => n.Result).Marge();
        }

        /// <summary>
        /// 並列実行する。blockは、一度に読み取る数。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <param name="block"></param>
        /// <param name="progressAction"></param>
        /// <returns></returns>
        public static IEnumerable<T> ParalellForEach<T>(this IEnumerable<T> source, Action<T> action, int block, Action<int> progressAction)
        {
            int count = 0;
            ConcurrentStack<T> stack = new ConcurrentStack<T>(source);
            int all = stack.Count;
            List<System.Threading.Tasks.Task<List<T>>> tasks = new List<System.Threading.Tasks.Task<List<T>>>();
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                var task = System.Threading.Tasks.Task.Factory.StartNew<List<T>>((n) =>
                {
                    List<T> list = new List<T>();
                    while (true)
                    {
                        T[] s = new T[block];
                        var c = stack.TryPopRange(s);
                        if (c == 0)
                        {
                            break;
                        }
                        foreach (var item in s.Where(m => m != null))
                        {
                            action(item);
                            list.Add(item);
                        }
                        if (progressAction != null)
                        {
                            var c1 = System.Threading.Interlocked.Add(ref count, c);
                            progressAction((c1 * 100 / all).MaxMin(100, 0));
                        }
                    }
                    return list;
                }, System.Threading.Tasks.TaskCreationOptions.LongRunning);
                if (task != null) tasks.Add(task);
            }
            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
            return tasks.SelectMany(n => n.Result);
        }

        /// <summary>
        /// 並列実行する。blockは、一度に読み取る数。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static IEnumerable<T> ParalellForEach<T>(this IEnumerable<T> source, Action<T> action, int block)
        {
            return source.ParalellForEach(action, block, null);
        }

        /// <summary>
        /// 並列実行する。逐次読み出し。
        /// </summary>
        /// <typeparam name="Source"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<T> ParalellForEach<Source, T>(this IEnumerable<Source> source, Func<Source, T> func)
        {
            ConcurrentStack<Source> stack = new ConcurrentStack<Source>(source);

            List<System.Threading.Tasks.Task<List<T>>> tasks = new List<System.Threading.Tasks.Task<List<T>>>();
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                var task = System.Threading.Tasks.Task.Factory.StartNew<List<T>>((n) =>
                {
                    List<T> list = new List<T>();
                    while (true)
                    {
                        Source s;
                        if (stack.TryPop(out s) == false)
                        {
                            break;
                        }
                        list.Add(func(s));
                    }
                    return list;
                }, System.Threading.Tasks.TaskCreationOptions.LongRunning);
                if (task != null) tasks.Add(task);
            }
            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
            return tasks.SelectMany(n => n.Result);
        }

    }

}
