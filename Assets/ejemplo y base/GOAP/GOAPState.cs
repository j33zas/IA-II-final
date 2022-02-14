using System.Collections.Generic;
using System.Linq;

public class GoapState
{
    public Dictionary<string, bool> values = new Dictionary<string, bool>();
    public GoapAction generatingAction = null;
    public int step = 0;

    #region CONSTRUCTOR
    public GoapState(GoapAction gen = null)
    {
        generatingAction = gen;
    }

    public GoapState(GoapState source, GoapAction gen = null)
    {
        foreach (var elem in source.values)
        {
            if (values.ContainsKey(elem.Key))
                values[elem.Key] = elem.Value;
            else
                values.Add(elem.Key, elem.Value);
        }
        generatingAction = gen;
    }
    #endregion

    public override bool Equals(object obj)
    {
        var result =
            obj is GoapState other
            && other.generatingAction == generatingAction  
            && other.values.Count == values.Count
            && other.values.All(kv => kv.In(values));
        return result;
    }

    public override int GetHashCode()
    {
        return values.Count == 0 ? 0 : 31 * values.Count + 31 * 31 * values.First().GetHashCode();
    }

    public override string ToString()
    {
        var str = "";
        foreach (var kv in values.OrderBy(x => x.Key))
        {
            str += (string.Format("{0:12} : {1}\n", kv.Key, kv.Value));
        }
        return ("--->" + (generatingAction != null ? generatingAction.Name : "NULL") + "\n" + str);
    }
}
