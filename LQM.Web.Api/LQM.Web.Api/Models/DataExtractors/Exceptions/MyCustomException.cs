
namespace LQM.Web.Api.Models.DataExtractors.Exceptions
{
    public class MyCustomException : Exception
    {
        public MyCustomException()
        {
        }

        public MyCustomException(string message)
            : base(message)
        {
        }

        public MyCustomException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
