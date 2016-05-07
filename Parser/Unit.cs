namespace Parser
{
    public class Unit
    {
        private Unit() { }

        private static readonly Unit _instance = new Unit();
        public static Unit Instance
        {
            get { return _instance; }
        }
    }
}