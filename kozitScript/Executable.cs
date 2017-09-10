namespace kozitScript
{
    public class Executable
    {

        public byte Type
        {
            get; private set;
        }
        public Executable(byte type)
        {
            Type = type;
        }
        public virtual Instruction[] CPU()
        {
            return new Instruction[] { };
        }

    }
}