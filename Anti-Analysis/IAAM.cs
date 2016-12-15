namespace Anti_Analysis
{
    abstract class IAAM
    {
        public abstract string Name { get; }
        public abstract string Description { get; }

        public abstract void DoWork(DetectionMethod m);
    }
}
