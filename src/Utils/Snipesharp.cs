namespace Utils
{
    public class Assembly
    {
        public string GetAssemblyVersion() {
            string fullVersion = GetType().Assembly.GetName().Version.ToString();
            return fullVersion.Substring(0, fullVersion.Length - 2);
        }
    }
}