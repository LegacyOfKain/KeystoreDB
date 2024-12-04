namespace KeystoreDB.Core.Exceptions
{
    public class KeystoreDBException : Exception
    {
        public KeystoreDBException() { }
        public KeystoreDBException(string message) : base(message) { }
        public KeystoreDBException(string message, Exception inner) : base(message, inner) { }
    }
}
