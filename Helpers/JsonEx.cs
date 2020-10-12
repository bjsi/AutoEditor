using Newtonsoft.Json;

namespace AutoEditor.Helpers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "<Pending>")]
    public static class JsonEx
    {
        #region Methods

        public static string Serialize(this object obj,
                                       Formatting format = Formatting.None)
        {
            return JsonConvert.SerializeObject(obj, format);
        }

        public static T Deserialize<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        #endregion
    }
}
