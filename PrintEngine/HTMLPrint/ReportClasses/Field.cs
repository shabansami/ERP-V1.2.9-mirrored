namespace PrintEngine.HTMLPrint
{
    public class Field
    {
        public string FieldName;
        public string HeaderName;

        public Field()
        {
            FieldName = "";
            HeaderName = "Column Header";
        }

        public Field(string fieldName, string headerName)
        {
            FieldName = fieldName;
            HeaderName = headerName;
        }
    }
}
