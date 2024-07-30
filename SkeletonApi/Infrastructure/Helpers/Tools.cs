namespace SkeletonApi.Infrastructure.Helpers
{
    public class Tools
    {
        public string SplitData(string str, int index, char deli)
        {
            string[] ArrStr;
            try
            {
                ArrStr = str.Split(deli);
                return ArrStr[index];
            }
            catch
            {
                return "";
            }
        }
    }
}