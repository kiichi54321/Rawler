using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace RawlerLib
{
    //public class DynamicDictionary<T> : DynamicObject
    //{
    //    private readonly Dictionary<string, T> dictionary;

    //    public DynamicDictionary(Dictionary<string, T> dictionary)
    //    {
    //        this.dictionary = dictionary;
    //    }

    //    public override bool TryGetMember(
    //        GetMemberBinder binder, out object result)
    //    {
    //        T r1;
    //        var flag = dictionary.TryGetValue(binder.Name, out r1);
    //        result = r1;
    //        return flag;
    //    }


    //    public override bool TrySetMember(
    //        SetMemberBinder binder, object value)
    //    {
    //        dictionary[binder.Name] = (T)value;
           
    //        return true;
    //    }
        
        
        

    //    public override IEnumerable<string> GetDynamicMemberNames()
    //    {
    //        return dictionary.Keys;
    //    }

    //    public string ToJson()
    //    {
    //        return Codeplex.Data.DynamicJson.Serialize(this);
    //    }

    //    public static string ToJson(Dictionary<string, T> dictionary)
    //    {
    //        var d = new DynamicDictionary<T>(dictionary);
    //        return d.ToJson();
    //    }
    //}
}
