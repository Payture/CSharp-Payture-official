
namespace CSharpPayture
{
    public class EncodeString
    {
        public string GetPropertiesString()
        {
            var props = this.GetType().GetProperties();
            var result = "";
            foreach ( var prop in props )
            {
                var val = prop.GetValue( this, null );
                if ( val != null )
                    result += $"{prop.Name}={val};";
            }
            return result;
        }

    }
}
