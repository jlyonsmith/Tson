namespace TsonLibrary
{
    public struct TextLocation
    {
        public int Offset;
        public int Line;
        public int Column;

        public static readonly TextLocation None = new TextLocation(0, 0, 0);
        public static readonly TextLocation FirstCharacter = new TextLocation(0, 1, 1);

        public TextLocation(int offset)
        {
            this.Offset = offset;
            this.Line = -1;
            this.Column = -1;
        }

        public TextLocation(int offset, int line, int column)
        {
            this.Offset = offset;
            this.Line = line;
            this.Column = column;
        }

        public override string ToString()
        {
            return string.Format("[Offset: {0}, Line: {1}, Col: {2}]", Offset, Line, Column);
        }
    }
}